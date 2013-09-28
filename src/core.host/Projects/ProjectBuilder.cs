//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Apollo.Core.Base;
using Apollo.Core.Base.Activation;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;
using Apollo.Utilities.History;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Implements methods for building new <see cref="IProject"/> instances.
    /// </summary>
    internal sealed class ProjectBuilder : IBuildProjects
    {
        /// <summary>
        /// The timeline that stores all the changes.
        /// </summary>
        private ITimeline m_Timeline;

        /// <summary>
        /// The function which returns a <c>DistributionPlan</c> for a given
        /// <c>DatasetActivationRequest</c>.
        /// </summary>
        private Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> m_Distributor;

        /// <summary>
        /// The function which returns a storage proxy for a newly loaded dataset.
        /// </summary>
        private Func<DatasetOnlineInformation, DatasetStorageProxy> m_StorageBuilder;

        /// <summary>
        /// The object that collects the notifications for the user interface.
        /// </summary>
        private ICollectNotifications m_Notifications;

        /// <summary>
        /// The object describes how the project was persisted.
        /// </summary>
        private IPersistenceInformation m_ProjectStorage;

        /// <summary>
        /// Creates a new project definition object.
        /// </summary>
        /// <returns>
        /// The current builder instance with all the storage cleared.
        /// </returns>
        public IBuildProjects Define()
        {
            m_Distributor = null;
            m_ProjectStorage = null;

            return this;
        }

        /// <summary>
        /// Provides the timeline which tracks the changes to the project over time.
        /// </summary>
        /// <param name="timeline">The timeline.</param>
        /// <returns>
        /// The current builder instance with the timeline stored.
        /// </returns>
        public IBuildProjects WithTimeline(ITimeline timeline)
        {
            {
                Lokad.Enforce.Argument(() => timeline);
            }

            m_Timeline = timeline;
            return this;
        }

        /// <summary>
        /// Provides the function which handles <see cref="DatasetActivationRequest"/> objects and generates the 
        /// <see cref="DistributionPlan"/> for the request.
        /// </summary>
        /// <param name="distributor">
        /// The function which handles <c>DatasetActivationRequest</c> objects and generates the <c>DistributionPlan</c>
        /// for the request.
        /// </param>
        /// <returns>
        /// The current builder instance with the function stored.
        /// </returns>
        public IBuildProjects WithDatasetDistributor(Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor)
        {
            {
                Lokad.Enforce.Argument(() => distributor);
            }

            m_Distributor = distributor;
            return this;
        }

        /// <summary>
        /// Provides the function which creates a <see cref="DatasetStorageProxy"/> for a newly loaded dataset.
        /// </summary>
        /// <param name="storageBuilder">The function which returns a storage proxy for a newly loaded dataset.</param>
        /// <returns>
        /// The current builder instance with the stream containing the project stored.
        /// </returns>
        public IBuildProjects WithDataStorageBuilder(Func<DatasetOnlineInformation, DatasetStorageProxy> storageBuilder)
        {
            {
                Lokad.Enforce.Argument(() => storageBuilder);
            }

            m_StorageBuilder = storageBuilder;
            return this;
        }

        /// <summary>
        /// Provides the object that will store the notifications for use by the user interface.
        /// </summary>
        /// <param name="notifications">The object that stores the notifications for the user interface.</param>
        /// <returns>
        /// The current builder instance with the notification object stored.
        /// </returns>
        public IBuildProjects WithNotifications(ICollectNotifications notifications)
        {
            {
                Lokad.Enforce.Argument(() => notifications);
            }

            m_Notifications = notifications;
            return this;
        }

        /// <summary>
        /// Provides the <see cref="Stream"/> from which the project must be loaded.
        /// </summary>
        /// <param name="persistenceInfo">
        /// The object that describes how the project was persisted.
        /// </param>
        /// <returns>
        /// The current builder instance with the stream containing the project stored.
        /// </returns>
        public IBuildProjects FromStorage(IPersistenceInformation persistenceInfo)
        {
            {
                Lokad.Enforce.Argument(() => persistenceInfo);
            }

            m_ProjectStorage = persistenceInfo;
            return this;
        }

        /// <summary>
        /// Builds a new project.
        /// </summary>
        /// <returns>
        /// The newly created project.
        /// </returns>
        public IProject Build()
        {
            {
                Lokad.Enforce.With<CannotCreateProjectWithoutTimelineException>(
                    m_Timeline != null,
                    Resources.Exceptions_Messages_CannotCreateProjectWithoutTimeline);

                Lokad.Enforce.With<CannotCreateProjectWithoutDatasetDistributorException>(
                    m_Distributor != null,
                    Resources.Exceptions_Messages_CannotCreateProjectWithoutDatasetDistributor);
            }

            return new Project(m_Timeline, m_Distributor, m_StorageBuilder, m_Notifications, m_ProjectStorage);
        }
    }
}
