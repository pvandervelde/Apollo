﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Projects;
using Lokad;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Stores event arguments used when a dataset unload is complete.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Args",
        Justification = "This is an EventArgs derivative class.")]
    public sealed class DatasetUnloadEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetUnloadEventArgs"/> class.
        /// </summary>
        /// <param name="id">The ID number of the dataset that was unloaded.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        public DatasetUnloadEventArgs(DatasetId id)
        {
            {
                Enforce.Argument(() => id);
            }

            Id = id;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the dataset that was unloaded.
        /// </summary>
        public DatasetId Id
        {
            get;
            private set;
        }
    }
}
