//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Dataset.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines the possible exit results for a guarded execution of a method.
    /// </summary>
    internal enum GuardResult
    {
        /// <summary>
        /// There was no exit result. Not normally a valid value.
        /// </summary>
        None,

        /// <summary>
        /// The method executed successfully.
        /// </summary>
        Success,

        /// <summary>
        /// The method execution failed at some point with an unhandled exception.
        /// </summary>
        Failure,
    }
}
