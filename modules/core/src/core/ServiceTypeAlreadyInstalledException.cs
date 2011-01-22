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
    /// An exception thrown by the <see cref="Kernel"/> when the user tries to install a 
    /// <see cref="KernelService"/> of a type that is already installed.
    /// </summary>
    [Serializable]
    public sealed class ServiceTypeAlreadyInstalledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTypeAlreadyInstalledException"/> class.
        /// </summary>
        public ServiceTypeAlreadyInstalledException() 
            : this(Resources_NonTranslatable.Exception_Messages_ServiceTypeAlreadyInstalled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTypeAlreadyInstalledException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServiceTypeAlreadyInstalledException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTypeAlreadyInstalledException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceTypeAlreadyInstalledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTypeAlreadyInstalledException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private ServiceTypeAlreadyInstalledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
