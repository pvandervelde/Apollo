//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Utilities;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines the methods for loading a set of datasets onto a set of machines.
    /// </summary>
    internal interface ILoadDatasets
    {
        /// <summary>
        /// Takes the set of distribution plans and loads the given datasets onto the specified machines.
        /// </summary>
        /// <param name="planToImplement">The distribution plan that should be implemented.</param>
        /// <param name="token">The token used to indicate cancellation of the task.</param>
        /// <param name="progressReporter">The action that handles the reporting of progress.</param>
        /// <returns>
        /// A set of objects which allow act as proxies for the loaded datasets.
        /// </returns>
        Task<DatasetOnlineInformation> ImplementPlan(
            DistributionPlan planToImplement,
            CancellationToken token,
            Action<int, IProgressMark> progressReporter);
    }
}
