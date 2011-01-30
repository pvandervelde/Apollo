//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the interface for <see cref="KernelService"/> objects that have
    /// dependencies on other services.
    /// </summary>
    /// <design>
    /// Once a service is connected to another service it cannot be disconnected
    /// anymore. In order to disconnect a service both services need to be stopped 
    /// and the depending service object needs to be destroyed.
    /// </design>
    internal interface IHaveServiceDependencies
    {
        /// <summary>
        /// Returns a set of types indicating which services the current service
        /// needs to be linked to in order to be functional.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of services
        ///     on which this service depends.
        /// </returns>
        IEnumerable<Type> ServicesToConnectTo();
        
        /// <summary>
        /// Provides one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        void ConnectTo(KernelService dependency);

        /// <summary>
        /// Gets a value indicating whether this instance is connected to all dependencies.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this instance is connected to all dependencies; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
          Justification = "Documentation can start with a language keyword")]
        bool IsConnectedToAllDependencies
        {
            get;
        }
    }
}
