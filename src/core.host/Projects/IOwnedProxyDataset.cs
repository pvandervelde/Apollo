﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities;

namespace Apollo.Core.Host.Projects
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
        /// A method called by the owner when the owner has progress to report for the
        /// current action that the dataset is executing.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="mark">The action that is currently being processed.</param>
        /// <param name="estimatedTime">
        ///     The amount of time it will take to finish the entire task from start to finish. Can be negative 
        ///     if no time is known.
        /// </param>
        void OwnerReportsDatasetCurrentActionProgress(int progress, IProgressMark mark, TimeSpan estimatedTime);

        /// <summary>
        /// Called when the owner has successfully loaded the dataset onto one or more machines.
        /// </summary>
        void OwnerHasLoadedDataset();

        /// <summary>
        /// Called when the owner has successfully unloaded the dataset from the machines it was loaded onto.
        /// </summary>
        void OwnerHasUnloadedDataset();

        /// <summary>
        /// Called when the owner got notification of a change in the history of the current dataset.
        /// </summary>
        void OwnerHasBeenNotifiedOfHistoryChange();

        /// <summary>
        /// Called when the owner gets the notification that the dataset has switched to edit mode.
        /// </summary>
        void OwnerHasBeenNotifiedOfSwitchToEditMode();

        /// <summary>
        /// Called when the owner gets the notification that the dataset has switched to executing mode.
        /// </summary>
        void OwnerHasBeenNotifiedOfSwitchToExecutingMode();
    }
}
