﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Utils;

namespace Apollo.Core.UserInterfaces.Project
{
    /// <summary>
    /// Defines the interface for objects that provide an interface abstraction to the
    /// projects.
    /// </summary>
    public interface ILinkToProjects
    {
        /// <summary>
        /// Returns a value indicating if a project is loaded.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if a project is loaded; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasActiveProject();

        /// <summary>
        /// Returns a value indicating if a new project can be created.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if a new project can be created; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool CanCreateNewProject();

        /// <summary>
        /// Creates a new project.
        /// </summary>
        void NewProject();

        /// <summary>
        /// Returns a value indicating if a project can be loaded.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if a project can be loaded; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool CanLoadProject();

        /// <summary>
        /// Loads a new project from the given resource stream.
        /// </summary>
        /// <param name="persistenceInformation">The object that describes how the project was persisted.</param>
        void LoadProject(IPersistenceInformation persistenceInformation);

        /// <summary>
        /// Returns a value indicating if the existing project can be unloaded.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the existing project can be unloaded; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool CanUnloadProject();

        /// <summary>
        /// Unloads the current project.
        /// </summary>
        void UnloadProject();

        /// <summary>
        /// Returns a value indicating if the existing project should be saved.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the existing project should be saved; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool ShouldSaveProject();

        /// <summary>
        /// Saves the current project.
        /// </summary>
        /// <param name="persistenceInformation">The object that describes how the project should be persisted.</param>
        void SaveProject(IPersistenceInformation persistenceInformation);

        // - Get project information

        // - Get Dataset information
        //   - Graph
        //   - Datasets
    }
}
