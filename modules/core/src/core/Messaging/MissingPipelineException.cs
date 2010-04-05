//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.Core.Properties;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// An exception thrown when the <see cref="MessageProcessingAssistance"/> class is asked to send
    /// a message without having a link to an <see cref="IMessagePipeline"/>.
    /// </summary>
    [Serializable]
    public sealed class MissingPipelineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingPipelineException"/> class.
        /// </summary>
        public MissingPipelineException()
            : this(Resources_NonTranslatable.Exceptions_Messages_PipelineObjectMissing)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingPipelineException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MissingPipelineException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingPipelineException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MissingPipelineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingPipelineException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private MissingPipelineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
