//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.Core.Properties;

namespace Apollo.Core.UserInterfaces.Scripting
{
    /// <summary>
    /// An exception thrown when the user tries to run multiple scripts at the same time.
    /// </summary>
    [Serializable]
    public sealed class CannotInterruptRunningScriptException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotInterruptRunningScriptException"/> class.
        /// </summary>
        public CannotInterruptRunningScriptException()
            : this(Resources_NonTranslatable.Exceptions_Messages_CannotInteruptRunningScript)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotInterruptRunningScriptException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CannotInterruptRunningScriptException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotInterruptRunningScriptException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CannotInterruptRunningScriptException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotInterruptRunningScriptException"/> class.
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
        private CannotInterruptRunningScriptException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
