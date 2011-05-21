﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Apollo.Utilities.Licensing
{
#if !DEPLOY

    /// <summary>
    /// An exception thrown when the the verification of the license fails.
    /// </summary>
    /// <design>
    /// This exception is not available in 'Deploy' mode because the use of 
    /// the exception would give away the locations where we check the
    /// license. However for testing purposes we want to throw an exception
    /// and not do random shutdowns.
    /// </design>
    [Serializable]
    public sealed class LicenseValidationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseValidationFailedException"/> class.
        /// </summary>
        public LicenseValidationFailedException() 
            : this(SrcOnlyResources.ExceptionsMessagesVerificationFailure)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseValidationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LicenseValidationFailedException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseValidationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public LicenseValidationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseValidationFailedException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about
        ///     the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about
        ///     the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private LicenseValidationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

#endif
}
