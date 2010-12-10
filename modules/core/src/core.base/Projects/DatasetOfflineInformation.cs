//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Projects
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
        private readonly DatasetId m_Id;

        /// <summary>
        /// Describes for what purpose the dataset was created.
        /// </summary>
        private DatasetCreationReason m_ConstructionReason;

        /// <summary>
        /// The object that describes how the dataset was persisted.
        /// </summary>
        private IPersistenceInformation m_LoadFrom;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetOfflineInformation"/> class.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <param name="constructionReason">The object describing why the dataset was created.</param>
        /// <param name="loadFrom">The object that stores information describing how and where the dataset is persisted.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="constructionReason"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="loadFrom"/> is <see langword="null" />.
        /// </exception>
        public DatasetOfflineInformation(DatasetId id, DatasetCreationReason constructionReason, IPersistenceInformation loadFrom)
        {
            {
                Enforce.Argument(() => id);
                Enforce.Argument(() => constructionReason);
                Enforce.Argument(() => loadFrom);
            }

            m_Id = id;
            m_ConstructionReason = constructionReason;
            m_LoadFrom = loadFrom;
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
        /// Gets a value indicating why the dataset was created and by whom.
        /// </summary>
        public DatasetCreationReason ReasonForExistence
        {
            get
            {
                return m_ConstructionReason;
            }
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

        // Meta data?
        // - Loaded size?
    }
}
