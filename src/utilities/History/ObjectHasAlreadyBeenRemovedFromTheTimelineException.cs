//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// An exception that is thrown when the user tries to delete an object from the timeline 
    /// when that object already has a deletion time.
    /// </summary>
    [Serializable]
    public sealed class ObjectHasAlreadyBeenRemovedFromTheTimelineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectHasAlreadyBeenRemovedFromTheTimelineException"/> class.
        /// </summary>
        public ObjectHasAlreadyBeenRemovedFromTheTimelineException() 
            : this(Resources.Exceptions_Messages_ObjectHasAlreadyBeenRemoved)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectHasAlreadyBeenRemovedFromTheTimelineException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ObjectHasAlreadyBeenRemovedFromTheTimelineException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectHasAlreadyBeenRemovedFromTheTimelineException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ObjectHasAlreadyBeenRemovedFromTheTimelineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectHasAlreadyBeenRemovedFromTheTimelineException"/> class.
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
        private ObjectHasAlreadyBeenRemovedFromTheTimelineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
