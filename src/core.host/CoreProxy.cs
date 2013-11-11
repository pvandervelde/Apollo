//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Lokad;
using Nuclei.Diagnostics;

namespace Apollo.Core.Host
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
    internal sealed class CoreProxy : KernelService
    {
        /// <summary>
        /// The <see cref="Kernel"/> that owns this proxy.
        /// </summary>
        private readonly IKernel m_Owner;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreProxy"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Kernel"/> to which this proxy is linked.</param>
        /// <param name="diagnostics">The object that handles the diagnostics for the application.</param>
        /// <param name="scheduler">The scheduler that is used to run tasks.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="owner"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="diagnostics"/> is <see langword="null"/>.
        /// </exception>
        public CoreProxy(IKernel owner, SystemDiagnostics diagnostics, TaskScheduler scheduler = null)
            : base(diagnostics)
        {
            {
                Enforce.Argument(() => owner);
            }

            m_Owner = owner;
            m_Scheduler = scheduler ?? TaskScheduler.Default;
        }

        /// <summary>
        /// Shuts the kernel and the application down.
        /// </summary>
        /// <returns>The task that is running the shutdown of the system.</returns>
        public Task Shutdown()
        {
            return Task.Factory.StartNew(
                () => m_Owner.Shutdown(),
                new CancellationToken(),
                TaskCreationOptions.None,
                m_Scheduler);
        }

        /// <summary>
        /// Notifies all listeners that application startup has finished.
        /// </summary>
        internal void NotifyServicesOfStartupCompletion()
        {
            RaiseOnStartupComplete();
        }

        /// <summary>
        /// An event raised when the application startup process has finished.
        /// </summary>
        public event EventHandler<ApplicationStartupEventArgs> OnStartupComplete;

        private void RaiseOnStartupComplete()
        {
            var local = OnStartupComplete;
            if (local != null)
            {
                local(this, new ApplicationStartupEventArgs());
            }
        }
    }
}
