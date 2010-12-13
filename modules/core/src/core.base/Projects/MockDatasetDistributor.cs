//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Projects
{
    /// <summary>
    /// This is a fake implementation of the <see cref="IHelpDistributingDatasets"/> interface.
    /// </summary>
    public sealed class MockDatasetDistributor : IHelpDistributingDatasets
    {
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
        public DistributionPlan ProposeDistributionFor(DatasetRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
