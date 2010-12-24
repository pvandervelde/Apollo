//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Apollo.Core.Base.Projects;
using Apollo.Core.Properties;

namespace Apollo.Core.UserInterfaces.Project
{
    /// <summary>
    /// An exception thrown when the user tries to delete a dataset that cannot be deleted.
    /// </summary>
    [Serializable]
    public sealed class CannotDeleteDatasetException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotDeleteDatasetException"/> class.
        /// </summary>
        public CannotDeleteDatasetException()
            : this(Resources_NonTranslatable.Exception_Messages_CannotDeleteDataset)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotDeleteDatasetException"/> class.
        /// </summary>
        /// <param name="dataset">The ID number of the dataset that could not be deleted.</param>
        public CannotDeleteDatasetException(DatasetId dataset)
            : base(string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.Exception_Messages_CannotDeleteDataset_WithDatasetId, dataset))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotDeleteDatasetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CannotDeleteDatasetException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotDeleteDatasetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CannotDeleteDatasetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotDeleteDatasetException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private CannotDeleteDatasetException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
