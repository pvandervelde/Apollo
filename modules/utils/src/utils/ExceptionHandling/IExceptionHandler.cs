//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utils.ExceptionHandling
{
    /// <summary>
    /// Defines the interface for objects which take care of the last chance exception handling 
    /// actions.
    /// </summary>
    /// <remarks>
    /// Must be implemented by <see cref="MarshalByRefObject"/> objects or must be serializable
    /// because these object interact with elements from a specific <c>AppDomain</c>.
    /// </remarks>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Used when an unhandled exception occurs in an <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="sender">
        ///     The object from which the event origined.
        /// </param>
        /// <param name="e">
        ///     The event arguments which where the exception is originating from
        ///     and if the <c>AppDomain</c> is shutting down.
        /// </param>
        void OnUnhandledException(object sender, UnhandledExceptionEventArgs e);
    }
}
