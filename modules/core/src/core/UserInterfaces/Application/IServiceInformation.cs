//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.UserInterfaces.Application
{
    /// <summary>
    /// Defines the interface for objects that store information
    /// about a specific service.
    /// </summary>
    public interface IServiceInformation
    {
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        string Name
        {
            get; 
        }

        // - service status (not started, started, failed, stopped)
        // - Missing dependencies

        /// <summary>
        /// Gets the time the service was started.
        /// </summary>
        /// <value>The startup time for the service.</value>
        DateTimeOffset StartupTime
        {
            get;
        }

        /// <summary>
        /// Gets the uptime for the service.
        /// </summary>
        /// <value>The service uptime.</value>
        TimeSpan Uptime
        {
            get;
        }
    }
}
