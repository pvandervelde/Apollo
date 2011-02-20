//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Builds proxy objects for the <see cref="MessageHub"/>.
    /// </summary>
    internal sealed class CommandProxyBuilder
    {
        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The interface of the commandset for which a proxy must be made.</typeparam>
        /// <param name="endpoint">The endpoint for which a proxy must be made.</param>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public T ProxyConnectingTo<T>(EndpointId endpoint) where T : ICommandSet
        {
            object result = ProxyConnectingTo(typeof(T), endpoint);
            return (T)result;
        }

        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <param name="interfaceType">The interface of the commandset for which a proxy must be made.</param>
        /// <param name="endpoint">The endpoint for which a proxy must be made.</param>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public object ProxyConnectingTo(Type interfaceType, EndpointId endpoint)
        {
            // No properties
            // No events other than the ones on ICommandSet

            // Check for each command method:
            // - Return value is one of void / Task / IObservable
            // - All parameters are serializable
            throw new NotImplementedException();
        }
    }
}
