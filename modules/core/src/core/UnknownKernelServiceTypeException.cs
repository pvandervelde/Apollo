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
    /// An exception thrown when the <see cref="Kernel"/> is asked to uninstall a
    /// <see cref="KernelService"/> that has not been installed.
    /// </summary>
    [ExcludeFromCoverage("Exceptions do not need to be tested")]
    [Serializable]
    public sealed class UnknownKernelServiceTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownKernelServiceTypeException"/> class.
        /// </summary>
        public UnknownKernelServiceTypeException() 
            : this(Resources_NonTranslatable.Exceptions_Messages_UnknownKernelServiceType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownKernelServiceTypeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnknownKernelServiceTypeException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownKernelServiceTypeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownKernelServiceTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownKernelServiceTypeException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private UnknownKernelServiceTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
