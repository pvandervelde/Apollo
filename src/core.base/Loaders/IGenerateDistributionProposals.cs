//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines the interface for objects that provide distribution proposals.
    /// </summary>
    internal interface IGenerateDistributionProposals
    {
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
        IEnumerable<DistributionPlan> ProposeDistributionFor(DatasetRequest request, CancellationToken token);
    }
}
