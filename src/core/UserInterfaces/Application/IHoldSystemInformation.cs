// -----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.UserInterfaces.Application
{
    /// <summary>
    /// Defines the interface for objects that hold information about
    /// the system.
    /// </summary>
    public interface IHoldSystemInformation
    {
        /// <summary>
        /// Refreshes the cached information.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Gets the time the data was last updated.
        /// </summary>
        /// <value>The last update time.</value>
        DateTimeOffset LastUpdateTime
        {
            get;
        }

        /// <summary>
        /// Gets the time the kernel was started.
        /// </summary>
        /// <value>The startup time for the kernel.</value>
        DateTimeOffset StartupTime
        {
            get;
        }

        /// <summary>
        /// Gets the kernel uptime.
        /// </summary>
        /// <value>The kernel uptime.</value>
        TimeSpan Uptime 
        {
            get; 
        }

        /// <summary>
        /// Gets the version number of the core of Apollo.
        /// </summary>
        Version CoreVersion
        {
            get;
        }

        /// <summary>
        /// Returns the collection containing information about the 
        /// available services.
        /// </summary>
        /// <returns>
        /// A collection containing information about the available services.
        /// </returns>
        IEnumerable<IServiceInformation> ServiceInformation();
    }
}
