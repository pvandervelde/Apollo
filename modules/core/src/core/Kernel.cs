//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the core class that controls the kernel of the Apollo application.
    /// </summary>
    /// <design>
    /// The kernel will load the services in the following order:
    /// <list type="bullet">
    /// <item>Message service</item>
    /// <item>Core</item>
    /// <item>User interface service</item>
    /// <item>License service</item>
    /// <item>Persistence service</item>
    /// <item>Log sink</item>
    /// <item>Timeline service</item>
    /// <item>Plug-in service</item>
    /// <item>Project service</item>
    /// </list>
    /// </design>
    internal sealed class Kernel : INeedStartup
    {
        // Can we use Autofac as our container?
        // Probably

        /// <summary>
        /// The event that is fired when there is an update in the startup process.
        /// </summary>
        public event EventHandler<StartupProgressEventArgs> StartupProgress;

        /// <summary>
        /// Starts the startup process.
        /// </summary>
        public void Start()
        {
            // During start up we track the startup state of the different services
            // and create our own startup state based on that?
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a value indicating what the state of the object is regarding
        /// the startup process.
        /// </summary>
        /// <returns>
        ///   The current startup state for the object.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "See: Framework Design Guidelines; Section 5.1, page 136.")]
        public StartupState GetStartupState()
        {
            throw new NotImplementedException();
        }

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
        public void Install(KernelService service)
        {
            throw new NotImplementedException();
        }

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
        public void Uninstall(KernelService service)
        {
            throw new NotImplementedException();
        }
    }
}
