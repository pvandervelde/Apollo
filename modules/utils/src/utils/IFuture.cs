//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utils
{
    /// <summary>
    /// Defines the interface for objects which deliver a value at some point in the future.
    /// </summary>
    /// <typeparam name="T">The type of the value that will be delivered at some point in time.</typeparam>
    public interface IFuture<T>
    {
        /// <summary>
        /// Gets a value indicating whether the result has been obtained.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if the result has been obtained; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasResult
        {
            get;
        }

        /// <summary>
        /// Returns the result of the future computation.
        /// </summary>
        /// <returns>The requested value.</returns>
        T Result();
    }
}
