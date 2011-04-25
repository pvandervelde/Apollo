//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Projects;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.UserInterfaces.Projects
{
    /// <summary>
    /// Defines a facade for a project.
    /// </summary>
    /// <design>
    /// <para>
    /// There should really only be one of these. If there are more then we could end up in the situation where
    /// one facade creates a new project but the other facade(s) don't get the new project. 
    /// </para>
    /// </design>
    public sealed class ProjectFacade
    {
        /// <summary>
        /// The current project.
        /// </summary>
        private readonly IProject m_Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectFacade"/> class.
        /// </summary>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="project"/> is <see langword="null" />.
        /// </exception>
        internal ProjectFacade(IProject project)
        {
            {
                Enforce.Argument(() => project);
            }

            m_Current = project;
            m_Current.OnClosed += (s, e) => RaiseOnProjectClosed();
            m_Current.OnDatasetCreated += (s, e) => RaiseOnDatasetCreated();
            m_Current.OnDatasetDeleted += (s, e) => RaiseOnDatasetDeleted();
            m_Current.OnNameChanged += (s, e) => RaiseOnProjectNameUpdated();
            m_Current.OnSummaryChanged += (s, e) => RaiseOnProjectSummaryUpdated();
        }

        /// <summary>
        /// Gets a value indicating whether the project has been closed.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return m_Current.IsClosed;
            }
        }

        /// <summary>
        /// The event raised when the project is closed.
        /// </summary>
        public event EventHandler<EventArgs> OnProjectClosed;

        private void RaiseOnProjectClosed()
        {
            var local = OnProjectClosed;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the name of the project.
        /// </summary>
        public string Name
        {
            get 
            {
                return m_Current.Name;
            }

            set
            {
                m_Current.Name = value;
            }
        }

        /// <summary>
        /// An event raised when the name of the project is updated.
        /// </summary>
        public event EventHandler<EventArgs> OnProjectNameUpdated;

        private void RaiseOnProjectNameUpdated()
        {
            var local = OnProjectNameUpdated;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value describing the project.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Current.Summary;
            }

            set
            {
                m_Current.Summary = value;
            }
        }

        /// <summary>
        /// An event raised when the summary of the project is updated.
        /// </summary>
        public event EventHandler<EventArgs> OnProjectSummaryUpdated;

        private void RaiseOnProjectSummaryUpdated()
        {
            var local = OnProjectSummaryUpdated;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating the number of dataset for the project.
        /// </summary>
        public int NumberOfDatasets
        {
            get
            {
                return m_Current.NumberOfDatasets;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the project has changed since the 
        /// last save.
        /// </summary>
        public bool HasProjectChanged
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a value indicating if the existing project should be saved.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the existing project should be saved; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool ShouldSaveProject()
        {
            // Really we should only be able to save if there is something to save
            // Then again we should always be able to save a project under a new name ...
            return (m_Current != null) && HasProjectChanged;
        }

        /// <summary>
        /// Saves the current project.
        /// </summary>
        /// <param name="persistenceInformation">The object that describes how the project should be persisted.</param>
        public void SaveProject(IPersistenceInformation persistenceInformation)
        {
            if (m_Current != null)
            {
                m_Current.Save(persistenceInformation);
                HasProjectChanged = false;
            }
        }

        /// <summary>
        /// Returns the root dataset for the current project.
        /// </summary>
        /// <returns>The root dataset.</returns>
        public DatasetFacade Root()
        {
            var dataset = m_Current.BaseDataset();
            return new DatasetFacade(dataset);
        }

        /// <summary>
        /// An event raised when a new dataset is created.
        /// </summary>
        public event EventHandler<EventArgs> OnDatasetCreated;

        private void RaiseOnDatasetCreated()
        {
            var local = OnDatasetCreated;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when a dataset is deleted.
        /// </summary>
        public event EventHandler<EventArgs> OnDatasetDeleted;

        private void RaiseOnDatasetDeleted()
        {
            var local = OnDatasetDeleted;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }
    }
}
