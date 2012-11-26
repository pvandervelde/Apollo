//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Apollo.Utilities;

namespace Apollo.Core.Dataset.Plugins
{
    /// <summary>
    /// Defines an ID number for a part that is included in a composition graph.
    /// </summary>
    /// <design>
    /// <para>
    /// Graph ID objects are never re-used, even after groups are removed from the graph.
    /// </para>
    /// </design>
    [Serializable]
    internal sealed class PartCompositionId : Id<PartCompositionId, Guid>
    {
        /// <summary>
        /// Defines the ID number for an invalid dataset ID.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Guid s_InvalidId = new Guid("{9C5C17FA-53A1-44C2-AB65-2F155D249081}");

        /// <summary>
        /// Returns the next integer that can be used for an ID number.
        /// </summary>
        /// <returns>
        /// The next unused ID value.
        /// </returns>
        private static Guid NextIdValue()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartCompositionId"/> class.
        /// </summary>
        public PartCompositionId()
            : this(NextIdValue())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartCompositionId"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Must be larger than -1.</param>
        internal PartCompositionId(Guid id)
            : base(id)
        {
            Debug.Assert(id != s_InvalidId, "The ID number should not be invalid"); 
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override PartCompositionId Clone(Guid value)
        {
            var result = new PartCompositionId(value);
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
                "PartCompositionId: [{0}]",
                InternalValue);
        }
    }
}
