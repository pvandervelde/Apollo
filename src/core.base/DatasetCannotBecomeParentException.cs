//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Apollo.Core.Base.Properties;

namespace Apollo.Core.Base
{
    /// <summary>
    /// An exception thrown when a new dataset is requested as a child of another 
    /// dataset which is not allowed to have children.
    /// </summary>
    [Serializable]
    public sealed class DatasetCannotBecomeParentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetCannotBecomeParentException"/> class.
        /// </summary>
        public DatasetCannotBecomeParentException()
            : this(Resources.Exceptions_Messages_DatasetCannotBecomeParent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetCannotBecomeParentException"/> class.
        /// </summary>
        /// <param name="id">The ID number of the dataset that could not be made a parent.</param>
        public DatasetCannotBecomeParentException(DatasetId id)
            : this(string.Format(CultureInfo.InvariantCulture, Resources.Exceptions_Messages_DatasetCannotBecomeParent_WithId, id))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetCannotBecomeParentException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DatasetCannotBecomeParentException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetCannotBecomeParentException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DatasetCannotBecomeParentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetCannotBecomeParentException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private DatasetCannotBecomeParentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
