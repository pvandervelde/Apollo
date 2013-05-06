//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Dataset.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines the interface for objects that process unhandled exceptions.
    /// </summary>
    public interface IExceptionProcessor : IDisposable
    {
        /// <summary>
        /// Processes the given exception.
        /// </summary>
        /// <param name="exception">The exception to process.</param>
        void Process(Exception exception);
    }
}
