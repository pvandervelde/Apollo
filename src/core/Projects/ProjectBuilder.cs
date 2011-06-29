//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Properties;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Implements methods for building new <see cref="IProject"/> instances.
    /// </summary>
    internal sealed class ProjectBuilder : IBuildProjects
    {
        /// <summary>
        /// The function which returns a <c>DistributionPlan</c> for a given
        /// <c>DatasetRequest</c>.
        /// </summary>
        private Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> m_Distributor;

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
        /// Provides the function which handles <see cref="DatasetRequest"/> objects and generates the 
        /// <see cref="DistributionPlan"/> for the request.
        /// </summary>
        /// <param name="distributor">
        /// The function which handles <c>DatasetRequest</c> objects and generates the <c>DistributionPlan</c>
        /// for the request.
        /// </param>
        /// <returns>
        /// The current builder instance with the function stored.
        /// </returns>
        public IBuildProjects WithDatasetDistributor(Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor)
        {
            {
                Enforce.Argument(() => distributor);
            }

            m_Distributor = distributor;
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
                Enforce.Argument(() => persistenceInfo);
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
                Enforce.With<CannotCreateProjectWithoutDatasetDistributorException>(
                    m_Distributor != null,
                    Resources_NonTranslatable.Exception_Messages_CannotCreateProjectWithoutDatasetDistributor);
            }

            return new Project(m_Distributor, m_ProjectStorage);
        }
    }
}
