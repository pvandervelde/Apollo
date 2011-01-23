//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utils.Licensing;

namespace Apollo.Core.Utils.Licensing
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
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "The return value is an IEnumerable, generating this may take time thus a method is more suitable.")]
        IEnumerable<ValidationSequence> GetLicenseValidationSequences();
    }
}
