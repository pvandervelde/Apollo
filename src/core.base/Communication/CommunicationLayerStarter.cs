//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines a initialization method for starting the communication layer when the application
    /// starts.
    /// </summary>
    internal sealed class CommunicationLayerStarter : ILoadOnApplicationStartup, IDisposable
    {
        /// <summary>
        /// The object that handles remote commands.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_Hub;

        /// <summary>
        /// The communication layer for the application.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationLayerStarter"/> class.
        /// </summary>
        /// <param name="hub">The object that sends commands to the remote endpoints.</param>
        /// <param name="layer">The communication layer for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="hub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        public CommunicationLayerStarter(ISendCommandsToRemoteEndpoints hub, ICommunicationLayer layer)
        {
            {
                Enforce.Argument(() => hub);
                Enforce.Argument(() => layer);
            }

            // We need a reference to the hub so that it may be alive 
            // before we sign in on the layer. That way the hub gets
            // all the new endpoints etc.
            m_Hub = hub;
            m_Layer = layer;
        }

        /// <summary>
        /// Starts the validation of the licenses.
        /// </summary>
        public void Initialize()
        {
            m_Layer.SignIn();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_Hub.CloseConnections();
            m_Layer.SignOut();
        }
    }
}
