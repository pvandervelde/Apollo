//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Utilities.Licensing;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Defines the <see cref="KernelService"/> that handles the project interaction with the kernel.
    /// </summary>
    internal sealed partial class ProjectService : KernelService
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The object that stores the validity of the license.
        /// </summary>
        private readonly IValidationResultStorage m_LicenseValidationStorage;

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
        /// Initializes a new instance of the <see cref="ProjectService"/> class.
        /// </summary>
        /// <param name="licenseValidationStorage">The object that stores the validity of the license.</param>
        /// <param name="datasetDistributor">The object that handles the distribution of datasets.</param>
        /// <param name="projectBuilder">The object that builds new projects.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="licenseValidationStorage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="datasetDistributor"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="projectBuilder"/> is <see langword="null"/>.
        /// </exception>
        public ProjectService(
            IValidationResultStorage licenseValidationStorage,
            IHelpDistributingDatasets datasetDistributor,
            IBuildProjects projectBuilder)
            : base()
        {
            {
                Enforce.Argument(() => licenseValidationStorage);
                Enforce.Argument(() => datasetDistributor);
                Enforce.Argument(() => projectBuilder);
            }

            // No locks are necessary because we're in the constructor, no other
            // methods have been called or can be called.
            m_LicenseValidationStorage = licenseValidationStorage;
            m_DatasetDistributor = datasetDistributor;
            m_Builder = projectBuilder;
        }

        /// <summary>
        /// Create a new empty project.
        /// </summary>
        public void CreateNewProject()
        {
            UnloadProject();

            lock (m_Lock)
            {
                m_Current = m_Builder.Define()
                    .WithDatasetDistributor((request, token) => m_DatasetDistributor.ProposeDistributionFor(request, token))
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
                    .WithDatasetDistributor((request, token) => m_DatasetDistributor.ProposeDistributionFor(request, token))
                    .FromStorage(persistenceInfo)
                    .Build();
            }
        }

        /// <summary>
        /// Gets the current project.
        /// </summary>
        public IProject Current
        {
            get
            {
                return m_Current;
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
            ICanClose current = null;
            lock (m_Lock)
            {
                current = m_Current as ICanClose;
                m_Current = null;
            }

            if (current == null)
            {
                return;
            }

            // Close the project. After this method call
            // the project cannot be used anymore
            current.Close();
        }

        #region Overrides

        /// <summary>
        /// Unloads the current project before shutting down the service.
        /// </summary>
        protected override void StopService()
        {
            UnloadProject();
        }

        #endregion
    }
}
