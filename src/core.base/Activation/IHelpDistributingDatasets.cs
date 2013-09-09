//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Defines the interface for objects which handle activating of datasets either on the
    /// local machine or on a remote machine.
    /// </summary>
    public interface IHelpDistributingDatasets
    {
        /// <summary>
        /// Processes the dataset request and creates a distribution plan 
        /// which can then be accepted by the user.
        /// </summary>
        /// <param name="activationRequest">
        /// The request that describes the characteristics of the dataset that 
        /// should be activated.
        /// </param>
        /// <param name="token">The cancellation token that can be used to terminate the proposal.</param>
        /// <returns>
        /// The distribution plan that takes into account the characteristics of
        /// the dataset and the currently available computing power.
        /// </returns>
        IEnumerable<DistributionPlan> ProposeDistributionFor(DatasetActivationRequest activationRequest, CancellationToken token);
    }
}
