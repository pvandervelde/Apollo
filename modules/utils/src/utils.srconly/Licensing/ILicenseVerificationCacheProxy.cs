//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Utils.Licensing
{
    /// <summary>
    /// Defines the interface for objects that form a proxy between two
    /// <see cref="ILicenseVerificationCache"/> objects.
    /// </summary>
    internal interface ILicenseVerificationCacheProxy
    {
        /// <summary>
        /// Gets the latest verification result.
        /// </summary>
        /// <value>The latest result.</value>
        LicenseCheckResult LatestResult
        {
            get;
        }
    }
}
