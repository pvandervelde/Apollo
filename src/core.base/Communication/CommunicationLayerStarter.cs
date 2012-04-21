//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines a initialization method for starting the communication layer when the application
    /// starts.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class CommunicationLayerStarter : Autofac.IStartable
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
        /// Perform once-off startup processing.
        /// </summary>
        public void Start()
        {
            // Starting the communication layer takes quite a while
            // so lets not block the current thread which is being used
            // to start the application.
            Task.Factory.StartNew(() => m_Layer.SignIn());
        }
    }
}
