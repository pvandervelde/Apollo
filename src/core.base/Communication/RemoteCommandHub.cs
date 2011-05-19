//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
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
    /// Defines the methods for handling communication commands.
    /// </summary>
    /// <remarks>
    /// Objects can register <see cref="ICommandSet"/> implementations with the <see cref="ICommandCollection"/>. The 
    /// availability and definition of these commands is then passed on to all endpoints that are connected
    /// to the current endpoint. Upon reception of command information an endpoint will generate a proxy for
    /// the command interface thereby allowing remote invocation of commands through the proxy command interface.
    /// </remarks>
    public sealed class RemoteCommandHub
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection that holds all the <see cref="ICommandSet"/> proxies for each endpoint that
        /// has been registered.
        /// </summary>
        private readonly IDictionary<EndpointId, IDictionary<Type, CommandSetProxy>> m_RemoteCommands
            = new Dictionary<EndpointId, IDictionary<Type, CommandSetProxy>>();

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
        /// The object that creates command proxy objects.
        /// </summary>
        private readonly CommandProxyBuilder m_Builder;

        /// <summary>
        /// The function used to write messages to the log.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCommandHub"/> class.
        /// </summary>
        /// <param name="layer">The communication layer that will handle the actual connections.</param>
        /// <param name="builder">The object that is responsible for building the command proxies.</param>
        /// <param name="logger">The function that is used to write messages to the log.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        internal RemoteCommandHub(
            ICommunicationLayer layer, 
            CommandProxyBuilder builder, 
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

            m_Builder = builder;
            m_Logger = logger;
        }

        private void HandleEndpointSignIn(ChannelConnectionInformation channelConnectionInformation)
        {
            // See if we already had the endpoint information. If so then we don't really care
            // If not then we should get the command information
            var endpoint = channelConnectionInformation.Id;
            if (!m_RemoteCommands.ContainsKey(endpoint))
            {
                // Contact the endpoint and ask for information about the 
                // available commands. Because this will take some time we'll just ignore
                // the fact that the endpoint signed on and we'll pretend it didn't happen (yet).
                lock (m_Lock)
                {
                    if (m_WaitingForCommandInformation.Contains(endpoint))
                    {
                        // We've already contacted the endpoint and we're still waiting.
                        return;
                    }

                    // Send out a message and ask for more information
                    var msg = new EndpointInformationRequestMessage(m_Layer.Id);
                    var task = m_Layer.SendMessageAndWaitForResponse(endpoint, msg);
                    m_WaitingForCommandInformation.Add(endpoint);

                    task.ContinueWith(
                        t =>
                        {
                            ProcessInformationResponse(t, endpoint);
                        });
                }
            }
        }

        private void ProcessInformationResponse(Task<ICommunicationMessage> task, EndpointId endpoint)
        {
            bool haveStoredCommandInformation = false;
            try
            {
                if (task.IsCompleted)
                {
                    var msg = task.Result as EndpointInformationResponseMessage;
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
                            Debug.Assert(!m_RemoteCommands.ContainsKey(endpoint), "There shouldn't be any endpoint information");

                            // We expect that each endpoint will only have a few command sets but there might be a lot of 
                            // endpoints. Also accessing any method in a command set proxy is going to be slow because it 
                            // needs to travel to another application and come back (possibly over the network). it seems the
                            // sorted list is the best trade-off (memory vs performance) in this case.
                            var commands = msg.Commands;
                            m_RemoteCommands.Add(endpoint, new SortedList<Type, CommandSetProxy>(commands.Count, new TypeComparer()));
                            
                            var list = m_RemoteCommands[endpoint];
                            foreach (var command in commands)
                            { 
                                // Hydrate the command type. This requires loading the assembly which a) might
                                // be slow and b) might fail
                                Type commandSetType = null;
                                try
                                {
                                    commandSetType = CommandSetProxyExtensions.ToType(command);
                                }
                                catch (UnableToLoadCommandSetTypeException)
                                {
                                    m_Logger(
                                        LogSeverityProxy.Error,
                                        string.Format(
                                            CultureInfo.InvariantCulture,
                                            "Could not load the command set type: {0} for endpoint {1}",
                                            command.AssemblyQualifiedTypeName,
                                            endpoint));

                                    throw;
                                }

                                list.Add(commandSetType, (CommandSetProxy)m_Builder.ProxyConnectingTo(endpoint, commandSetType));
                            }

                            haveStoredCommandInformation = true;
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
                        m_WaitingForCommandInformation.Remove(endpoint);
                    }
                }
            }

            // Notify the outside world that we have more commands. Do this outside
            // the lock because a) the notification may take a while and b) it 
            // may trigger all kinds of other mayhem.
            if (haveStoredCommandInformation)
            {
                RaiseOnEndpointSignedIn();
            }
        }

        private void HandleEndpointSignOut(EndpointId endpoint)
        {
            IDictionary<Type, CommandSetProxy> commands = null;
            lock (m_Lock)
            {
                if (m_WaitingForCommandInformation.Contains(endpoint))
                {
                    m_WaitingForCommandInformation.Remove(endpoint);
                }

                if (m_RemoteCommands.ContainsKey(endpoint))
                {
                    commands = m_RemoteCommands[endpoint];
                    m_RemoteCommands.Remove(endpoint);
                }
            }

            if (commands != null)
            {
                foreach (var pair in commands)
                {
                    pair.Value.EndpointHasSignedOff();
                }
            }

            RaiseOnEndpointSignedOff(endpoint);
        }

        /// <summary>
        /// An event raised when an endpoint signs on and provides a set of commands.
        /// </summary>
        public event EventHandler<CommandSetAvailabilityEventArgs> OnEndpointSignedIn;

        private void RaiseOnEndpointSignedIn()
        {
            var local = OnEndpointSignedIn;
            if (local != null)
            {
                local(this, new CommandSetAvailabilityEventArgs());
            }
        }

        /// <summary>
        /// An event raised when an endpoint signs off.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointSignedOff;

        private void RaiseOnEndpointSignedOff(EndpointId endpoint)
        {
            var local = OnEndpointSignedOff;
            if (local != null)
            {
                local(this, new EndpointEventArgs(endpoint));
            }
        }

        /// <summary>
        /// Returns a value indicating if there are any known commands for a given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if there are known commands for the given endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasCommandsFor(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                return m_RemoteCommands.ContainsKey(endpoint);
            }
        }

        /// <summary>
        /// Returns a value indicating if a specific set of commands is available for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <param name="commandInterfaceType">The type of the command that should be available.</param>
        /// <returns>
        ///     <see langword="true" /> if there are the speicfic commands exist for the given endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasCommandFor(EndpointId endpoint, Type commandInterfaceType)
        {
            lock (m_Lock)
            {
                if (m_RemoteCommands.ContainsKey(endpoint))
                {
                    var commands = m_RemoteCommands[endpoint];
                    return commands.ContainsKey(commandInterfaceType);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the command proxy for the given endpoint.
        /// </summary>
        /// <typeparam name="TCommand">The typeof command set that should be returned.</typeparam>
        /// <param name="endpoint">The ID number of the endpoint for which the commands should be returned.</param>
        /// <returns>The requested command set.</returns>
        public TCommand CommandsFor<TCommand>(EndpointId endpoint) where TCommand : class, ICommandSet
        {
            if (!m_RemoteCommands.ContainsKey(endpoint))
            {
                return null;
            }

            var commandSets = m_RemoteCommands[endpoint];
            if (!commandSets.ContainsKey(typeof(TCommand)))
            {
                throw new CommandNotSupportedException(typeof(TCommand));
            }

            var result = commandSets[typeof(TCommand)];
            return result as TCommand;
        }

        /// <summary>
        /// Closes the connection to the given endpoint.
        /// </summary>
        /// <remarks>
        /// Closing the connection to a given endpoint also invalidates
        /// all commandsets for that endpoint.
        /// </remarks>
        /// <param name="endpoint">The ID of the endpoint with which the connection should be closed.</param>
        public void CloseConnectionTo(EndpointId endpoint)
        {
            m_Layer.DisconnectFromEndpoint(endpoint);
        }

        /// <summary>
        /// Closes all connections.
        /// </summary>
        public void CloseConnections()
        {
            var endpoints = new List<EndpointId>();
            lock (m_Lock)
            {
                endpoints.AddRange(m_RemoteCommands.Keys);
                endpoints.AddRange(m_WaitingForCommandInformation);
            }

            foreach (var endpoint in endpoints)
            {
                CloseConnectionTo(endpoint);
            }
        }
    }
}
