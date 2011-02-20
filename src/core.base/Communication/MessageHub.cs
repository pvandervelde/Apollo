//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for handling communication commands.
    /// </summary>
    public sealed class MessageHub
    {
        /// <summary>
        /// The communication layer which handles the sending and receiving of messages.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// The object that creates command proxy objects.
        /// </summary>
        private readonly CommandProxyBuilder m_Builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHub"/> class.
        /// </summary>
        /// <param name="layer">The communication layer that will handle the actual connections.</param>
        /// <param name="builder">The object that is responsible for building the command proxies.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builder"/> is <see langword="null" />.
        /// </exception>
        internal MessageHub(ICommunicationLayer layer, CommandProxyBuilder builder)
        {
            {
                Enforce.Argument(() => layer);
                Enforce.Argument(() => builder);
            }

            m_Layer = layer;
            m_Builder = builder;
        }

        /// <summary>
        /// Returns the command proxy for the given endpoint.
        /// </summary>
        /// <typeparam name="TCommand">The typeof command set that should be returned.</typeparam>
        /// <param name="endpoint">The ID number of the endpoint for which the commands should be returned.</param>
        /// <returns>The requested command set.</returns>
        public TCommand CommandsFor<TCommand>(EndpointId endpoint) where TCommand : ICommandSet
        {
            // Check that we have a connection to the endpoint. 
            //   if not then error out.
            //
            // Check that the endpoint supports the given command set
            //   if not then error out
            //
            // Check that we have the given command for the endpoint
            //   If not then get the builder to create it
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers the given commandset with the current hub.
        /// </summary>
        /// <param name="commands">The commands that should be registered.</param>
        public void RegisterCommands(ICommandSet commands)
        {
            // Store the new commandset
            // Advertise that we have a new set(?)
            // Somehow link the set up with the communication layer?
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes all connections.
        /// </summary>
        public void CloseConnections()
        {
            throw new NotImplementedException();
        }
    }
}
