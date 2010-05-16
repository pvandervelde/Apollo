//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Apollo.Utils.Properties;

namespace Apollo.Utils.Commands
{
    /// <summary>
    /// An exception thrown when the user requests a <see cref="ICommand" /> 
    /// with a specific <see cref="CommandId" /> which is not stored.
    /// </summary>
    [ExcludeFromCoverage("Exceptions do not need to be tested.")]
    [Serializable]
    public sealed class UnknownCommandException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        public UnknownCommandException() 
            : this(Resources.Exceptions_Messages_UnknownCommand)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="id">The <c>CommandId</c> which was unknown.</param>
        public UnknownCommandException(CommandId id)
            : this(string.Format(CultureInfo.InvariantCulture, Resources.Exceptions_Messages_UnknownCommand_WithName, id))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnknownCommandException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownCommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private UnknownCommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
