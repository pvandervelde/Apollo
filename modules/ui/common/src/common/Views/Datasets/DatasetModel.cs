//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Projects;
using Apollo.Core.UserInterfaces.Project;
using Lokad;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// Defines the viewmodel for a dataset.
    /// </summary>
    public sealed class DatasetModel : Model
    {
        /// <summary>
        /// The dataset that holds the actual data.
        /// </summary>
        private readonly DatasetFacade m_Dataset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetModel"/> class.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataset"/> is <see langword="null" />.
        /// </exception>
        public DatasetModel(DatasetFacade dataset)
        {
            {
                Enforce.Argument(() => dataset);
            }

            m_Dataset = dataset;
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
            get
            {
                return m_Dataset.CanBeAdopted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be copied to another
        /// dataset.
        /// </summary>
        public bool CanBeCopied
        {
            get
            {
                return m_Dataset.CanBeCopied;
            }
        }

        /// <summary>
        /// Gets a value indicating who created the dataset.
        /// </summary>
        public DatasetCreator CreatedBy
        {
            get
            {
                return m_Dataset.CreatedBy;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is loaded on the local machine
        /// or a remote machine.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return m_Dataset.IsLoaded;
            }
        }
    }
}
