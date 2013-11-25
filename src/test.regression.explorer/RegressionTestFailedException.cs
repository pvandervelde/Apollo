using System;
using System.Runtime.Serialization;

namespace Test.UI.Explorer
{
    /// <summary>
    /// An exception thrown when the user interface testing has failed.
    /// </summary>
    [Serializable]
    public sealed class RegressionTestFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionTestFailedException"/> class.
        /// </summary>
        public RegressionTestFailedException() 
            : this("The regression test of the application failed.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionTestFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RegressionTestFailedException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionTestFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RegressionTestFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionTestFailedException"/> class.
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
        private RegressionTestFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
