//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Lokad;

namespace Apollo.Core.UserInterfaces.Application
{
    /// <summary>
    /// Stores information about the Apollo application and allows the user 
    /// to requests updates to this information.
    /// </summary>
    internal sealed class SystemInformation : IHoldSystemInformation
    {
        /// <summary>
        /// The collection that holds information about all the active services.
        /// </summary>
        private readonly List<IServiceInformation> m_ServiceInformation 
            = new List<IServiceInformation>();

        /// <summary>
        /// The function that returns the current time.
        /// </summary>
        private readonly Func<DateTimeOffset> m_CurrentTime;

        /// <summary>
        /// The function used to get the updated information from the 
        /// core.
        /// </summary>
        /// <returns></returns>
        private readonly Func<ISystemInformationStorage> m_Update;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemInformation"/> class.
        /// </summary>
        /// <param name="currentTime">The function used to get the current time.</param>
        /// <param name="update">The function used in order to get the updated information.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="currentTime"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="update"/> is <see langword="null" />.
        /// </exception>
        public SystemInformation(Func<DateTimeOffset> currentTime, Func<ISystemInformationStorage> update)
        {
            {
                Enforce.Argument(() => currentTime);
                Enforce.Argument(() => update);
            }

            m_CurrentTime = currentTime;
            m_Update = update;
            
            Refresh();
        }

        /// <summary>
        /// Refreshes the cached information.
        /// </summary>
        public void Refresh()
        {
            var updateResult = m_Update();
            LastUpdateTime = m_CurrentTime();
            CoreVersion = updateResult.CoreVersion;
            StartupTime = updateResult.StartupTime;
        }

        /// <summary>
        /// Gets the time the data was last updated.
        /// </summary>
        /// <value>The last update time.</value>
        public DateTimeOffset LastUpdateTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the time the kernel was started.
        /// </summary>
        /// <value>The startup time for the kernel.</value>
        public DateTimeOffset StartupTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the kernel uptime.
        /// </summary>
        /// <value>The kernel uptime.</value>
        public TimeSpan Uptime
        {
            get
            {
                return m_CurrentTime() - StartupTime;
            }
        }

        /// <summary>
        /// Gets the version number of the core of Apollo.
        /// </summary>
        public Version CoreVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the collection containing information about the 
        /// available services.
        /// </summary>
        /// <returns>
        /// A collection containing information about the available services.
        /// </returns>
        public IEnumerable<IServiceInformation> ServiceInformation()
        {
            return m_ServiceInformation;
        }
    }
}
