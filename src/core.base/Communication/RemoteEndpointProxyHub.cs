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
        /// their available commands.
        /// </summary>
        private readonly IList<EndpointId> m_WaitingForCommandInformation
            = new List<EndpointId>();

        /// <summary>
        /// The communication layer which handles the sending and receiving of messages.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// The object that reports when a new proxy is registered on a remote endpoint.
        /// </summary>
        private readonly IReportNewProxies m_CommandReporter;

        /// <summary>
        /// The function that creates command proxy objects.
        /// </summary>
        private readonly Func<EndpointId, Type, TProxyObject> m_Builder;

        /// <summary>
        /// The function used to write messages to the log.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        protected readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteEndpointProxyHub{TProxyObject}"/> class.
        /// </summary>
        /// <param name="layer">The communication layer that will handle the actual connections.</param>
        /// <param name="commandReporter">The object that reports when a new command is registered on a remote endpoint.</param>
        /// <param name="builder">The fun ction that is responsible for building the command proxies.</param>
        /// <param name="logger">The function that is used to write messages to the log.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandReporter"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        protected RemoteEndpointProxyHub(
            ICommunicationLayer layer,
            IReportNewProxies commandReporter,
            Func<EndpointId, Type, TProxyObject> builder, 
            Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => layer);
                Enforce.Argument(() => builder);
                Enforce.Argument(() => logger);
            }

            m_Layer = layer;
            {
                m_Layer.OnEndpointSignedIn += (s, e) => HandleEndpointSignIn(e.ConnectionInformation);
                m_Layer.OnEndpointSignedOut += (s, e) => HandleEndpointSignOut(e.Endpoint);
            }

            m_CommandReporter = commandReporter;
            {
                m_CommandReporter.OnNewProxyRegistered += (s, e) => HandleNewProxyReported(e.Endpoint, e.Proxy);
            }

            m_Builder = builder;
            m_Logger = logger;
        }

        private void HandleEndpointSignIn(ChannelConnectionInformation channelConnectionInformation)
        {
            // See if we already had the endpoint information. If so then we don't really care
            // If not then we should get the command information
            var endpoint = channelConnectionInformation.Id;
            if (!HasProxyFor(endpoint))
            {
                // Contact the endpoint and ask for information about the 
                // available commands. Because this will take some time we'll just ignore
                // the fact that the endpoint signed on and we'll pretend it didn't happen (yet).
                lock (m_Lock)
                {
                    if (m_WaitingForCommandInformation.Contains(endpoint))
                    {
                        // We've already contacted the endpoint and we're still waiting.
                        m_Logger(
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
                    m_WaitingForCommandInformation.Add(endpoint);

                    task.ContinueWith(
                        t =>
                        {
                            ProcessInformationResponse(t, endpoint);
                        },
                        TaskContinuationOptions.ExecuteSynchronously);
                }
            }
        }

        private void ProcessInformationResponse(Task<ICommunicationMessage> task, EndpointId endpoint)
        {
            bool haveStoredCommandInformation = false;
            var commandList = new List<Type>();
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
                        m_Logger(
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
                        if (m_WaitingForCommandInformation.Contains(endpoint))
                        {
                            Debug.Assert(!HasProxyFor(endpoint), "There shouldn't be any endpoint information");

                            // We expect that each endpoint will only have a few proxy types but there might be a lot of 
                            // endpoints. Also accessing any method in a command set proxy is going to be slow because it 
                            // needs to travel to another application and come back (possibly over the network). it seems the
                            // sorted list is the best trade-off (memory vs performance) in this case.
                            var commands = msg.ProxyTypes;
                            m_Logger(
                                LogSeverityProxy.Trace,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Received {0} {1} from endpoint [{2}].",
                                    commands.Count,
                                    TraceNameForProxyObjects(),
                                    endpoint));

                            var list = new SortedList<Type, TProxyObject>(commands.Count, new TypeComparer());
                            foreach (var command in commands)
                            { 
                                // Hydrate the command type. This requires loading the assembly which a) might
                                // be slow and b) might fail
                                Type commandSetType = LoadProxyType(endpoint, command);
                                list.Add(commandSetType, m_Builder(endpoint, commandSetType));
                            }

                            if (list.Count > 0)
                            {
                                AddProxiesToStorage(endpoint, list);
                                haveStoredCommandInformation = true;
                                commandList.AddRange(list.Keys);
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
                haveStoredCommandInformation = false;
            }
            finally 
            {
                lock (m_Lock)
                {
                    if (m_WaitingForCommandInformation.Contains(endpoint))
                    {
                        m_Logger(
                           LogSeverityProxy.Trace,
                           string.Format(
                               CultureInfo.InvariantCulture,
                               "No longer waiting for {0} from endpoint: {1}",
                               TraceNameForProxyObjects(),
                               endpoint));

                        m_WaitingForCommandInformation.Remove(endpoint);
                    }
                }
            }

            // Notify the outside world that we have more commands. Do this outside
            // the lock because a) the notification may take a while and b) it 
            // may trigger all kinds of other mayhem.
            if (haveStoredCommandInformation)
            {
                if (commandList.Count > 0)
                {
                    RaiseOnEndpointSignedIn(endpoint, commandList);
                }
            }
        }

        private void HandleEndpointSignOut(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                if (m_WaitingForCommandInformation.Contains(endpoint))
                {
                    m_Logger(
                        LogSeverityProxy.Trace,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "No longer waiting for {0} from endpoint [{1}].",
                            TraceNameForProxyObjects(),
                            endpoint));

                    m_WaitingForCommandInformation.Remove(endpoint);
                }

                if (HasProxyFor(endpoint))
                {
                    m_Logger(
                        LogSeverityProxy.Trace,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Removing {0} for endpoint [{1}].",
                            TraceNameForProxyObjects(),
                            endpoint));

                    RemoveProxiesFor(endpoint);
                }
            }

            RaiseOnEndpointSignedOff(endpoint);
        }

        private void HandleNewProxyReported(EndpointId endpoint, ISerializedType serializedProxyType)
        {
            // We're going to assume that if we hear about a new command then we already have information
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
            // Hydrate the command type. This requires loading the assembly which a) might
            // be slow and b) might fail
            Type proxyType = null;
            try
            {
                proxyType = ProxyExtensions.ToType(serializedType);

                m_Logger(
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
                m_Logger(
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
