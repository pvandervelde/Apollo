//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Apollo.Utils;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines an ID number for base line calculations.
    /// </summary>
    /// <remarks>
    /// A base line calculation is a calculation which consists of a known
    /// set of elements. By comparing a new calculation to the base line it
    /// is possible to establish a rough estimate for the demands placed on
    /// a machine by the unknown calculation.
    /// </remarks>
    [Serializable]
    public sealed class BaseLineId : Id<BaseLineId, int>
    {
        /// <summary>
        /// Defines the ID number for an unknown base line.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int UnknownBaseLineId = -1;

        /// <summary>
        /// Gets a value indicating the ID number for an unknown base line.
        /// </summary>
        public static BaseLineId Unknown
        {
            get
            {
                return new BaseLineId(UnknownBaseLineId);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseLineId"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Any value smaller than 0 indicates that the ID points to an unknown base line.</param>
        public BaseLineId(int id)
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
        protected override BaseLineId Clone(int value)
        {
            return new BaseLineId(value);
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
