//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Nuclei;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// Defines an ID number for dataset locks.
    /// </summary>
    /// <remarks>
    /// This token uses integers internally. We don't expect to
    /// have enough uploads in a single execution of an application
    /// for integer overflow to occur.
    /// </remarks>
    internal sealed class DatasetLockKey : Id<DatasetLockKey, int>
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
        /// Initializes a new instance of the <see cref="DatasetLockKey"/> class.
        /// </summary>
        public DatasetLockKey()
            : this(NextIdValue())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetLockKey"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Must be larger than -1.</param>
        internal DatasetLockKey(int id)
            : base(id)
        {
            Debug.Assert(id > InvalidId, "The key should not be invalid"); 
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override DatasetLockKey Clone(int value)
        {
            var result = new DatasetLockKey(value);
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
                "Dataset lock key: [{0}]",
                InternalValue);
        }
    }
}
