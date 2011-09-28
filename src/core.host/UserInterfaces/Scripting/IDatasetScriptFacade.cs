//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;

namespace Apollo.Core.Host.UserInterfaces.Scripting
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
        bool IsLoaded
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
        /// An event fired after the dataset has been distributed to one or more machines.
        /// </summary>
        event EventHandler<EventArgs> OnLoaded;

        /// <summary>
        /// An event fired after the dataset has been unloaded from the machines it was loaded onto.
        /// </summary>
        event EventHandler<EventArgs> OnUnloaded;

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
        /// The newly created datset.
        /// </returns>
        IDatasetScriptFacade AddChild();

        /// <summary>
        /// Adds a new child.
        /// </summary>
        /// <param name="filePath">The path to the file that stores the dataset that should be copied.</param>
        /// <returns>
        /// The newly created datset.
        /// </returns>
        IDatasetScriptFacade AddChild(string filePath);
    }
}
