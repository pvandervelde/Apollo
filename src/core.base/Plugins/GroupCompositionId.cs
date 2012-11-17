//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Apollo.Utilities;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Defines an ID number for a part group that is included in a composition graph.
    /// </summary>
    /// <remarks>
    /// This token uses integers internally. We don't expect to
    /// have enough composed groups in a single execution of an application
    /// for integer overflow to occur.
    /// </remarks>
    /// <design>
    /// <para>
    /// The <c>GroupCompositionId</c> class stores a token for a part group which is included in 
    /// a composition graph. The internal data is an integer which indicates the sequential order in
    /// which the group was inserted in the graph.
    /// </para>
    /// <para>
    /// Graph ID objects are never re-used, even after groups are removed from the graph.
    /// </para>
    /// </design>
    [Serializable]
    public sealed class GroupCompositionId : Id<GroupCompositionId, int>
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
        /// Initializes a new instance of the <see cref="GroupCompositionId"/> class.
        /// </summary>
        public GroupCompositionId()
            : this(NextIdValue())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCompositionId"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Must be larger than -1.</param>
        internal GroupCompositionId(int id)
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
        protected override GroupCompositionId Clone(int value)
        {
            var result = new GroupCompositionId(value);
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
                "GroupCompositionId: [{0}]",
                InternalValue);
        }
    }
}
