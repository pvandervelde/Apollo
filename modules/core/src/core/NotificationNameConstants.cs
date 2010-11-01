//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utils;

namespace Apollo.Core
{
    /// <summary>
    /// Stores the <see cref="NotificationName"/> objects in use by the core.
    /// </summary>
    [ExcludeFromCoverage("This class only holds constant values. No need for unit testing.")]
    internal sealed class NotificationNameConstants : INotificationNameConstants
    {
        /// <summary>
        /// The <see cref="NotificationName"/> used for start-up complete notifications.
        /// </summary>
        private readonly NotificationName m_StartupComplete = new NotificationName("StartupComplete");

        /// <summary>
        /// The <see cref="NotificationName"/> used for shut down capability requests.
        /// </summary>
        private readonly NotificationName m_CanSystemShutDown = new NotificationName("CanSystemShutDown");

        /// <summary>
        /// The <see cref="NotificationName"/> used for shut down notifications.
        /// </summary>
        private readonly NotificationName m_SystemShuttingDown = new NotificationName("SystemShuttingDown");

        #region Implementation of INotificationNameConstants

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
        public NotificationName CanSystemShutDown
        {
            get
            {
                return m_CanSystemShutDown;
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

        #endregion
    }
}