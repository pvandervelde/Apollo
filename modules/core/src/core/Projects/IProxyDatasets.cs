//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base.Projects;
using Apollo.Utils;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Defines the interface for objects that store information about datasets.
    /// </summary>
    internal interface IProxyDatasets : IEquatable<IProxyDatasets>
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
        /// Gets or sets a value describing the dataset.
        /// </summary>
        string Summary
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is loaded on the local machine
        /// or a remote machine.
        /// </summary>
        bool IsLoaded
        {
            get;
        }

        /// <summary>
        /// Returns a collection containing information on all the machines
        /// the dataset is distributed over.
        /// </summary>
        /// <returns>
        /// A collection containing information about all the machines the
        /// dataset is distributed over.
        /// </returns>
        IEnumerable<Machine> RunsOn();

        /// <summary>
        /// Loads the dataset onto a machine.
        /// </summary>
        /// <param name="preferredLocation">
        /// Indicates a preferred machine location for the dataset to be loaded onto.
        /// </param>
        /// <param name="range">
        /// The number of machines over which the data set should be distributed.
        /// </param>
        /// <remarks>
        /// Note that the <paramref name="preferredLocation"/> and the <paramref name="range"/> are
        /// only suggestions. The loader may deside to ignore the suggestions if there is a distribution
        /// plan that is better suited to the contents of the dataset.
        /// </remarks>
        void LoadOntoMachine(LoadingLocation preferredLocation, MachineDistributionRange range);

        /// <summary>
        /// Unloads the dataset from the machine it is currently loaded onto.
        /// </summary>
        void UnloadFromMachine();

        /// <summary>
        /// Returns the collection of sub-datasets.
        /// </summary>
        /// <returns>
        /// The collection of sub-datasets.
        /// </returns>
        IEnumerable<IProxyDatasets> Children();

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
        IProxyDatasets CreateNewChild(DatasetCreationInformation newChild);

        /// <summary>
        /// Creates a set of child datasets and returns a collection containing the ID numbers
        /// of the newly created children.
        /// </summary>
        /// <param name="newChildren">The collection containing the information to create the child datasets.</param>
        /// <returns>
        /// A collection containing the newly created children.
        /// </returns>
        IEnumerable<IProxyDatasets> CreateNewChildren(IEnumerable<DatasetCreationInformation> newChildren);

        /// <summary>
        /// Gets a value indicating the set of commands that apply to the current dataset.
        /// </summary>
        ProxyCommandSet Commands
        {
            get;
        }

        /// <summary>
        /// Registers the given object for change notifications.
        /// </summary>
        /// <param name="toBeNotified">The object that wants to receive change notifications.</param>
        void RegisterForEvents(INotifyOnDatasetChange toBeNotified);

        /// <summary>
        /// Unregisters the given object for change notifications.
        /// </summary>
        /// <param name="toBeNotified">The object that is registered for change notifications.</param>
        void UnregisterForEvents(INotifyOnDatasetChange toBeNotified);
    }
}
