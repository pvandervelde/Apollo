//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the interface for objects that provide kernel services.
    /// </summary>
    internal interface IKernel
    {
        /// <summary>
        /// Installs the specified service.
        /// </summary>
        /// <param name="service">The service which should be installed.</param>
        /// <remarks>
        /// <para>
        /// Only services that are 'installed' can be used by the service manager.
        /// Services that have not been installed are simply unknown to the service
        /// manager.
        /// </para>
        /// <para>
        /// Note that only one instance for each <c>Type</c> can be provided to
        /// the service manager.
        /// </para>
        /// </remarks>
        void Install(KernelService service);

        /// <summary>
        /// Initialized the kernel by allowing all the kernel services to 
        /// go through their initialization processes.
        /// </summary>
        /// <design>
        /// <para>
        /// This method ensures that all the services are operational and
        /// have been started. Once all the services are capable of
        /// running then we return focus to the object that started the 
        /// kernel.
        /// </para>
        /// <para>
        /// The <c>Kernel.Start</c> method does not specifically start a
        /// new thread for the services to run on (although individual
        /// services may start new threads). This is done specifically because
        /// eventually the focus needs to return to the UI thread. After all
        /// this is where the user interaction will take place on. Thus there
        /// is no reason for the kernel to have it's own thread.
        /// </para>
        /// </design>
        void Start();

        /// <summary>
        /// Shuts the application down.
        /// </summary>
        void Shutdown();
    }
}
