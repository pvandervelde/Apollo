//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Common.Bootstrappers
{
    /// <summary>
    /// Extension methods for <see cref="ContainerRegistrationScope"/>.
    /// </summary>
    /// <source>
    /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter.
    /// </source>
    public static class ContainerRegistrationScopeExtensions
    {
        /// <summary>
        /// Determines whether the specified scope is singleton.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified scope is singleton; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsSingleton(this ContainerRegistrationScope scope)
        {
            return scope == ContainerRegistrationScope.Singleton;
        }

        /// <summary>
        /// Determines whether the specified scope is instance.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified scope is instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsInstance(this ContainerRegistrationScope scope)
        {
            return scope == ContainerRegistrationScope.Instance;
        }
    }
}