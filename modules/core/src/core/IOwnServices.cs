// Copyright (c) P. van der Velde. All rights reserved.

using System;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the interface for objects which control kernel services.
    /// </summary>
    public interface IOwnServices
    {
        /// <summary>
        /// Installs the service of the specified type.
        /// </summary>
        /// <remarks>
        ///     Only services that are 'installed' can be used by the service manager.
        ///     Services that have not been installed are simply unknown to the service
        ///     manager.
        /// </remarks>
        /// <param name="service">
        ///     The service which should be installed.
        /// </param>
        void Install(Type service);

        /// <summary>
        /// Uninstalls the service of the specified type.
        /// </summary>
        /// <remarks>
        ///     Once a service is uninstalled it can no longer be started. It is effectively
        ///     removed from the list of known services.
        /// </remarks>
        /// <param name="service">
        ///     The type of service that needs to be uninstalled.
        /// </param>
        void Uninstall(Type service);

        /// <summary>
        /// Starts the a service of the specified type.
        /// </summary>
        /// <param name="service">
        ///     The type of the service that needs to be started.
        /// </param>
        void Start(Type service);

        /// <summary>
        /// Stops the specified service.
        /// </summary>
        /// <param name="service">
        ///     The service which should be stopped.
        /// </param>
        void Stop(KernelService service);

        /// <summary>
        /// Restarts the specified service and returns a link to the newly started service.
        /// </summary>
        /// <param name="service">
        ///     The service which should be restarted.
        /// </param>
        /// <returns>
        ///     A new instance of the service.
        /// </returns>
        KernelService Restart(KernelService service);
    }
}
