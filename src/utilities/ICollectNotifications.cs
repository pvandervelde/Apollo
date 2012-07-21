//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for objects that collect notifications.
    /// </summary>
    public interface ICollectNotifications
    {
        /// <summary>
        /// Stores the notification.
        /// </summary>
        /// <param name="notification">The notification text.</param>
        void StoreNotification(string notification);

        /// <summary>
        /// Indicates that a notification has been received.
        /// </summary>
        event EventHandler<NotificationEventArgs> OnNotification;
    }
}
