//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Defines the interface for objects that provide connections to plug-in repositories.
    /// </summary>
    public interface IProvideConnectionToRepositories
    {
        /// <summary>
        /// Returns a value indicating whether the specified repository can be contacted.
        /// </summary>
        /// <param name="id">The ID of the repository.</param>
        /// <returns>
        /// <see langword="true" /> if the specified repository can be contacted; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsConnectedToRepository(PluginRepositoryId id);

        /// <summary>
        /// Returns a collection containing the IDs of all the repositories that can be contacted.
        /// </summary>
        /// <returns>A collection containing the IDs of all the connected repositories.</returns>
        IEnumerable<PluginRepositoryId> Repositories();
    }
}
