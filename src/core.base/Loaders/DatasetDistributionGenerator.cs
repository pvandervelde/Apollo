//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lokad;

namespace Apollo.Core.Base.Loaders
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
        /// <param name="request">
        /// The request that describes the characteristics of the dataset that 
        /// should be loaded.
        /// </param>
        /// <param name="token">The cancellation token that can be used to terminate the proposal.</param>
        /// <returns>
        /// The distribution plan that takes into account the characteristics of
        /// the dataset and the currently available computing power.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="request"/> is <see langword="null" />.
        /// </exception>
        public Task<IEnumerable<DistributionPlan>> ProposeDistributionFor(DatasetRequest request, CancellationToken token)
        {
            {
                Enforce.Argument(() => request);
            }

            Func<IEnumerable<DistributionPlan>> action =
                () =>
                {
                    var list = new List<DistributionPlan>();
                    foreach (var distributor in m_Distributors)
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }

                        var proposals = distributor.ProposeDistributionFor(request, token);
                        list.AddRange(proposals);
                    }
                    
                    var comparer = new DatasetLoadingProposalComparer();
                    list.Sort((x, y) => comparer.Compare(x.Proposal, y.Proposal));
                    
                    return list;
                };

            return Task<IEnumerable<DistributionPlan>>.Factory.StartNew(action, token, TaskCreationOptions.LongRunning, null);
        }
    }
}
