//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Projects
{
    /// <summary>
    /// Defines the reason for the creation of a dataset.
    /// </summary>
    public sealed class DatasetCreationReason
    {
        // The reason for creation defines (in a roundabout way)
        // - If a dataset is visible to the user (system datasets normally aren't)
        // - If a dataset can be deleted (system datasets normally can't, they only get thrown away when the parent gets deleted).

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetCreationReason"/> class.
        /// </summary>
        /// <param name="creationInformation">
        /// The object that holds information about the reasons for creation of the dataset.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="creationInformation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="CannotCreateDatasetWithoutCreatorException">
        ///     Thrown if <paramref name="creationInformation"/> specifies that the creator is <see cref="DatasetCreator.None"/>.
        /// </exception>
        public DatasetCreationReason(DatasetCreationInformation creationInformation)
        {
            {
                Enforce.Argument(() => creationInformation);
                Enforce.With<CannotCreateDatasetWithoutCreatorException>(
                    creationInformation.CreatedOnRequestOf != DatasetCreator.None, 
                    Resources.Exceptions_Messages_CannotCreateDatasetWithoutCreator);
            }

            CreatedBy = creationInformation.CreatedOnRequestOf;
            CanBecomeParent = creationInformation.CanBecomeParent;
            CanBeAdopted = creationInformation.CanBeAdopted;
            CanBeCopied = creationInformation.CanBeCopied;
            CanBeDeleted = creationInformation.CanBeDeleted;
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

        // Owner (parent / user / system (root))
        // Can be deleted?
    }
}
