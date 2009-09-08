//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the service that is used by the <c>Kernel</c> to 
    /// interact with the other services on an equal basis.
    /// </summary>
    /// <remarks>
    /// The <c>CoreProxy</c> is automatically loaded by the <see cref="Kernel"/>
    /// so there is no need to request the bootstrapper to load it.
    /// </remarks>
    [AutoLoad]
    internal sealed class CoreProxy : KernelService, IHaveServiceDependencies
    {
        /// <summary>
        /// Gets the type of the service. Currently either a background service
        /// or a foreground service.
        /// </summary>
        /// <returns>The type of the service.</returns>
        public override ServiceType ServicePreferenceType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a set of types indicating which services need to be present
        /// for the current service to be functional.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of 
        ///     services which this service requires to be functional.
        /// </returns>
        public IEnumerable<Type> ServicesToBeAvailable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a set of types indicating which services the current service
        /// needs to be linked to in order to be functional.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{Type}"/> which contains the types of services
        /// on which this service depends.
        /// </returns>
        public IEnumerable<Type> ServicesToConnectTo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        public void ConnectTo(KernelService dependency)
        {
            throw new NotImplementedException();
        }
    }
}
