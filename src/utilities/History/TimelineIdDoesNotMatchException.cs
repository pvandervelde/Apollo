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
    /// An exception thrown when the provided <see cref="IAmHistoryEnabled"/> object has an ID that does not
    /// match with the expected ID.
    /// </summary>
    [Serializable]
    public sealed class TimelineIdDoesNotMatchException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineIdDoesNotMatchException"/> class.
        /// </summary>
        public TimelineIdDoesNotMatchException() 
            : this(Resources.Exceptions_Messages_HistoryIdDoesNotMatch)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineIdDoesNotMatchException"/> class.
        /// </summary>
        /// <param name="provided">The ID number that was provided.</param>
        /// <param name="expected">The ID number that was expected.</param>
        public TimelineIdDoesNotMatchException(HistoryId provided, HistoryId expected)
            : this(
                string.Format(
                    CultureInfo.InvariantCulture, 
                    Resources.Exceptions_Messages_HistoryIdDoesNotMatch_WithIds,
                    provided,
                    expected))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineIdDoesNotMatchException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TimelineIdDoesNotMatchException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineIdDoesNotMatchException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TimelineIdDoesNotMatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineIdDoesNotMatchException"/> class.
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
        private TimelineIdDoesNotMatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
