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
        /// <param name="serviceDomain">The <see cref="AppDomain"/> in which the service resides.</param>
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
        void Install(KernelService service, AppDomain serviceDomain);

        /// <summary>
        /// Uninstalls the specified service.
        /// </summary>
        /// <remarks>
        ///     Once a service is uninstalled it can no longer be started. It is effectively
        ///     removed from the list of known services.
        /// </remarks>
        /// <param name="service">
        ///     The service that needs to be uninstalled.
        /// </param>
        /// <param name="shouldUnloadDomain">
        /// Indicates if the <c>AppDomain</c> that held the service should be unloaded or not.
        /// </param>
        void Uninstall(KernelService service, bool shouldUnloadDomain);

        /// <summary>
        /// Determines whether the application can shutdown cleanly.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if the application can shutdown cleanly; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool CanShutdown();

        /// <summary>
        /// Shuts the application down.
        /// </summary>
        void Shutdown();
    }
}
