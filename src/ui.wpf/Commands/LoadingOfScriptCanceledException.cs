//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.UI.Wpf.Properties;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// An exception thrown if the user cancels the loading of an existing script.
    /// </summary>
    [Serializable]
    public sealed class LoadingOfScriptCanceledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingOfScriptCanceledException"/> class.
        /// </summary>
        public LoadingOfScriptCanceledException()
            : this(Resources.Exceptions_Messages_LoadingOfScriptCancelled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingOfScriptCanceledException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LoadingOfScriptCanceledException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingOfScriptCanceledException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public LoadingOfScriptCanceledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingOfScriptCanceledException"/> class.
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
        private LoadingOfScriptCanceledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
