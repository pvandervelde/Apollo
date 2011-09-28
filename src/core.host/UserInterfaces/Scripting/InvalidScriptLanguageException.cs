//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Apollo.Core.Host.Properties;

namespace Apollo.Core.Host.UserInterfaces.Scripting
{
    /// <summary>
    /// An exception thrown when a script language which is not supported is selected
    /// in the <see cref="ScriptDomainLauncher"/>.
    /// </summary>
    [Serializable]
    public sealed class InvalidScriptLanguageException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScriptLanguageException"/> class.
        /// </summary>
        public InvalidScriptLanguageException()
            : this(Resources_NonTranslatable.Exceptions_Messages_InvalidScriptLanguage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScriptLanguageException"/> class.
        /// </summary>
        /// <param name="language">The unknown script language.</param>
        public InvalidScriptLanguageException(ScriptLanguage language)
            : this(
                string.Format(
                    CultureInfo.InvariantCulture, 
                    Resources_NonTranslatable.Exceptions_Messages_InvalidScriptLanguage_WithLanguage, 
                    language))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScriptLanguageException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidScriptLanguageException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScriptLanguageException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidScriptLanguageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScriptLanguageException"/> class.
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
        private InvalidScriptLanguageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
