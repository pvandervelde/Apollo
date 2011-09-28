//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Host.UserInterfaces.Scripting
{
    /// <summary>
    /// Defines the interface for objects that form a facade of a project for the
    /// scripting API.
    /// </summary>
    public interface IProjectScriptFacade
    {
        /// <summary>
        /// Gets a value indicating whether the project has been closed.
        /// </summary>
        bool IsClosed
        {
            get;
        }

        /// <summary>
        /// The event raised when the project is closed.
        /// </summary>
        event EventHandler<EventArgs> OnClosed;

        /// <summary>
        /// Gets or sets a value indicating the name of the project.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// An event raised when the name of the project is updated.
        /// </summary>
        event EventHandler<EventArgs> OnNameChanged;

        /// <summary>
        /// Gets or sets a value describing the project.
        /// </summary>
        string Summary
        {
            get;
            set;
        }

        /// <summary>
        /// An event raised when the summary of the project is updated.
        /// </summary>
        event EventHandler<EventArgs> OnSummaryChanged;

        /// <summary>
        /// Gets a value indicating the number of dataset for the project.
        /// </summary>
        int NumberOfDatasets
        {
            get;
        }

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
        /// <param name="filePath">The path to the file where the project should be persisted.</param>
        void SaveProject(string filePath);

        /// <summary>
        /// Returns the root dataset for the current project.
        /// </summary>
        /// <returns>The root dataset.</returns>
        IDatasetScriptFacade Root();

        /// <summary>
        /// An event raised when a new dataset is created.
        /// </summary>
        event EventHandler<EventArgs> OnDatasetCreated;

        /// <summary>
        /// An event raised when a dataset is deleted.
        /// </summary>
        event EventHandler<EventArgs> OnDatasetDeleted;
    }
}
