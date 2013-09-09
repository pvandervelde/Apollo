//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Describes the expected load a dataset will put on a given machine.
    /// </summary>
    [Serializable]
    public sealed class ExpectedDatasetLoad
    {
        /// <summary>
        /// Gets or sets a value indicating the size of the dataset in bytes while
        /// stored on disk.
        /// </summary>
        public long OnDiskSizeInBytes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the size of the dataset in bytes while
        /// loaded into memory.
        /// </summary>
        public long InMemorySizeInBytes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the expansion of the memory demands
        /// relative to the loaded size when the dataset is running calculations.
        /// </summary>
        public double RelativeMemoryExpansionWhileRunning
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the expansion of the disk space demands
        /// relative to the original size if the dataset is stored after running calculations.
        /// </summary>
        public double RelativeOnDiskExpansionAfterRunning
        {
            get;
            set;
        }
    }
}
