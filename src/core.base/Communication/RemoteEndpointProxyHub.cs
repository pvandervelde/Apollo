//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Utilities;
using Lokad;
using Utilities.Diagnostics.Profiling;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// The base class for classes that store proxies for one or more remote endpoints.
    /// </summary>
    /// <typeparam name="TProxyObject">The base type of the proxy object that is created by the proxy builder.</typeparam>
    internal abstract class RemoteEndpointProxyHub<TProxyObject> where TProxyObject : class
    {
        /// <summary>
        /// The collection of endpoints which have been contacted for information about 
        /// their available proxies.
        /// </summary>
        private readonly IList<EndpointId> m_WaitingForEndpointInformation
            = new List<EndpointId>();

        /// <summary>
        /// The communication layer which handles the sending and receiving of messages.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// The object that reports when a new proxy is registered on a remote endpoint.
        /// </summary>
        private readonly IReportNewProxies m_ProxyReporter;

        /// <summary>
        /// The function that creates proxy objects.
        /// </summary>
        private readonly Func<EndpointId, Type, TProxyObject> m_Builder;

        /// <summary>
        /// The object that provides the diagnostic methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        protected readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteEndpointProxyHub{TProxyObject}"/> class.
        /// </summary>
        /// <param name="layer">The communication layer that will handle the actual connections.</param>
        /// <param name="proxyReporter">The object that reports when a new proxy is registered on a remote endpoint.</param>
        /// <param name="builder">The function that is responsible for building the proxies.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="proxyReporter"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        protected RemoteEndpointProxyHub(
            ICommunicationLayer layer,
            IReportNewProxies proxyReporter,
            Func<EndpointId, Type, TProxyObject> builder,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Enforce.Argument(() => layer);
                Enforce.Argument(() => builder);
                Enforce.Argument(() => systemDiagnostics);
            }

            m_Layer = layer;
            {
                m_Layer.OnEndpointSignedIn += (s, e) => HandleEndpointSignIn(e.ConnectionInformation);
                m_Layer.OnEndpointSignedOut += (s, e) => HandleEndpointSignOut(e.Endpoint);
            }

            m_ProxyReporter = proxyReporter;
            {
                m_ProxyReporter.OnNewProxyRegistered += (s, e) => HandleNewProxyReported(e.Endpoint, e.Proxy);
            }

            m_Builder = builder;
            m_Diagnostics = systemDiagnostics;
        }

        private void HandleEndpointSignIn(ChannelConnectionInformation channelConnectionInformation)
        {
            using (var interval = m_Diagnostics.Profiler.Measure("Getting endpoint information async."))
            {
                // See if we already had the endpoint information. If so then we don't really care
                // If not then we should get the command information
                var endpoint = channelConnectionInformation.Id;
                if (!HasProxyFor(endpoint))
                {
                    // Contact the endpoint and ask for information about the 
                    // available proxies. Because this will take some time we'll just ignore
                    // the fact that the endpoint signed on and we'll pretend it didn't happen (yet).
                    lock (m_Lock)
                    {
                        if (m_WaitingForEndpointInformation.Contains(endpoint))
                        {
                            // We've already contacted the endpoint and we're still waiting.
                            m_Diagnostics.Log(
                                LogSeverityProxy.Trace,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Already waiting for {0} from endpoint [{1}].",
                                    TraceNameForProxyObjects(),
                                    endpoint));
                            return;
                        }

                        // Send out a message and ask for more information
                        var msg = CreateInformationRequestMessage(m_Layer.Id);
                        var task = m_Layer.SendMessageAndWaitForResponse(endpoint, msg);
                        m_WaitingForEndpointInformation.Add(endpoint);

                        task.ContinueWith(
                            t =>
                            {
                                ProcessInformationResponse(t, endpoint);
                            },
                            TaskContinuationOptions.ExecuteSynchronously);
                    }
                }
            }
        }

        private void ProcessInformationResponse(Task<ICommunicationMessage> task, EndpointId endpoint)
        {
            using (var interval = m_Diagnostics.Profiler.Measure("Received endpoint information"))
            {
                bool haveStoredEndpointInformation = false;
                var proxyList = new List<Type>();
                try
                {
                    if (task.IsCompleted)
                    {
                        var msg = task.Result as EndpointProxyTypesResponseMessage;
                        if (msg == null)
                        {
                            // The message isn't the one we were expecting. Unfortunately 
                            // there's nothing we can do about it. We'll just pretend we
                            // didn't get a response and remove the endpoint from the
                            // collection.
                            m_Diagnostics.Log(
                                LogSeverityProxy.Warning,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "The information about endpoint: {0} was missing",
                                    endpoint));

                            return;
                        }

                        lock (m_Lock)
                        {
                            // Only push the changes in to the collection if we
                            // are indeed waiting for the changes. If the endpoint
                            // was removed at some point before we get here then 
                            // we apparently don't care anymore and we just ignore it
                            if (m_WaitingForEndpointInformation.Contains(endpoint))
                            {
                                Debug.Assert(!HasProxyFor(endpoint), "There shouldn't be any endpoint information");

                                // We expect that each endpoint will only have a few proxy types but there might be a lot of 
                                // endpoints. Also accessing any method in a proxy is going to be slow because it 
                                // needs to travel to another application and come back (possibly over the network). it seems the
                                // sorted list is the best trade-off (memory vs performance) in this case.
                                var proxyTypes = msg.ProxyTypes;
                                m_Diagnostics.Log(
                                    LogSeverityProxy.Trace,
                                    string.Format(
                                        CultureInfo.InvariantCulture,
                                        "Received {0} {1} from endpoint [{2}].",
                                        proxyTypes.Count,
                                        TraceNameForProxyObjects(),
                                        endpoint));

                                var list = new SortedList<Type, TProxyObject>(proxyTypes.Count, new TypeComparer());
                                foreach (var proxy in proxyTypes)
                                {
                                    // Hydrate the proxy type. This requires loading the assembly which a) might
                                    // be slow and b) might fail
                                    Type proxyType = LoadProxyType(endpoint, proxy);
                                    list.Add(proxyType, m_Builder(endpoint, proxyType));
                                }

                                if (list.Count > 0)
                                {
                                    AddProxiesToStorage(endpoint, list);
                                    haveStoredEndpointInformation = true;
                                    proxyList.AddRange(list.Keys);
                                }
                            }
                        }
                    }
                }
                catch (AggregateException)
                {
                    // We don't really care about any exceptions that were thrown
                    // If there was a problem we just remove the endpoint from the
                    // 'waiting list' and move on.
                    haveStoredEndpointInformation = false;
                }
                finally
                {
                    lock (m_Lock)
                    {
                        if (m_WaitingForEndpointInformation.Contains(endpoint))
                        {
                            m_Diagnostics.Log(
                               LogSeverityProxy.Trace,
                               string.Format(
                                   CultureInfo.InvariantCulture,
                                   "No longer waiting for {0} from endpoint: {1}",
                                   TraceNameForProxyObjects(),
                                   endpoint));

                            m_WaitingForEndpointInformation.Remove(endpoint);
                        }
                    }
                }

                // Notify the outside world that we have more proxies. Do this outside
                // the lock because a) the notification may take a while and b) it 
                // may trigger all kinds of other mayhem.
                if (haveStoredEndpointInformation)
                {
                    if (proxyList.Count > 0)
                    {
                        using (var innerInterval = m_Diagnostics.Profiler.Measure("Notifying of new endpoint"))
                        {
                            RaiseOnEndpointSignedIn(endpoint, proxyList);
                        }
                    }
                }
            }
        }

        private void HandleEndpointSignOut(EndpointId endpoint)
        {
            using (var interval = m_Diagnostics.Profiler.Measure("Endpoint disconnected - removing proxies."))
            {
                lock (m_Lock)
                {
                    if (m_WaitingForEndpointInformation.Contains(endpoint))
                    {
                        m_Diagnostics.Log(
                            LogSeverityProxy.Trace,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "No longer waiting for {0} from endpoint [{1}].",
                                TraceNameForProxyObjects(),
                                endpoint));

                        m_WaitingForEndpointInformation.Remove(endpoint);
                    }

                    if (HasProxyFor(endpoint))
                    {
                        m_Diagnostics.Log(
                            LogSeverityProxy.Trace,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Removing {0} for endpoint [{1}].",
                                TraceNameForProxyObjects(),
                                endpoint));

                        RemoveProxiesFor(endpoint);
                    }
                }

                using (var innerInterval = m_Diagnostics.Profiler.Measure("Notifying of proxy removal."))
                {
                    RaiseOnEndpointSignedOff(endpoint);
                }
            }
        }

        private void HandleNewProxyReported(EndpointId endpoint, ISerializedType serializedProxyType)
        {
            // We're going to assume that if we hear about a new proxy then we already have information
            // about the endpoint. This may well be an incorrect assumption but for now it will do.
            Type proxyType = LoadProxyType(endpoint, serializedProxyType);
            lock (m_Lock)
            {
                var proxy = (TProxyObject)m_Builder(endpoint, proxyType);
                AddProxyFor(endpoint, proxyType, proxy);
            }
        }

        private Type LoadProxyType(EndpointId endpoint, ISerializedType serializedType)
        {
            // Hydrate the proxy type. This requires loading the assembly which a) might
            // be slow and b) might fail
            Type proxyType = null;
            try
            {
                proxyType = ProxyExtensions.ToType(serializedType);

                m_Diagnostics.Log(
                    LogSeverityProxy.Trace,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Got {0} from endpoint [{1}] of type {2}.",
                        TraceNameForProxyObjects(),
                        endpoint,
                        proxyType));
            }
            catch (UnableToLoadProxyTypeException)
            {
                m_Diagnostics.Log(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Could not load the {0} type: {1} for endpoint {2}",
                        TraceNameForProxyObjects(),
                        serializedType.AssemblyQualifiedTypeName,
                        endpoint));

                throw;
            }

            return proxyType;
        }

        /// <summary>
        /// Returns the name of the proxy objects for use in the trace logs.
        /// </summary>
        /// <returns>A string containing the name of the proxy objects for use in the trace logs.</returns>
        protected abstract string TraceNameForProxyObjects();

        /// <summary>
        /// Returns a value indicating if one or more proxies exist for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if one or more proxies exist for the endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected abstract bool HasProxyFor(EndpointId endpoint);

        /// <summary>
        /// Creates an <see cref="ICommunicationMessage"/> that requests information about the desired proxy types.
        /// </summary>
        /// <param name="endpointId">The ID number of the current endpoint.</param>
        /// <returns>
        /// The <see cref="ICommunicationMessage"/> that will be used to request the correct information.
        /// </returns>
        protected abstract ICommunicationMessage CreateInformationRequestMessage(EndpointId endpointId);

        /// <summary>
        /// Adds the collection of proxies to the storage.
        /// </summary>
        /// <param name="endpoint">The endpoint from which the proxies came.</param>
        /// <param name="list">The collection of proxies.</param>
        protected abstract void AddProxiesToStorage(EndpointId endpoint, SortedList<Type, TProxyObject> list);

        /// <summary>
        /// Adds the proxy to the storage.
        /// </summary>
        /// <param name="endpoint">The endpoint from which the proxies came.</param>
        /// <param name="proxyType">The type of the proxy.</param>
        /// <param name="proxy">The proxy.</param>
        protected abstract void AddProxyFor(EndpointId endpoint, Type proxyType, TProxyObject proxy);

        /// <summary>
        /// Removes all the proxies for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint for which all the proxies have to be removed.</param>
        protected abstract void RemoveProxiesFor(EndpointId endpoint);

        /// <summary>
        /// Indicates that a new endpoint has signed in and all the proxy information has been obtained.
        /// </summary>
        /// <param name="endpoint">The endpoint that has signed in.</param>
        /// <param name="proxies">The proxy types for the given endpoint.</param>
        protected abstract void RaiseOnEndpointSignedIn(EndpointId endpoint, IEnumerable<Type> proxies);

        /// <summary>
        /// Indicates that an endpoint has signed off.
        /// </summary>
        /// <param name="endpoint">The endpoint that signed off.</param>
        protected abstract void RaiseOnEndpointSignedOff(EndpointId endpoint);
    }
}
