//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities.Licensing
{
    /// <summary>
    /// Defines the interface for the actual validation objects.
    /// </summary>
    internal interface IValidator
    {
        /// <summary>
        /// Validates the current license.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the license is valid; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool Validate();
    }
}
