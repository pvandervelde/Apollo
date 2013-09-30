//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using Apollo.Core.Base;
using Apollo.Core.Base.Activation;
using Apollo.Utilities;

namespace Apollo.Core.Scripting.Projects
{
    /// <summary>
    /// Defines the interface for objects that form a facade of a dataset for the
    /// scripting API.
    /// </summary>
    public interface IDatasetScriptFacade
    {
        /// <summary>
        /// Gets or sets a value indicating the name of the dataset.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// An event raised when the name of the dataset is updated.
        /// </summary>
        event EventHandler<EventArgs> OnNameChanged;

        /// <summary>
        /// Gets or sets a value describing the dataset.
        /// </summary>
        string Summary
        {
            get;
            set;
        }

        /// <summary>
        /// An event raised when the summary of the dataset is updated.
        /// </summary>
        event EventHandler<EventArgs> OnSummaryChanged;

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
        /// An event fired if the current dataset becomes invalid.
        /// </summary>
        event EventHandler<EventArgs> OnInvalidate;

        /// <summary>
        /// Gets a value indicating whether the new dataset can be deleted from the
        /// project.
        /// </summary>
        bool CanBeDeleted
        {
            get;
        }

        /// <summary>
        /// Removes the current dataset and it's children from the project.
        /// </summary>
        void Delete();

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
        /// Gets a value indicating whether the dataset is loaded on the local machine
        /// or a remote machine.
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
        /// Activates the dataset.
        /// </summary>
        /// <param name="preferredLocation">
        /// Indicates a preferred machine location for the dataset to be distributed to.
        /// </param>
        /// <param name="machineSelector">
        ///     The function that selects the most suitable machine for the dataset to run on.
        /// </param>
        /// <param name="token">The token that is used to cancel the activation.</param>
        /// <remarks>
        /// Note that the <paramref name="preferredLocation"/> is
        /// only a suggestion. The loader may decide to ignore the suggestion if there is a distribution
        /// plan that is better suited to the contents of the dataset.
        /// </remarks>
        void Activate(
            DistributionLocations preferredLocation,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
            CancellationToken token);

        /// <summary>
        /// Deactivates the dataset.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// An event raised when there is progress in the action that the dataset is
        /// currently executing.
        /// </summary>
        event EventHandler<ProgressEventArgs> OnProgressOfCurrentAction;

        /// <summary>
        /// An event fired after the dataset has been activated.
        /// </summary>
        event EventHandler<EventArgs> OnActivated;

        /// <summary>
        /// An event fired after the dataset has been deactivated.
        /// </summary>
        event EventHandler<EventArgs> OnDeactivated;

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
        /// Returns the collection containing the direct children of the 
        /// current dataset.
        /// </summary>
        /// <returns>The collection that contains the direct children of the current dataset.</returns>
        IEnumerable<IDatasetScriptFacade> Children();

        /// <summary>
        /// Adds a new child.
        /// </summary>
        /// <returns>
        /// The newly created dataset.
        /// </returns>
        IDatasetScriptFacade AddChild();

        /// <summary>
        /// Adds a new child.
        /// </summary>
        /// <param name="filePath">The path to the file that stores the dataset that should be copied.</param>
        /// <returns>
        /// The newly created dataset.
        /// </returns>
        IDatasetScriptFacade AddChild(string filePath);
    }
}
