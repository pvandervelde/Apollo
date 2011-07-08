//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utilities;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Defines the interface for dataset proxies that have an owner.
    /// </summary>
    internal interface IOwnedProxyDataset : IProxyDataset
    {
        /// <summary>
        /// A method called by the owner when the owner is about to delete the dataset.
        /// </summary>
        void OwnerHasDeletedDataset();

        /// <summary>
        /// A method called by the owner when the owner has dataset loading progress to report.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="mark">The action that is currently being processed.</param>
        void OwnerReportsDatasetLoadingProgress(int progress, IProgressMark mark);

        /// <summary>
        /// Called when the owner has successfully loaded the dataset onto one or more machines.
        /// </summary>
        void OwnerHasLoadedDataset();

        /// <summary>
        /// Called when the owner has successfully unloaded the dataset from the machines it was loaded onto.
        /// </summary>
        void OwnerHasUnloadedDataset();
    }
}
