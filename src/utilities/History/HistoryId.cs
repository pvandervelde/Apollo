//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Threading;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines a unique ID for an object that stores its history in a timeline.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>HistoryId</c> class uses <see cref="long"/> values internally to uniquely identify
    /// different object histories. We don't expect to ever overflow it given that just iterating
    /// over a long from start to end takes (on average) about 500+ years on an intel i7 920 (in
    /// debug mode). We don't expect to have that many objects signing up for the timeline.
    /// </para>
    /// <para>
    /// Also note that timeline data is reset when we open a new project or shut down the
    /// application.
    /// </para>
    /// </remarks>
    public sealed class HistoryId : Id<HistoryId, long>
    {
        /// <summary>
        /// The none ID.
        /// </summary>
        private const long NoneIdValue = long.MinValue;

        /// <summary>
        /// The value of the last ID number.
        /// </summary>
        private static long s_LastIdValue = long.MinValue;

        /// <summary>
        /// Gets the none ID.
        /// </summary>
        public static HistoryId None
        {
            get
            {
                return new HistoryId(NoneIdValue);
            }
        }

        /// <summary>
        /// Returns the next integer that can be used for an ID number.
        /// </summary>
        /// <returns>
        /// The next unused ID value.
        /// </returns>
        private static long GetNextValue()
        {
            var current = Interlocked.Increment(ref s_LastIdValue);
            return current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryId"/> class.
        /// </summary>
        public HistoryId()
            : this(GetNextValue())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryId"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Must be larger than <see cref="long.MinValue"/>.</param>
        internal HistoryId(long id)
            : base(id)
        {
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override HistoryId Clone(long value)
        {
            var result = new HistoryId(value);
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
                "HistoryId: [{0}]",
                InternalValue);
        }
    }
}
