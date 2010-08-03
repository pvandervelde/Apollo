//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils.Licensing;

namespace Apollo.Core.Utils.Licensing
{
    /// <summary>
    /// Defines the interface for objects that handle validation results.
    /// </summary>
    internal interface IValidationResultStorage
    {
        /// <summary>
        /// Used to store the latest license validation results.
        /// </summary>
        /// <param name="checksum">The checksum that was generated from the result.</param>
        /// <param name="generationTime">The time at which the result was generated.</param>
        /// <param name="expirationTime">The time after which the current result is considered expired.</param>
        void StoreLicenseValidationResult(Checksum checksum, DateTimeOffset generationTime, DateTimeOffset expirationTime);
    }
}
