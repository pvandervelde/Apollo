//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Apollo.ProjectExplorer.Properties;
using Apollo.Utilities;

namespace Apollo.ProjectExplorer
{
    /// <summary>
    /// An exception thrown when the user request the start-up time
    /// linked to a <see cref="IProgressMark"/> object which is 
    /// unknown.
    /// </summary>
    [Serializable]
    public sealed class UnknownProgressMarkException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownProgressMarkException"/> class.
        /// </summary>
        public UnknownProgressMarkException() 
            : this(Resources.Exceptions_Messages_UnknownProgressMark)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownProgressMarkException"/> class.
        /// </summary>
        /// <param name="mark">The unknown progress mark.</param>
        public UnknownProgressMarkException(IProgressMark mark)
            : this(string.Format(CultureInfo.InvariantCulture, Resources.Exceptions_Messages_UnknownProgressMark_WithMark, mark))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownProgressMarkException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnknownProgressMarkException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownProgressMarkException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownProgressMarkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownProgressMarkException"/> class.
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
        private UnknownProgressMarkException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
