//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Stores information used in the creation of a new dataset.
    /// </summary>
    [Serializable]
    [ExcludeFromCoverage("This class only has compiler generated setting methods.")]
    public sealed class DatasetCreationInformation
    {
        /// <summary>
        /// Gets or sets a value indicating where the new dataset can load its data from.
        /// </summary>
        public IPersistenceInformation LoadFrom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicated who requested the creation of the dataset.
        /// </summary>
        public DatasetCreator CreatedOnRequestOf
        {
            get;
            set;
        }

        // - Why it was created (User specified reason?

        /// <summary>
        /// Gets or sets a value indicating whether the new dataset is allowed to request the 
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
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the new dataset can be deleted from the
        /// project.
        /// </summary>
        public bool CanBeDeleted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the new dataset can be moved from one parent
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
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the new dataset can be copied to another
        /// dataset.
        /// </summary>
        public bool CanBeCopied
        {
            get;
            set;
        }
    }
}
