//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    internal sealed class RemoteCommandHub : RemoteEndpointProxyHub<CommandSetProxy>, ISendCommandsToRemoteEndpoints
    {
        /// <summary>
        /// The collection that holds all the <see cref="ICommandSet"/> proxies for each endpoint that
        /// has been registered.
        /// </summary>
        private readonly IDictionary<EndpointId, IDictionary<Type, CommandSetProxy>> m_RemoteCommands
            = new Dictionary<EndpointId, IDictionary<Type, CommandSetProxy>>();

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
        /// <param name="commandReporter">The object that reports when a new command is registered on a remote endpoint.</param>
        /// <param name="builder">The object that is responsible for building the command proxies.</param>
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
        internal RemoteCommandHub(
            ICommunicationLayer layer,
            IReportNewProxies commandReporter,
            CommandProxyBuilder builder, 
            Action<LogSeverityProxy, string> logger)
            : base(
                layer,
                commandReporter,
                (endpoint, type) => (CommandSetProxy)builder.ProxyConnectingTo(endpoint, type),
                logger)
        {
            {
                Enforce.Argument(() => builder);
                Enforce.Argument(() => logger);
            }

            m_Builder = builder;
            m_Logger = logger;
        }

        /// <summary>
        /// Returns a value indicating if one or more proxies exist for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if one or more proxies exist for the endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected override bool HasProxyFor(EndpointId endpoint)
        {
            return m_RemoteCommands.ContainsKey(endpoint);
        }

        /// <summary>
        /// Creates an <see cref="ICommunicationMessage"/> that requests information about the desired proxy types.
        /// </summary>
        /// <param name="endpointId">The ID number of the current endpoint.</param>
        /// <returns>
        /// The <see cref="ICommunicationMessage"/> that will be used to request the correct information.
        /// </returns>
        protected override ICommunicationMessage CreateInformationRequestMessage(EndpointId endpointId)
        {
            return new CommandInformationRequestMessage(endpointId);
        }

        /// <summary>
        /// Adds the collection of proxies to the storage.
        /// </summary>
        /// <param name="endpoint">The endpoint from which the proxies came.</param>
        /// <param name="list">The collection of proxies.</param>
        protected override void AddProxiesToStorage(EndpointId endpoint, SortedList<Type, CommandSetProxy> list)
        {
            if (!m_RemoteCommands.ContainsKey(endpoint))
            {
                m_RemoteCommands.Add(endpoint, list);
            }
            else
            {
                foreach (var pair in list)
                {
                    var existingList = (SortedList<Type, CommandSetProxy>)m_RemoteCommands[endpoint];
                    if (!existingList.ContainsKey(pair.Key))
                    {
                        existingList.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the proxy to the storage.
        /// </summary>
        /// <param name="endpoint">The endpoint from which the proxies came.</param>
        /// <param name="proxyType">The type of the proxy.</param>
        /// <param name="proxy">The proxy.</param>
        protected override void AddProxyFor(EndpointId endpoint, Type proxyType, CommandSetProxy proxy)
        {
            if (m_RemoteCommands.ContainsKey(endpoint))
            {
                var list = m_RemoteCommands[endpoint];
                if (!list.ContainsKey(proxyType))
                {
                    list.Add(proxyType, proxy);
                }
            }
            else
            {
                var list = new SortedList<Type, CommandSetProxy>(new TypeComparer());
                list.Add(proxyType, proxy);
                m_RemoteCommands.Add(endpoint, list);
            }
        }

        /// <summary>
        /// Removes all the proxies for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint for which all the proxies have to be removed.</param>
        protected override void RemoveProxiesFor(EndpointId endpoint)
        {
            m_RemoteCommands.Remove(endpoint);
        }

        /// <summary>
        /// Returns a collection describing all the known endpoints and the commands they
        /// provide.
        /// </summary>
        /// <returns>
        /// The collection describing all the known endpoints and the commands they describe.
        /// </returns>
        public IEnumerable<CommandInformationPerEndpoint> AvailableCommands()
        {
            var result = new List<CommandInformationPerEndpoint>(); 
            lock (m_Lock)
            {
                foreach (var pair in m_RemoteCommands)
                {
                    var list = new List<Type>();
                    list.AddRange(pair.Value.Keys);
                    result.Add(new CommandInformationPerEndpoint(pair.Key, list));
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a collection describing all the known commands for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     The collection describing all the known commands for the given endpoint.
        /// </returns>
        public IEnumerable<Type> AvailableCommandsFor(EndpointId endpoint)
        {
            var result = new List<Type>();
            lock (m_Lock)
            {
                if (m_RemoteCommands.ContainsKey(endpoint))
                {
                    var list = m_RemoteCommands[endpoint];
                    result.AddRange(list.Keys);
                }
            }

            return result;
        }

        /// <summary>
        /// An event raised when an endpoint signs on and provides a set of commands.
        /// </summary>
        public event EventHandler<CommandSetAvailabilityEventArgs> OnEndpointSignedIn;

        protected override void RaiseOnEndpointSignedIn(EndpointId endpoint, IEnumerable<Type> commands)
        {
            var local = OnEndpointSignedIn;
            if (local != null)
            {
                local(this, new CommandSetAvailabilityEventArgs(endpoint, commands));
            }
        }

        /// <summary>
        /// An event raised when an endpoint signs off.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointSignedOff;

        protected override void RaiseOnEndpointSignedOff(EndpointId endpoint)
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
            return CommandsFor(endpoint, typeof(TCommand)) as TCommand;
        }

        /// <summary>
        /// Returns the command proxy for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint for which the commands should be returned.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <returns>The requested command set.</returns>
        public ICommandSet CommandsFor(EndpointId endpoint, Type commandType)
        {
            lock (m_Lock)
            {
                if (!m_RemoteCommands.ContainsKey(endpoint))
                {
                    return null;
                }

                var commandSets = m_RemoteCommands[endpoint];
                if (!commandSets.ContainsKey(commandType))
                {
                    throw new CommandNotSupportedException(commandType);
                }

                var result = commandSets[commandType];
                return result as ICommandSet;
            }
        }
    }
}
