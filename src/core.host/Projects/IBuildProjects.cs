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
using Apollo.Utilities;
using Apollo.Utilities.History;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for classes that construct <see cref="IProject"/> objects.
    /// </summary>
    internal interface IBuildProjects
    {
        /// <summary>
        /// Creates a new project definition object.
        /// </summary>
        /// <returns>
        /// The current builder instance with all the storage cleared.
        /// </returns>
        IBuildProjects Define();

        /// <summary>
        /// Provides the timeline which tracks the changes to the project over time.
        /// </summary>
        /// <param name="timeline">The timeline.</param>
        /// <returns>
        /// The current builder instance with the timeline stored.
        /// </returns>
        IBuildProjects WithTimeline(ITimeline timeline);

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
        IBuildProjects WithDatasetDistributor(Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor);

        /// <summary>
        /// Provides the function which creates a <see cref="DatasetStorageProxy"/> for a newly loaded dataset.
        /// </summary>
        /// <param name="storageBuilder">The function which returns a storage proxy for a newly loaded dataset.</param>
        /// <returns>
        /// The current builder instance with the stream containing the project stored.
        /// </returns>
        IBuildProjects WithDataStorageBuilder(Func<DatasetOnlineInformation, DatasetStorageProxy> storageBuilder);

        /// <summary>
        /// Provides the <see cref="Stream"/> from which the project must be loaded.
        /// </summary>
        /// <param name="persistenceInfo">
        /// The object that describes how the project was persisted.
        /// </param>
        /// <returns>
        /// The current builder instance with the stream containing the project stored.
        /// </returns>
        IBuildProjects FromStorage(IPersistenceInformation persistenceInfo);

        /// <summary>
        /// Builds a new project.
        /// </summary>
        /// <returns>
        /// The newly created project.
        /// </returns>
        IProject Build();
    }
}
