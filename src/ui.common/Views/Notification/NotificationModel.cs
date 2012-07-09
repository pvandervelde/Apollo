//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities;

namespace Apollo.UI.Common.Views.Notification
{
    /// <summary>
    /// Defines the model for the notification reporting in the application.
    /// </summary>
    public sealed class NotificationModel : Model
    {
        /// <summary>
        /// The object that collects the notifications.
        /// </summary>
        private readonly ICollectNotifications m_Notifications;

        /// <summary>
        /// The notification text.
        /// </summary>
        private string m_NotificationText;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="notificationCollector">The object that collects notifications from all over the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationCollector"/> is <see langword="null" />.
        /// </exception>
        public NotificationModel(IContextAware context, ICollectNotifications notificationCollector)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => notificationCollector);
            }

            m_Notifications = notificationCollector;
            m_Notifications.OnNotification += (s, e) => Notification = e.Notification;
        }

        /// <summary>
        /// Gets the notification text.
        /// </summary>
        public string Notification
        {
            get
            {
                return m_NotificationText;
            }

            private set
            {
                m_NotificationText = value;
                Notify(() => Notification);
            }
        }
    }
}
