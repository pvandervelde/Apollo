//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// An exception that is thrown when the user tries to delete an object that is not alive in 
    /// the current timeline.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "NonLiving",
        Justification = "The correct term is non-living, i.e. it's two words.")]
    public sealed class CannotRemoveNonLivingObjectException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotRemoveNonLivingObjectException"/> class.
        /// </summary>
        public CannotRemoveNonLivingObjectException() 
            : this(Resources.Exceptions_Messages_CannotRemoveNonLivingObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotRemoveNonLivingObjectException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CannotRemoveNonLivingObjectException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotRemoveNonLivingObjectException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CannotRemoveNonLivingObjectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotRemoveNonLivingObjectException"/> class.
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
        private CannotRemoveNonLivingObjectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
