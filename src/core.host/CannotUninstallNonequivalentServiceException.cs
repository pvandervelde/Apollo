//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;

namespace Apollo.Core.Host
{
    /// <summary>
    /// An exception thrown when the user tries to uninstall a service of a specific type but the matching service
    /// is a different object.
    /// </summary>
    [Serializable]
    public sealed class CannotUninstallNonequivalentServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotUninstallNonequivalentServiceException"/> class.
        /// </summary>
        public CannotUninstallNonequivalentServiceException() 
            : this(Resources.Exceptions_Messages_CannotUninstallNonEquivalentService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotUninstallNonequivalentServiceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CannotUninstallNonequivalentServiceException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotUninstallNonequivalentServiceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CannotUninstallNonequivalentServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotUninstallNonequivalentServiceException"/> class.
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
        private CannotUninstallNonequivalentServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
