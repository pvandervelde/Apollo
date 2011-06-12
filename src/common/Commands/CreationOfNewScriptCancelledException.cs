//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.UI.Common.Properties;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// An exception thrown if the user cancels the creation of a new script.
    /// </summary>
    [Serializable]
    public sealed class CreationOfNewScriptCancelledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreationOfNewScriptCancelledException"/> class.
        /// </summary>
        public CreationOfNewScriptCancelledException()
            : this(Resources.Exceptions_Messages_CreationOfNewScriptCancelled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationOfNewScriptCancelledException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CreationOfNewScriptCancelledException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationOfNewScriptCancelledException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CreationOfNewScriptCancelledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationOfNewScriptCancelledException"/> class.
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
        private CreationOfNewScriptCancelledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
