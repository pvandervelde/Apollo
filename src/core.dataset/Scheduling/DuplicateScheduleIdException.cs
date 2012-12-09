//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.Core.Dataset.Properties;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// An exception thrown when the user tries register a schedule with an existing ID.
    /// </summary>
    [Serializable]
    public sealed class DuplicateScheduleIdException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateScheduleIdException"/> class.
        /// </summary>
        public DuplicateScheduleIdException()
            : this(Resources.Exceptions_Messages_DuplicateScheduleId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateScheduleIdException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DuplicateScheduleIdException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateScheduleIdException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DuplicateScheduleIdException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateScheduleIdException"/> class.
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
        private DuplicateScheduleIdException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
