//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using Apollo.Core.Base;
using Apollo.Core.Host;
using Apollo.UI.Explorer.Nuclei;
using Autofac.Core;
using Lokad;
using Nuclei.Communication;
using Nuclei.Progress;

namespace Apollo.UI.Explorer
{
    /// <summary>
    /// Defines the bootstrapper which will initialize the kernel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this bootstrapper only takes care of the bootstrapping
    /// of the core, not the UI. By design the core and the UI are 
    /// running with different IOC containers / bootstrapper objects. This means
    /// that we can force a code separation because the UI controls cannot
    /// get linked to any of the internal core elements. The only way for
    /// the core and the UI to interact is via the UserInterfaceService.
    /// </para>
    /// </remarks>
    internal sealed class KernelBootstrapper : Bootstrapper
    {
        /// <summary>
        /// The function that will start the User Interface.
        /// </summary>
        private readonly Action<IModule> m_OnStartUserInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBootstrapper"/> class.
        /// </summary>
        /// <param name="progress">The object used to track the progress of the bootstrapping process.</param>
        /// <param name="shutdownEvent">The event that signals to the application that it is safe to shut down.</param>
        /// <param name="onStartUserInterface">The function used to store the DI container which holds the kernel UI references.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="progress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="shutdownEvent"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="onStartUserInterface"/> is <see langword="null"/>.
        /// </exception>
        public KernelBootstrapper(
            ITrackProgress progress,
            AutoResetEvent shutdownEvent,
            Action<IModule> onStartUserInterface)
            : base(progress, shutdownEvent)
        {
            {
                Enforce.Argument(() => onStartUserInterface);
            }

            m_OnStartUserInterface = onStartUserInterface;
        }

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
                    new CommunicationModule(
                        new List<CommunicationSubject>
                            {
                                CommunicationSubjects.Dataset,
                            }, 
                        true),
                    new BaseModuleForLoaders(),
                };
        }

        /// <summary>
        /// Stores the IOC module that contains the references that will be used by 
        /// the User Interface and then starts all the user interface elements.
        /// </summary>
        /// <param name="container">
        ///     The IOC module that contains the references for the User Interface.
        /// </param>
        protected override void StartUserInterface(IModule container)
        {
            m_OnStartUserInterface(container);
        }
    }
}
