//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for objects that store information about datasets.
    /// </summary>
    internal interface IProxyDataset : IEquatable<IProxyDataset>
    {
        /// <summary>
        /// Gets a value indicating the ID number of the dataset.
        /// </summary>
        DatasetId Id
        { 
            get; 
        }

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
        /// Gets a value indicating whether the new dataset can be deleted from the
        /// project.
        /// </summary>
        bool CanBeDeleted
        {
            get;
        }

        /// <summary>
        /// Deletes the current dataset and all its children.
        /// </summary>
        void Delete();

        /// <summary>
        /// An event raised when the dataset is deleted.
        /// </summary>
        event EventHandler<EventArgs> OnDeleted;

        /// <summary>
        /// Gets a value indicating whether the new dataset can be moved from one parent
        /// to another parent.
        /// </summary>
        /// <design>
        /// Datasets created by the user are normally movable. Datasets created by the system
        /// may not be movable because it doesn't make sense to move a dataset whose only purpose
        /// is to provide information to the parent.
        /// </design>
        bool CanBeAdopted
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be copied to another
        /// dataset.
        /// </summary>
        bool CanBeCopied
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating who created the dataset.
        /// </summary>
        DatasetCreator CreatedBy
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating from where the datset is or will be 
        /// loaded.
        /// </summary>
        IPersistenceInformation StoredAt
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating the name of the dataset.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// An event raised when the name of a dataset is changed.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<string>> OnNameChanged;

        /// <summary>
        /// Gets or sets a value describing the dataset.
        /// </summary>
        string Summary
        {
            get;
            set;
        }

        /// <summary>
        /// An event raised when the summary of a dataset is changed.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<string>> OnSummaryChanged;

        /// <summary>
        /// Gets a value indicating whether the dataset is loaded on the local machine
        /// or a remote machine.
        /// </summary>
        bool IsLoaded
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the dataset can be loaded onto a machine.
        /// </summary>
        bool CanLoad
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
        /// Loads the dataset onto a machine.
        /// </summary>
        /// <param name="preferredLocation">
        /// Indicates a preferred machine location for the dataset to be loaded onto.
        /// </param>
        /// <param name="machineSelector">
        ///     The function that selects the most suitable machine for the dataset to run on.
        /// </param>
        /// <param name="token">The token that is used to cancel the loading.</param>
        /// <remarks>
        /// Note that the <paramref name="preferredLocation"/> is
        /// only a suggestion. The loader may deside to ignore the suggestion if there is a distribution
        /// plan that is better suited to the contents of the dataset.
        /// </remarks>
        void LoadOntoMachine(
            LoadingLocations preferredLocation,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
            CancellationToken token);

        /// <summary>
        /// An event raised when there is progress in the current action which is being
        /// executed by the dataset.
        /// </summary>
        event EventHandler<ProgressEventArgs> OnProgressOfCurrentAction;

        /// <summary>
        /// An event raised when the dataset is loaded onto one or more machines.
        /// </summary>
        event EventHandler<EventArgs> OnLoaded;

        /// <summary>
        /// Unloads the dataset from the machine it is currently loaded onto.
        /// </summary>
        void UnloadFromMachine();

        /// <summary>
        /// An event raised when the dataset is unloaded from the machines it was loaded onto.
        /// </summary>
        event EventHandler<EventArgs> OnUnloaded;

        /// <summary>
        /// Returns the collection of sub-datasets.
        /// </summary>
        /// <returns>
        /// The collection of sub-datasets.
        /// </returns>
        IEnumerable<IProxyDataset> Children();

        /// <summary>
        /// Gets a value indicating whether the current dataset is allowed to request the 
        /// creation of its own children.
        /// </summary>
        /// <design>
        /// Normally all datasets created by the user are allowed to create their own 
        /// children. In some cases datasets created by the system are blocked from 
        /// creating their own children.
        /// </design>
        bool CanBecomeParent
        {
            get;
        }

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
        /// An event fired when the dataset is switched to edit mode.
        /// </summary>
        event EventHandler<EventArgs> OnSwitchToEditMode;

        /// <summary>
        /// An event fired when the dataset is switched to executing mode.
        /// </summary>
        event EventHandler<EventArgs> OnSwitchToExecutingMode;

        /// <summary>
        /// Gets a value indicating the set of commands that apply to the current dataset.
        /// </summary>
        IProxyCommandSet Commands
        {
            get;
        }
    }
}
