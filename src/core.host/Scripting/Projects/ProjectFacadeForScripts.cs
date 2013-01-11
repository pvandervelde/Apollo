//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Core.Scripting.Projects;
using Lokad;

namespace Apollo.Core.Host.Scripting.Projects
{
    /// <summary>
    /// Forms a facade of a project for the scripting API.
    /// </summary>
    /// <design>
    /// <para>
    /// There should really only be one of these. If there are more then we could end up in the situation where
    /// one facade creates a new project but the other facade(s) don't get the new project. 
    /// </para>
    /// </design>
    internal sealed class ProjectFacadeForScripts : MarshalByRefObject, IProjectScriptFacade
    {
        /// <summary>
        /// The object that describes the current project.
        /// </summary>
        private readonly ProjectFacade m_Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectFacadeForScripts"/> class.
        /// </summary>
        /// <param name="current">The object that describes the current project.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="current"/> is <see langword="null" />.
        /// </exception>
        public ProjectFacadeForScripts(ProjectFacade current)
        {
            {
                Enforce.Argument(() => current);
            }

            m_Current = current;
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
        public event EventHandler<EventArgs> OnClosed;

        private void RaiseOnProjectClosed()
        {
            var local = OnClosed;
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
        public event EventHandler<EventArgs> OnNameChanged;

        private void RaiseOnProjectNameUpdated()
        {
            var local = OnNameChanged;
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
        public event EventHandler<EventArgs> OnSummaryChanged;

        private void RaiseOnProjectSummaryUpdated()
        {
            var local = OnSummaryChanged;
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
            return m_Current != null;
        }

        /// <summary>
        /// Saves the current project.
        /// </summary>
        /// <param name="filePath">The path to the file where the project should be persisted.</param>
        public void SaveProject(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the root dataset for the current project.
        /// </summary>
        /// <returns>The root dataset.</returns>
        public IDatasetScriptFacade Root()
        {
            var dataset = m_Current.Root();
            return new DatasetFacadeForScripts(dataset);
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

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        ///     An object of type System.Runtime.Remoting.Lifetime.ILease used to control
        ///     the lifetime policy for this instance. This is the current lifetime service
        ///     object for this instance if one exists; otherwise, a new lifetime service
        ///     object initialized to the value of the 
        ///     System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime property.
        /// </returns>
        public override object InitializeLifetimeService()
        {
            // We don't allow the object to die, unless we
            // release the references.
            return null;
        }
    }
}
