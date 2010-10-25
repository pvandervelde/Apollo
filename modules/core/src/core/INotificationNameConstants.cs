//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core
{
    /// <summary>
    /// Defines the application level notification names.
    /// </summary>
    public interface INotificationNameConstants
    {
        /// <summary>
        /// Gets the <see cref="NotificationName"/> that is used
        /// for the notification upon the completion of startup.
        /// </summary>
        /// <value>The start-up complete notification.</value>
        NotificationName StartupComplete
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="NotificationName"/> that is used for
        /// shut down notifications.
        /// </summary>
        /// <value>The shutdown notification.</value>
        NotificationName SystemShuttingDown
        {
            get;
        }
    }
}
