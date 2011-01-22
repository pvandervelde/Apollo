﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Apollo.Core.Base.Projects;
using Apollo.Utils;

namespace Apollo.Core.Projects
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
        IBuildProjects WithDatasetDistributor(Func<DatasetRequest, DistributionPlan> distributor);

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
