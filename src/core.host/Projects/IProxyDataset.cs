﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Activation;
using Apollo.Utilities;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for objects that store information about datasets.
    /// </summary>
    internal interface IProxyDataset : IDatasetOfflineInformation, IEquatable<IProxyDataset>
    {
        /// <summary>
        /// Gets a value indicating whether the current object is valid. 
        /// </summary>
        /// <remarks>
        /// The object can become invalid when:
        /// <list type="bullet">
        /// <item>The project is closed.</item>
        /// <item>The dataset is deleted.</item>
        /// </list>
        /// </remarks>
        bool IsValid
        {
            get;
        }

        /// <summary>
        /// Deletes the current dataset and all its children.
        /// </summary>
        void Delete();

        /// <summary>
        /// Gets a value indicating whether the dataset is activated.
        /// </summary>
        bool IsActivated
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the dataset can be activated.
        /// </summary>
        bool CanActivate
        {
            get;
        }

        /// <summary>
        /// Returns the machine on which the dataset is running.
        /// </summary>
        /// <returns>
        /// The machine on which the dataset is running.
        /// </returns>
        NetworkIdentifier RunsOn();

        /// <summary>
        /// Activates the dataset in an asynchronous manner.
        /// </summary>
        /// <param name="preferredLocation">
        /// Indicates a preferred machine location for the dataset to be activated onto.
        /// </param>
        /// <param name="machineSelector">
        ///     The function that selects the most suitable machine for the dataset to run on.
        /// </param>
        /// <param name="token">The token that is used to cancel the activation.</param>
        /// <remarks>
        /// Note that the <paramref name="preferredLocation"/> is
        /// only a suggestion. The activator may decide to ignore the suggestion if there is a distribution
        /// plan that is better suited to the contents of the dataset.
        /// </remarks>
        /// <returns>
        ///     A <see cref="Task"/> that completes when the dataset is activated.
        /// </returns>
        Task Activate(
            DistributionLocations preferredLocation,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
            CancellationToken token);

        /// <summary>
        /// Deactivates the dataset.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> that completes when the dataset is deactivated.
        /// </returns>
        Task Deactivate();

        /// <summary>
        /// Returns the collection of sub-datasets.
        /// </summary>
        /// <returns>
        /// The collection of sub-datasets.
        /// </returns>
        IEnumerable<IProxyDataset> Children();

        /// <summary>
        /// Creates a new child dataset and returns the ID number of the child.
        /// </summary>
        /// <param name="newChild">The information required to create the new child.</param>
        /// <returns>
        /// The newly created child.
        /// </returns>
        IProxyDataset CreateNewChild(DatasetCreationInformation newChild);

        /// <summary>
        /// Creates a set of child datasets and returns a collection containing the ID numbers
        /// of the newly created children.
        /// </summary>
        /// <param name="newChildren">The collection containing the information to create the child datasets.</param>
        /// <returns>
        /// A collection containing the newly created children.
        /// </returns>
        IEnumerable<IProxyDataset> CreateNewChildren(IEnumerable<DatasetCreationInformation> newChildren);

        /// <summary>
        /// Gets a value indicating whether the dataset is in edit mode or not.
        /// </summary>
        bool IsEditMode
        {
            get;
        }

        /// <summary>
        /// Switches the dataset to edit mode.
        /// </summary>
        void SwitchToEditMode();

        /// <summary>
        /// Switches the dataset to executing mode.
        /// </summary>
        void SwitchToExecutingMode();

        /// <summary>
        /// An event raised when the dataset is deleted.
        /// </summary>
        event EventHandler<EventArgs> OnDeleted;

        /// <summary>
        /// An event raised when the name of a dataset is changed.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<string>> OnNameChanged;

        /// <summary>
        /// An event raised when the summary of a dataset is changed.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<string>> OnSummaryChanged;

        /// <summary>
        /// An event raised when there is progress in the current action which is being
        /// executed by the dataset.
        /// </summary>
        event EventHandler<ProgressEventArgs> OnProgressOfCurrentAction;

        /// <summary>
        /// An event raised when the dataset is activated.
        /// </summary>
        event EventHandler<EventArgs> OnActivated;

        /// <summary>
        /// An event raised when the dataset is deactivated.
        /// </summary>
        event EventHandler<EventArgs> OnDeactivated;

        /// <summary>
        /// An event fired when the dataset is switched to edit mode.
        /// </summary>
        event EventHandler<EventArgs> OnSwitchToEditMode;

        /// <summary>
        /// An event fired when the dataset is switched to executing mode.
        /// </summary>
        event EventHandler<EventArgs> OnSwitchToExecutingMode;

        /// <summary>
        /// Gets the object that provides access to the data stored in the dataset.
        /// </summary>
        DatasetStorageProxy Data
        {
            get;
        }
    }
}
