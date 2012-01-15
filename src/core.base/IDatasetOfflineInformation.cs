//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utilities;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines the interface for objects that store offline information about a dataset.
    /// </summary>
    public interface IDatasetOfflineInformation
    {
        /// <summary>
        /// Gets a value indicating the ID number of the dataset for 
        /// which the persistence information is stored.
        /// </summary>
        DatasetId Id
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
        /// Gets a value indicating whether the new dataset can be deleted from the
        /// project.
        /// </summary>
        bool CanBeDeleted
        {
            get;
        }

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
        /// Gets a value indicating where and how the dataset was persisted.
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
    }
}
