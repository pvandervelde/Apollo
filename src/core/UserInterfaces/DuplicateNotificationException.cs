﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Apollo.Core.Properties;
using Apollo.Utilities;

namespace Apollo.Core.UserInterfaces
{
    /// <summary>
    /// An exception thrown if a user tries to register a notification with a
    /// <see cref="NotificationName"/> that is already registered.
    /// </summary>
    [Serializable]
    public sealed class DuplicateNotificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateNotificationException"/> class.
        /// </summary>
        public DuplicateNotificationException()
            : this(Resources_NonTranslatable.Exception_Messages_UnknownNotificationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateNotificationException"/> class.
        /// </summary>
        /// <param name="name">The <c>DnsName</c> which was a duplicate.</param>
        public DuplicateNotificationException(NotificationName name)
            : this(string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.Exception_Messages_UnknownNotificationName_WithName, name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateNotificationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DuplicateNotificationException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateNotificationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DuplicateNotificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateNotificationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private DuplicateNotificationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
