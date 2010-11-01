//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils.ExceptionHandling;

namespace Apollo.Core.Test.Spec
{
    /// <summary>
    /// A fake exception handler. This will later be removed and replaced
    /// by a proper one.
    /// </summary>
    /// <design>
    /// This class must be public because we use it in the AppDomainBuilder.
    /// </design>
    [Serializable]
    public sealed class MockExceptionHandler : IExceptionHandler
    {
        #region Implementation of IExceptionHandler

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
        public void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Do nothing really ....
        }

        #endregion
    }
}
