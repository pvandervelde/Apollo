//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base.Communication;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines methods for the distribution of datasets.
    /// </summary>
    public sealed class DatasetLoader : IHelpDistributingDatasets, ILoadDatasets
    {
        /// <summary>
        /// The object that provides the commands to load datasets onto other machines.
        /// </summary>
        private readonly Dictionary<EndpointId, IDatasetLoaderCommands> m_LoaderCommands =
            new Dictionary<EndpointId, IDatasetLoaderCommands>();

        // configuration?

        /// <summary>
        /// Processes the dataset request and creates a distribution plan 
        /// which can then be accepted by the user.
        /// </summary>
        /// <param name="request">
        /// The request that describes the characteristics of the dataset that 
        /// should be loaded.
        /// </param>
        /// <returns>
        /// The distribution plan that takes into account the characteristics of
        /// the dataset and the currently available computing power.
        /// </returns>
        public IObservable<DistributionPlan> ProposeDistributionFor(DatasetRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Takes the set of distribution plans and loads the given datasets onto the specified machines.
        /// </summary>
        /// <param name="plansToImplement">The collection of distribution plans.</param>
        /// <returns>
        /// A set of objects which allow act as proxies for the loaded datasets.
        /// </returns>
        public IObservable<DatasetOnlineInformation> ImplementPlan(IEnumerable<DistributionPlan> plansToImplement)
        {
            throw new NotImplementedException();
        }
    }
}
