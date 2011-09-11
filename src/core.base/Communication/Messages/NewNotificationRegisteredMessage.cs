//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that contains information about a newly registered notification.
    /// </summary>
    [Serializable]
    internal sealed class NewNotificationRegisteredMessage : CommunicationMessage
    {
        /// <summary>
        /// The newly registered notification.
        /// </summary>
        private readonly ISerializedType m_Notification;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewNotificationRegisteredMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="notificationType">
        ///     The newly available <see cref="INotificationSet"/> interface.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationType"/> is <see langword="null" />.
        /// </exception>
        public NewNotificationRegisteredMessage(EndpointId origin, Type notificationType)
            : base(origin)
        {
            {
                Enforce.Argument(() => notificationType);
            }

            m_Notification = ProxyExtensions.FromType(notificationType);
        }

        /// <summary>
        /// Gets the newly registered notification.
        /// </summary>
        public ISerializedType Notification
        {
            get
            {
                return m_Notification;
            }
        }
    }
}
