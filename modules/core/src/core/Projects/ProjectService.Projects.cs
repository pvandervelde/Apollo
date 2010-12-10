//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base;
using Apollo.Core.Base.Projects;
using Lokad;

namespace Apollo.Core.Projects
{
    /// <content>
    /// Defines the methods used for working with projects.
    /// </content>
    internal sealed partial class ProjectService
    {
        /// <summary>
        /// The object that handles loading of datasets either on the local machine or
        /// on remote machines.
        /// </summary>
        private readonly IHelpDistributingDatasets m_DatasetDistributor;

        /// <summary>
        /// Handles the construction of new project objects.
        /// </summary>
        private readonly IBuildProjects m_Builder;

        /// <summary>
        /// The currently active project.
        /// </summary>
        private IProject m_Current;

        /// <summary>
        /// Create a new empty project.
        /// </summary>
        public void CreateNewProject()
        {
            UnloadProject();

            lock (m_Lock)
            {
                m_Current = m_Builder.Define()
                    .WithDatasetDistributor(request => m_DatasetDistributor.ProposeDistributionFor(request))
                    .Build();
            }
        }

        /// <summary>
        /// Loads the project from the given stream. Note that in first instance
        /// only the project is loaded, the datasets will not be loaded
        /// until requested.
        /// </summary>
        /// <param name="persistenceInfo">
        /// The object that describes how the project was persisted.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="persistenceInfo"/> is <see langword="null" />.
        /// </exception>
        public void LoadProject(IPersistenceInformation persistenceInfo)
        {
            {
                Enforce.Argument(() => persistenceInfo);
            }

            UnloadProject();

            lock (m_Lock)
            {
                m_Current = m_Builder.Define()
                    .WithDatasetDistributor(request => m_DatasetDistributor.ProposeDistributionFor(request))
                    .FromStorage(persistenceInfo)
                    .Build();
            }
        }

        /// <summary>
        /// Asynchronously saves the current project to a stream. Note that 
        /// saving the project may take some time if there are any remote 
        /// datasets.
        /// </summary>
        /// <param name="persistenceInfo">The object that describes how the project should be persisted.</param>
        /// <design>
        /// Note that this operation may take a considerable time (think several minutes
        /// or more). Especially if the project has multiple datasets loaded onto one
        /// or more remote machines.
        /// </design>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="persistenceInfo"/> is <see langword="null" />.
        /// </exception>
        public void SaveProject(IPersistenceInformation persistenceInfo)
        {
            {
                Enforce.Argument(() => persistenceInfo);
            }

            IProject project;
            lock (m_Lock)
            {
                project = m_Current;
            }

            if (project != null)
            {
                project.Save(persistenceInfo);
            }
        }

        /// <summary>
        /// Unloads the project from memory. The project is not saved even if there
        /// are unsaved changes.
        /// </summary>
        /// <design>
        /// It is expected that shutting down a project may take some time if calculations 
        /// are running etc.
        /// </design>
        public void UnloadProject()
        {
            IProject current = null;
            lock (m_Lock)
            {
                current = m_Current;
                m_Current = null;
            }

            if (current == null)
            {
                return;
            }

            current.Close();
        }
    }
}
