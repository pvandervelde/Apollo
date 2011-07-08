//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Apollo.Utilities.Licensing;

namespace Apollo.Core.Utilities.Licensing
{
    /// <summary>
    /// Defines the interface for objects that generate 
    /// a set of <see cref="ValidationSequence"/> instances.
    /// </summary>
    internal interface IValidationSequenceGenerator
    {
        /// <summary>
        /// Generates a set of validation sequences.
        /// </summary>
        /// <returns>
        ///     A collection of <see cref="ValidationSequence"/> instances.
        /// </returns>
        IEnumerable<ValidationSequence> GetLicenseValidationSequences();
    }
}
