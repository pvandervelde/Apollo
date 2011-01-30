//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Base.Projects;
using Apollo.Core.Properties;
using Lokad;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Stores event arguments used when a dataset load is complete.
    /// </summary>
    public sealed class DatasetLoadEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetLoadEventArgs"/> class.
        /// </summary>
        /// <param name="id">The ID number of the dataset that was loaded.</param>
        /// <param name="machines">The collection of machines over which the dataset is distributed.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="machines"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="machines"/> is empty.
        /// </exception>
        public DatasetLoadEventArgs(DatasetId id, IEnumerable<Machine> machines)
        {
            {
                Enforce.Argument(() => id);
                Enforce.Argument(() => machines);
                Enforce.With<ArgumentException>(machines.Any(), Resources_NonTranslatable.Exception_Messages_CannotLoadDatasetOnNoMachines);
            }

            Id = id;
            LoadedOn = machines;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the dataset that was loaded.
        /// </summary>
        public DatasetId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the set of machines over which the dataset is
        /// distributed.
        /// </summary>
        public IEnumerable<Machine> LoadedOn
        {
            get;
            private set;
        }
    }
}
