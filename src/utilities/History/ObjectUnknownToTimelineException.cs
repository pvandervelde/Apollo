//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// An exception thrown when the user tries to get an object from the timeline when that object
    /// doesn't exist in the timeline.
    /// </summary>
    [Serializable]
    public sealed class ObjectUnknownToTimelineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectUnknownToTimelineException"/> class.
        /// </summary>
        public ObjectUnknownToTimelineException()
            : this(Resources.Exceptions_Messages_ObjectUnknownToTimeline)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectUnknownToTimelineException"/> class.
        /// </summary>
        /// <param name="id">The history ID of the unknown object.</param>
        public ObjectUnknownToTimelineException(HistoryId id)
            : this(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Exceptions_Messages_ObjectUnknownToTimeline_WithId,
                    id))
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectUnknownToTimelineException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ObjectUnknownToTimelineException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectUnknownToTimelineException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ObjectUnknownToTimelineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectUnknownToTimelineException"/> class.
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
        private ObjectUnknownToTimelineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
