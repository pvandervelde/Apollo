//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.Licensing
{
    /// <summary>
    /// A delegate used to set the latest license verification results.
    /// </summary>
    /// <param name="checksum">The checksum that was generated from the result.</param>
    /// <param name="generationTime">The time at which the result was generated.</param>
    /// <param name="expirationTime">The time after which the current result is considered expired.</param>
    internal delegate void LicenseResultUpdated(Checksum checksum, DateTimeOffset generationTime, DateTimeOffset expirationTime);
}
