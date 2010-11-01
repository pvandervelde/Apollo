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
    /// An exception thrown when the user tries to store a <see cref="ICommand"/> 
    /// with a <see cref="CommandId"/> that is identical to an already stored command.
    /// </summary>
    [Serializable]
    public sealed class DuplicateCommandException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateCommandException"/> class.
        /// </summary>
        public DuplicateCommandException() 
            : this(Resources.Exceptions_Messages_DuplicateCommand)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateCommandException"/> class.
        /// </summary>
        /// <param name="id">The <c>DnsName</c> which was a duplicate.</param>
        public DuplicateCommandException(CommandId id)
            : this(string.Format(CultureInfo.InvariantCulture, Resources.Exceptions_Messages_DuplicateCommand_WithName, id))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateCommandException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DuplicateCommandException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateCommandException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DuplicateCommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateCommandException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private DuplicateCommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
