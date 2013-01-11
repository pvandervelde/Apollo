//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Scripting.Projects
{
    /// <summary>
    /// Defines the interface for objects that provide an interface abstraction to the
    /// projects.
    /// </summary>
    public interface ILinkScriptsToProjects
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
        /// Returns the currently active project.
        /// </summary>
        /// <returns>
        /// The currently active project.
        /// </returns>
        IProjectScriptFacade ActiveProject();

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
        /// An event raised when a new project is created or loaded.
        /// </summary>
        event EventHandler<EventArgs> OnNewProjectLoaded;

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
        /// <param name="projectFilePath">The path to the file in which the project was persisted.</param>
        void LoadProject(string projectFilePath);

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
        /// An event raised when the project is unloaded.
        /// </summary>
        event EventHandler<EventArgs> OnProjectUnloaded;
    }
}
