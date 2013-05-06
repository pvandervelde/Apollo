//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Host
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
        /// shutdown capability checks.
        /// </summary>
        /// <remarks>
        /// In order to indicate that the system is clear to shutdown an 
        /// <see cref="INotificationArguments"/> object is passed that allows
        /// the registering party to indicate if shutdown is allowed.
        /// </remarks>
        /// <value>The shutdown capability check.</value>
        NotificationName CanSystemShutdown
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
