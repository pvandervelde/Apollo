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
        /// shut down capability checks.
        /// </summary>
        /// <remarks>
        /// In order to indicate that the system is clear to shut down an 
        /// <see cref="INotificationArguments"/> object is passed that allows
        /// the registring party to indicate if shut down is allowed.
        /// </remarks>
        /// <value>The shutdown capability check.</value>
        NotificationName CanSystemShutDown
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
