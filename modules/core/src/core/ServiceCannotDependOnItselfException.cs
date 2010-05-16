//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.Core.Properties;
using Apollo.Utils;

namespace Apollo.Core
{
    /// <summary>
    /// An exception thrown when a <see cref="KernelService"/> is installed while depending on itself.
    /// </summary>
    [ExcludeFromCoverage("Exceptions do not need to be tested")]
    [Serializable]
    public sealed class ServiceCannotDependOnItselfException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCannotDependOnItselfException"/> class.
        /// </summary>
        public ServiceCannotDependOnItselfException() 
            : this(Resources_NonTranslatable.Exceptions_Messages_ServiceCannotDependOnItself)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCannotDependOnItselfException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServiceCannotDependOnItselfException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCannotDependOnItselfException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceCannotDependOnItselfException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCannotDependOnItselfException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private ServiceCannotDependOnItselfException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
