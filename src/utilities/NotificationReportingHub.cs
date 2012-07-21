//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// Distributes notifications across the application.
    /// </summary>
    public sealed class NotificationReportingHub : ICollectNotifications
    {
        /// <summary>
        /// Stores the notification.
        /// </summary>
        /// <param name="notification">The notification text.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notification"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="notification"/> is an empty string.
        /// </exception>
        public void StoreNotification(string notification)
        {
            {
                Lokad.Enforce.Argument(() => notification);
                Lokad.Enforce.Argument(() => notification, Lokad.Rules.StringIs.NotEmpty);
            }

            RaiseOnNotification(notification);
        }

        /// <summary>
        /// Indicates that a notification has been received.
        /// </summary>
        public event EventHandler<NotificationEventArgs> OnNotification;

        private void RaiseOnNotification(string notification)
        {
            var local = OnNotification;
            if (local != null)
            {
                local(this, new NotificationEventArgs(notification));
            }
        }
    }
}
