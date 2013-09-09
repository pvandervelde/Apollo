//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using Lokad;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Defines methods for the distribution of datasets.
    /// </summary>
    internal sealed class DatasetDistributionGenerator : IHelpDistributingDatasets
    {
        /// <summary>
        /// The distributor for the local machine.
        /// </summary>
        private readonly IEnumerable<IGenerateDistributionProposals> m_Distributors;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetDistributionGenerator"/> class.
        /// </summary>
        /// <param name="distributors">The collection of objects that handle distributing to the available machines.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="distributors"/> is <see langword="null" />.
        /// </exception>
        public DatasetDistributionGenerator(IEnumerable<IGenerateDistributionProposals> distributors)
        {
            {
                Enforce.Argument(() => distributors);
            }

            m_Distributors = distributors;
        }

        /// <summary>
        /// Processes the dataset request and creates a distribution plan 
        /// which can then be accepted by the user.
        /// </summary>
        /// <param name="activationRequest">
        /// The request that describes the characteristics of the dataset that 
        /// should be loaded.
        /// </param>
        /// <param name="token">The cancellation token that can be used to terminate the proposal.</param>
        /// <returns>
        /// The distribution plan that takes into account the characteristics of
        /// the dataset and the currently available computing power.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="activationRequest"/> is <see langword="null" />.
        /// </exception>
        public IEnumerable<DistributionPlan> ProposeDistributionFor(DatasetActivationRequest activationRequest, CancellationToken token)
        {
            {
                Enforce.Argument(() => activationRequest);
            }

            foreach (var distributor in m_Distributors)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                var proposals = distributor.ProposeDistributionFor(activationRequest, token);
                foreach (var proposal in proposals)
                {
                    yield return proposal;
                }
            }
        }
    }
}
