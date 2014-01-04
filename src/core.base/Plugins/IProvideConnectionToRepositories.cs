//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
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

        /// <summary>
        /// Returns a value indicating if the repository with the given <paramref name="id"/> has information about a type 
        /// with the given <paramref name="fullyQualifiedName"/>.
        /// </summary>
        /// <param name="id">The ID of the plug-in repository.</param>
        /// <param name="fullyQualifiedName">The assembly qualified name of the type.</param>
        /// <returns>
        /// <see langword="true" /> if the specified repository has information about the given type; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasTypeInformation(PluginRepositoryId id, string fullyQualifiedName);

        /// <summary>
        /// Gets the plug-in information from the specified repository.
        /// </summary>
        /// <param name="id">The ID of the repository from which the plug-in information should be obtained.</param>
        /// <returns>The information about all the plug-ins stored by the repository.</returns>
        RepositoryPluginInformation PluginInformationFrom(PluginRepositoryId id);

        /// <summary>
        /// An event raised if a new plug-in repository connects.
        /// </summary>
        event EventHandler<PluginRepositoryEventArgs> OnRepositoryConnected;

        /// <summary>
        /// An event raised if a plug-in repository indicates that it has updated one or 
        /// more plug-in definitions.
        /// </summary>
        event EventHandler<PluginRepositoryEventArgs> OnRepositoryUpdated;

        /// <summary>
        /// An event raised if a plug-in repository disconnects.
        /// </summary>
        event EventHandler<PluginRepositoryEventArgs> OnRepositoryDisconnected;
    }
}
