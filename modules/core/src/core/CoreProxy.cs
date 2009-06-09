// Copyright (c) P. van der Velde. All rights reserved.

using System;
using System.Collections.Generic;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the service that is used by the <c>Kernel</c> to 
    /// interact with the other services on an equal basis.
    /// </summary>
    internal sealed class CoreProxy : KernelService, IHaveServiceDependencies
    {
        /// <summary>
        /// Gets the type of the service. Currently either a background service
        /// or a foreground service.
        /// </summary>
        /// <returns></returns>
        /// <value>The type of the service.</value>
        public override ServiceType ServiceType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a set of types indicating on which services the current service
        /// depends.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{Type}"/> which contains the types of services
        /// on which this service depends.
        /// </returns>
        public IEnumerable<Type> Dependencies()
        {
            // Depends on the:
            // - Persistence service
            // - Message pipeline

            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        public void ConnectToDependency(KernelService dependency)
        {
            throw new NotImplementedException();
        }
    }
}
