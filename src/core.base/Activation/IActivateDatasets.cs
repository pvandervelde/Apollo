//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Defines the methods for activating a set of datasets on a set of machines.
    /// </summary>
    internal interface IActivateDatasets
    {
        /// <summary>
        /// Takes the set of distribution plans and activates the given datasets on the specified machines.
        /// </summary>
        /// <param name="planToImplement">The distribution plan that should be implemented.</param>
        /// <param name="token">The token used to indicate cancellation of the task.</param>
        /// <param name="progressReporter">The action that handles the reporting of progress.</param>
        /// <returns>
        /// A set of objects which allow act as proxies for the activated datasets.
        /// </returns>
        Task<DatasetOnlineInformation> ImplementPlan(
            DistributionPlan planToImplement,
            CancellationToken token,
            Action<int, string, bool> progressReporter);
    }
}
