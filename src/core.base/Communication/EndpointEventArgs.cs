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
    /// Defines an <see cref="EventArgs"/> class that provides the ID number of an endpoint.
    /// </summary>
    public sealed class EndpointEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        public EndpointEventArgs(EndpointId endpoint)
        {
            {
                Enforce.Argument(() => endpoint);
            }

            Endpoint = endpoint;
        }

        /// <summary>
        /// Gets a value indicating the ID of the affected endpoint.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }
    }
}
