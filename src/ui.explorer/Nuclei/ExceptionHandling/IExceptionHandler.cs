//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Explorer.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines the interface for objects which take care of the last chance exception handling 
    /// actions.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Used when an unhandled exception occurs in an <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="isApplicationTerminating">Indicates if the application is about to shut down or not.</param>
        void OnException(Exception exception, bool isApplicationTerminating);
    }
}
