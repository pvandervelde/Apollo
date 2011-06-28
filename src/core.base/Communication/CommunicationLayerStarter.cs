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
        /// The communication layer for the application.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationLayerStarter"/> class.
        /// </summary>
        /// <param name="layer">The communication layer for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        public CommunicationLayerStarter(ICommunicationLayer layer)
        {
            {
                Enforce.Argument(() => layer);
            }

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
            m_Layer.SignOut();
        }
    }
}
