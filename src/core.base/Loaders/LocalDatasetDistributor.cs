//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;
using Lokad;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Provide dataset loading proposals for the local machine.
    /// </summary>
    internal sealed class LocalDatasetDistributor : IGenerateDistributionProposals, ILoadDatasets
    {
        /// <summary>
        /// The object that handles the distribution proposals for the local machine.
        /// </summary>
        private readonly ICalculateDistributionParameters m_LocalDistributor;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDatasetDistributor"/> class.
        /// </summary>
        /// <param name="localDistributor">The object that handles distribution proposals for the local machine.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localDistributor"/> is <see langword="null" />.
        /// </exception>
        public LocalDatasetDistributor(ICalculateDistributionParameters localDistributor)
        {
            {
                Enforce.Argument(() => localDistributor);
            }

            m_LocalDistributor = localDistributor;
        }

        /// <summary>
        /// Processes the dataset request and returns a collection of distribution plans.
        /// </summary>
        /// <param name="request">
        /// The request that describes the characteristics of the dataset that 
        /// should be loaded.
        /// </param>
        /// <param name="token">The cancellation token that can be used to terminate the proposal.</param>
        /// <returns>
        /// The collection containing all the distribution plans.
        /// </returns>
        public IEnumerable<DistributionPlan> ProposeDistributionFor(DatasetRequest request, CancellationToken token)
        {
            var proposal = m_LocalDistributor.ProposeForLocalMachine(request.ExpectedLoadPerMachine);
            var plan = new DistributionPlan(
                (p, t) => ImplementPlan(p, t),
                request.DatasetToLoad,
                new NetworkIdentifier(proposal.Endpoint.OriginatesOnMachine()),
                proposal);
            return new DistributionPlan[] { plan };
        }

        /// <summary>
        /// Takes the set of distribution plans and loads the given datasets onto the specified machines.
        /// </summary>
        /// <param name="planToImplement">The distribution plan that should be implemented.</param>
        /// <param name="token">The token used to indicate cancellation of the task.</param>
        /// <returns>
        /// A set of objects which allow act as proxies for the loaded datasets.
        /// </returns>
        public Task<DatasetOnlineInformation> ImplementPlan(DistributionPlan planToImplement, CancellationToken token)
        {
            // Call the local loader to start the loading process
            // - Need to somehow transfer the file
            // - Might need to transfer assemblies (or do we let other parts figure that out)
            // - Returns channel connection information which needs to be passed on somehow
            throw new NotImplementedException();
        }
    }
}
