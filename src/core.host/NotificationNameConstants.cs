//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Stores the <see cref="NotificationName"/> objects in use by the core.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class NotificationNameConstants : INotificationNameConstants
    {
        /// <summary>
        /// The <see cref="NotificationName"/> used for start-up complete notifications.
        /// </summary>
        private readonly NotificationName m_StartupComplete = new NotificationName("StartupComplete");

        /// <summary>
        /// The <see cref="NotificationName"/> used for shut down capability requests.
        /// </summary>
        private readonly NotificationName m_CanSystemShutdown = new NotificationName("CanSystemShutdown");

        /// <summary>
        /// The <see cref="NotificationName"/> used for shut down notifications.
        /// </summary>
        private readonly NotificationName m_SystemShuttingDown = new NotificationName("SystemShuttingDown");

        /// <summary>
        /// Gets the <see cref="NotificationName"/> that is used
        /// for the notification upon the completion of startup.
        /// </summary>
        /// <value>The start-up complete notification.</value>
        public NotificationName StartupComplete
        {
            get
            {
                return m_StartupComplete;
            }
        }

        /// <summary>
        /// Gets the <see cref="NotificationName"/> that is used for
        /// shut down capability checks.
        /// </summary>
        /// <value>The shutdown capability check.</value>
        public NotificationName CanSystemShutdown
        {
            get
            {
                return m_CanSystemShutdown;
            }
        }

        /// <summary>
        /// Gets the <see cref="NotificationName"/> that is used for
        /// shut down notifications.
        /// </summary>
        /// <value>The shutdown notification.</value>
        public NotificationName SystemShuttingDown
        {
            get
            {
                return m_SystemShuttingDown;
            }
        }
    }
}
