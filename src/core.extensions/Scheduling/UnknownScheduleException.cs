//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.Core.Extensions.Properties;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// An exception thrown when the user tries to find a schedule with an unknown ID.
    /// </summary>
    [Serializable]
    public sealed class UnknownScheduleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownScheduleException"/> class.
        /// </summary>
        public UnknownScheduleException()
            : this(Resources.Exceptions_Messages_UnknownSchedule)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownScheduleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnknownScheduleException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownScheduleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownScheduleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownScheduleException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information
        ///     about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private UnknownScheduleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
