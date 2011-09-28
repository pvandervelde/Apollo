//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Host
{
    /// <summary>
    /// Defines the interface for objects which control kernel services.
    /// </summary>
    internal interface IOwnServices
    {
        /// <summary>
        /// Installs the specified service.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     Only services that are 'installed' can be used by the service manager.
        ///     Services that have not been installed are simply unknown to the service
        ///     manager.
        /// </para>
        /// <para>
        ///     Note that only one instance for each <c>Type</c> can be provided to
        ///     the service manager.
        /// </para>
        /// </remarks>
        /// <param name="service">
        ///     The service which should be installed.
        /// </param>
        void Install(KernelService service);

        /// <summary>
        /// Uninstalls the specified service.
        /// </summary>
        /// <remarks>
        ///     Once a service is uninstalled it can no longer be started. It is effectively
        ///     removed from the list of known services.
        /// </remarks>
        /// <param name="service">
        ///     The type of service that needs to be uninstalled.
        /// </param>
        void Uninstall(KernelService service);
    }
}
