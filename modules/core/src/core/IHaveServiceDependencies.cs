// Copyright (c) P. van der Velde. All rights reserved.

using System;
using System.Collections.Generic;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the interface for <see cref="KernelService"/> objects that have
    /// dependencies on other services.
    /// </summary>
    public interface IHaveServiceDependencies
    {
        /// <summary>
        /// Returns a set of types indicating on which services the current service
        /// depends.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of services
        ///     on which this service depends.
        /// </returns>
        IEnumerable<Type> Dependencies();
        
        /// <summary>
        /// Provides one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        void ConnectToDependency(KernelService dependency);
    }
}
