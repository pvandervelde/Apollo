//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Apollo.Core.Base;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for objects that own one or more datasets.
    /// </summary>
    internal interface IDatasetOwner
    {
        /// <summary>
        /// Creates a new datset as child of the given parent dataset.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="newChild">The information required to create the new child.</param>
        /// <returns>The new child.</returns>
        IProxyDataset CreateDataset(DatasetId parent, DatasetCreationInformation newChild);

        /// <summary>
        /// Deletes the given dataset and all its children.
        /// </summary>
        /// <remarks>
        /// The dataset and all its children will be deleted. If any of the datasets are
        /// loaded onto a (remote) machine they will be unloaded just before being deleted.
        /// No data will be saved.
        /// </remarks>
        /// <param name="dataset">The dataset that should be deleted.</param>
        /// <exception cref="CannotDeleteDatasetException">
        /// Thrown when the dataset or one of its children cannot be deleted. The exception
        /// is thrown before any of the datasets are deleted.
        /// </exception>
        void DeleteDatasetAndChildren(DatasetId dataset);

        /// <summary>
        /// Returns the collection of children for a given dataset.
        /// </summary>
        /// <param name="parent">The ID number of the parent dataset.</param>
        /// <returns>The collection of child datsets.</returns>
        IEnumerable<DatasetProxy> Children(DatasetId parent);
    }
}
