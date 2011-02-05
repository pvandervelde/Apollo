//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Properties;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Stores persistent information about a dataset. This information will be loaded
    /// even if the dataset isn't loaded.
    /// </summary>
    public sealed class DatasetOfflineInformation
    {
        /// <summary>
        /// The ID number of the dataset.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "mId",
            Justification = "Eh duh.")]
        private readonly DatasetId m_Id;

        /// <summary>
        /// The object that describes how the dataset was persisted.
        /// </summary>
        private IPersistenceInformation m_LoadFrom;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetOfflineInformation"/> class.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <param name="constructionReason">The object describing why the dataset was created.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="constructionReason"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="CannotCreateDatasetWithoutCreatorException">
        ///     Thrown if <paramref name="constructionReason"/> defines a creator as <see cref="DatasetCreator.None"/>.
        /// </exception>
        public DatasetOfflineInformation(DatasetId id, DatasetCreationInformation constructionReason)
        {
            {
                Enforce.Argument(() => id);
                Enforce.Argument(() => constructionReason);
                Enforce.With<CannotCreateDatasetWithoutCreatorException>(
                    constructionReason.CreatedOnRequestOf != DatasetCreator.None,
                    Resources.Exceptions_Messages_CannotCreateDatasetWithoutCreator);
            }

            m_Id = id;
            CreatedBy = constructionReason.CreatedOnRequestOf;
            CanBecomeParent = constructionReason.CanBecomeParent;
            CanBeAdopted = constructionReason.CanBeAdopted;
            CanBeCopied = constructionReason.CanBeCopied;
            CanBeDeleted = constructionReason.CanBeDeleted;
            m_LoadFrom = constructionReason.LoadFrom;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the dataset for 
        /// which the persistence information is stored.
        /// </summary>
        public DatasetId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets a value indicating who created the dataset.
        /// </summary>
        public DatasetCreator CreatedBy
        {
            get;
            private set;
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
        public bool CanBecomeParent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be deleted from the
        /// project.
        /// </summary>
        public bool CanBeDeleted
        {
            get;
            private set;
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
        public bool CanBeAdopted
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be copied to another
        /// dataset.
        /// </summary>
        public bool CanBeCopied
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating where and how the dataset was persisted.
        /// </summary>
        public IPersistenceInformation StoredAt
        {
            get
            {
                return m_LoadFrom;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the name of the dataset.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value describing the dataset.
        /// </summary>
        public string Summary
        {
            get;
            set;
        }

        // Meta data?
        // - Loaded size?
    }
}
