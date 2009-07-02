//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the core class that controls the kernel of the Apollo application.
    /// </summary>
    internal sealed class Kernel : IOwnServices
    {
        // How do we discover services? MEF?

        /// <summary>
        /// Installs the service of the specified type.
        /// </summary>
        /// <param name="service">The service which should be installed.</param>
        /// <remarks>
        /// Only services that are 'installed' can be used by the service manager.
        /// Services that have not been installed are simply unknown to the service
        /// manager.
        /// </remarks>
        public void Install(Type service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Uninstalls the service of the specified type.
        /// </summary>
        /// <param name="service">The type of service that needs to be uninstalled.</param>
        /// <remarks>
        /// Once a service is uninstalled it can no longer be started. It is effectively
        /// removed from the list of known services.
        /// </remarks>
        public void Uninstall(Type service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts the a service of the specified type.
        /// </summary>
        /// <param name="service">The type of the service that needs to be started.</param>
        public void Start(Type service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the specified service.
        /// </summary>
        /// <param name="service">The service which should be stopped.</param>
        public void Stop(KernelService service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Restarts the specified service and returns a link to the newly started service.
        /// </summary>
        /// <param name="service">The service which should be restarted.</param>
        /// <returns>A new instance of the service.</returns>
        public KernelService Restart(KernelService service)
        {
            throw new NotImplementedException();
        }
    }
}
