//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that is used to provide information about a notification.
    /// </summary>
    public sealed class NotificationEventArgs : EventArgs
    {
        /// <summary>
        /// The notification.
        /// </summary>
        private readonly string m_Notification;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public NotificationEventArgs(string notification)
        {
            m_Notification = notification;
        }

        /// <summary>
        /// Gets the notification text.
        /// </summary>
        public string Notification
        {
            get
            {
                return m_Notification;
            }
        }
    }
}
