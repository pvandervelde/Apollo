//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.Core.Dataset.Properties;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// An exception thrown when the user does not provide the correct command line arguments.
    /// </summary>
    [Serializable]
    public sealed class InvalidCommandLineArgumentsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandLineArgumentsException"/> class.
        /// </summary>
        public InvalidCommandLineArgumentsException()
            : this(Resources.Exceptions_Messages_InvalidCommandLineArguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandLineArgumentsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidCommandLineArgumentsException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandLineArgumentsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidCommandLineArgumentsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandLineArgumentsException"/> class.
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
        private InvalidCommandLineArgumentsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
