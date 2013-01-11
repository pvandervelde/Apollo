//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Apollo.Core.Extensions.Properties;
using Apollo.Utilities;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines an ID for an element in a schedule.
    /// </summary>
    [Serializable]
    public sealed class ScheduleElementId : Id<ScheduleElementId, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleElementId"/> class.
        /// </summary>
        public ScheduleElementId()
            : this(Guid.NewGuid())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleElementId"/> class.
        /// </summary>
        /// <param name="id">The ID number of the schedule.</param>
        /// <exception cref="CannotCreateScheduleElementIdWithEmptyGuidException">
        /// Thrown when <paramref name="id"/> is equal to <see cref="Guid.Empty"/>.
        /// </exception>
        public ScheduleElementId(Guid id)
            : base(id)
        {
            {
                Lokad.Enforce.With<CannotCreateScheduleElementIdWithEmptyGuidException>(
                    !Guid.Equals(id, Guid.Empty), 
                    Resources.Exceptions_Messages_CannotCreateScheduleElementIdWithEmptyGuid);
            }
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override ScheduleElementId Clone(Guid value)
        {
            return new ScheduleElementId(value);
        }

        /// <summary>
        /// Compares the values.
        /// </summary>
        /// <param name="ourValue">The value of the current object.</param>
        /// <param name="theirValue">The value of the object with which the current object is being compared.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// <paramref name="ourValue"/> is less than <paramref name="theirValue"/>.
        /// Zero
        /// <paramref name="ourValue"/> is equal to <paramref name="theirValue"/>.
        /// Greater than zero
        /// <paramref name="ourValue"/> is greater than <paramref name="theirValue"/>.
        /// </returns>
        protected override int CompareValues(Guid ourValue, Guid theirValue)
        {
            return ourValue.CompareTo(theirValue);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Schedule action [{0}]", InternalValue);
        }
    }
}
