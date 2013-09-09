//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Defines the interface for objects that calculate distribution parameters.
    /// </summary>
    internal interface ICalculateDistributionParameters
    {
        /// <summary>
        /// Returns a dataset activation proposal for the given expected load.
        /// </summary>
        /// <param name="expectedLoad">The load the dataset is expected to put on the machine.</param>
        /// <returns>
        ///     A proposal that indicates if the machine can activation the dataset and what the
        ///     expected activation performance will be like.
        /// </returns>
        DatasetActivationProposal ProposeForLocalMachine(ExpectedDatasetLoad expectedLoad);
    }
}
