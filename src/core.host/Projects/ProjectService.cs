//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base;
using Apollo.Core.Base.Activation;
using Apollo.Utilities;
using Apollo.Utilities.History;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the <see cref="KernelService"/> that handles the project interaction with the kernel.
    /// </summary>
    internal sealed class ProjectService : KernelService
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The function that returns a new timeline each time it is called.
        /// </summary>
        private readonly Func<ITimeline> m_TimelineBuilder;

        /// <summary>
        /// The function which returns a storage proxy for a newly loaded dataset.
        /// </summary>
        private readonly Func<DatasetOnlineInformation, DatasetStorageProxy> m_DataStorageProxyBuilder;

        /// <summary>
        /// The object that handles loading of datasets either on the local machine or
        /// on remote machines.
        /// </summary>
        private readonly IHelpDistributingDatasets m_DatasetDistributor;

        /// <summary>
        /// The object that collects the notifications for the user interface.
        /// </summary>
        private readonly ICollectNotifications m_Notifications;

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
        /// <param name="timelineBuilder">The function that returns a new <see cref="ITimeline"/> object each time it is called.</param>
        /// <param name="dataStorageProxyBuilder">The function which returns a storage proxy for a newly loaded dataset.</param>
        /// <param name="datasetDistributor">The object that handles the distribution of datasets.</param>
        /// <param name="notifications">The object that stores the notifications for the user interface.</param>
        /// <param name="projectBuilder">The object that builds new projects.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="timelineBuilder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dataStorageProxyBuilder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="datasetDistributor"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="notifications"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="projectBuilder"/> is <see langword="null"/>.
        /// </exception>
        public ProjectService(
            Func<ITimeline> timelineBuilder,
            Func<DatasetOnlineInformation, DatasetStorageProxy> dataStorageProxyBuilder,
            IHelpDistributingDatasets datasetDistributor,
            ICollectNotifications notifications,
            IBuildProjects projectBuilder)
        {
            {
                Lokad.Enforce.Argument(() => timelineBuilder);
                Lokad.Enforce.Argument(() => dataStorageProxyBuilder);
                Lokad.Enforce.Argument(() => datasetDistributor);
                Lokad.Enforce.Argument(() => notifications);
                Lokad.Enforce.Argument(() => projectBuilder);
            }

            // No locks are necessary because we're in the constructor, no other
            // methods have been called or can be called.
            m_TimelineBuilder = timelineBuilder;
            m_DataStorageProxyBuilder = dataStorageProxyBuilder;
            m_DatasetDistributor = datasetDistributor;
            m_Notifications = notifications;
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
                    .WithTimeline(m_TimelineBuilder())
                    .WithDatasetDistributor((request, token) => m_DatasetDistributor.ProposeDistributionFor(request, token))
                    .WithDataStorageBuilder(m_DataStorageProxyBuilder)
                    .WithNotifications(m_Notifications)
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
                Lokad.Enforce.Argument(() => persistenceInfo);
            }

            UnloadProject();

            lock (m_Lock)
            {
                m_Current = m_Builder.Define()
                    .WithTimeline(m_TimelineBuilder())
                    .WithDatasetDistributor((request, token) => m_DatasetDistributor.ProposeDistributionFor(request, token))
                    .WithDataStorageBuilder(m_DataStorageProxyBuilder)
                    .WithNotifications(m_Notifications)
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

        /// <summary>
        /// Unloads the current project before shutting down the service.
        /// </summary>
        protected override void StopService()
        {
            UnloadProject();
        }
    }
}
