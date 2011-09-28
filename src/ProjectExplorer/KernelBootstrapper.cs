//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Apollo.Core.Base;
using Apollo.Core.Host;
using Apollo.Utilities;
using Autofac.Core;
using Lokad;

namespace Apollo.ProjectExplorer
{
    /// <summary>
    /// Defines the bootstrapper which will initialize the kernel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this bootstrapper only takes care of the bootstrapping
    /// of the core, not the UI. By design the core and the UI are 
    /// running with different IOC containers / bootstrappers. This means
    /// that we can force a code separation because the UI controls cannot
    /// get linked to any of the internal core elements. The only way for
    /// the core and the UI to interact is via the UserInterfaceService.
    /// </para>
    /// </remarks>
    [ExcludeFromCodeCoverage]
    internal sealed class KernelBootstrapper : Bootstrapper
    {
        /// <summary>
        /// The function that allows storing a DI container.
        /// </summary>
        private readonly Action<IModule> m_ContainerStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBootstrapper"/> class.
        /// </summary>
        /// <param name="progress">The object used to track the progress of the bootstrapping process.</param>
        /// <param name="shutdownEvent">The event that signals to the application that it is safe to shut down.</param>
        /// <param name="containerStorage">The function used to store the DI container which holds the kernel UI references.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="progress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="shutdownEvent"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="containerStorage"/> is <see langword="null"/>.
        /// </exception>
        public KernelBootstrapper(
            ITrackProgress progress,
            AutoResetEvent shutdownEvent,
            Action<IModule> containerStorage)
            : base(progress, shutdownEvent)
        {
            {
                Enforce.Argument(() => containerStorage);
            }

            m_ContainerStorage = containerStorage;
        }

        #region Overrides of Bootstrapper

        /// <summary>
        /// Returns a collection containing additional IOC modules that are
        /// required to start the core.
        /// </summary>
        /// <returns>
        ///     The collection containing additional IOC modules necessary
        ///     to start the core.
        /// </returns>
        protected override IEnumerable<IModule> AdditionalCoreModules()
        {
            return new List<IModule> 
                { 
                    new UtilitiesModule(),
                    new BaseModule(true),
                    new BaseModuleForLoaders(),
                    new BaseModuleForHosts(),
                };
        }

        /// <summary>
        /// Stores the dependency injection container.
        /// </summary>
        /// <param name="container">The DI container.</param>
        protected override void StoreContainer(IModule container)
        {
            m_ContainerStorage(container);
        }

        #endregion
    }
}
