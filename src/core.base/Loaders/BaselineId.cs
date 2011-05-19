//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Apollo.Utilities;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines an ID number for baseline calculations.
    /// </summary>
    /// <remarks>
    /// A baseline calculation is a calculation which consists of a known
    /// set of elements. By comparing a new calculation to the baseline it
    /// is possible to establish a rough estimate for the demands placed on
    /// a machine by the unknown calculation.
    /// </remarks>
    [Serializable]
    public sealed class BaselineId : Id<BaselineId, int>
    {
        /// <summary>
        /// Defines the ID number for an unknown baseline.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int UnknownBaselineId = -1;

        /// <summary>
        /// Gets a value indicating the ID number for an unknown baseline.
        /// </summary>
        public static BaselineId Unknown
        {
            get
            {
                return new BaselineId(UnknownBaselineId);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineId"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Any value smaller than 0 indicates that the ID points to an unknown baseline.</param>
        public BaselineId(int id)
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
        protected override BaselineId Clone(int value)
        {
            return new BaselineId(value);
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
                "BaseLine ID: [{0}]",
                InternalValue);
        }
    }
}
