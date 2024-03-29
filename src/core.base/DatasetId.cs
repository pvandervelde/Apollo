﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Nuclei;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines an ID number for datasets.
    /// </summary>
    /// <remarks>
    /// This ID number uses integers internally. We don't expect to
    /// have enough datasets in a single project for integer overflow to occur.
    /// </remarks>
    /// <design>
    /// <para>
    /// The <c>DatasetId</c> class stores an ID number for a dataset. The internal
    /// data is an integer which indicates the sequential number of the ID. The way
    /// this is implemented means that ID numbers are only sequential inside a single
    /// application. This means that we should always get the ID number of the dataset
    /// from the same location. This should normally not be a big problem given that
    /// only the project should generate new datasets.
    /// </para>
    /// </design>
    [Serializable]
    [DebuggerDisplay("Dataset: [{InternalValue}]")]
    public sealed class DatasetId : Id<DatasetId, int>
    {
        /// <summary>
        /// Defines the ID number for an invalid dataset ID.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int InvalidId = -1;

        /// <summary>
        /// The value of the last ID number.
        /// </summary>
        private static int s_LastId = 0;

        /// <summary>
        /// Returns the next integer that can be used for an ID number.
        /// </summary>
        /// <returns>
        /// The next unused ID value.
        /// </returns>
        private static int NextIdValue()
        {
            var current = Interlocked.Increment(ref s_LastId);
            return current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetId"/> class.
        /// </summary>
        public DatasetId()
            : this(NextIdValue())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetId"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Must be larger than -1.</param>
        internal DatasetId(int id)
            : base(id)
        {
            Debug.Assert(id > InvalidId, "The ID number should not be invalid"); 
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override DatasetId Clone(int value)
        {
            var result = new DatasetId(value);
            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "DatasetId: [{0}]",
                InternalValue);
        }
    }
}
