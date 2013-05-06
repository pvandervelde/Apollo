//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Lokad;
using Lokad.Rules;
using Nuclei;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Defines a name for a notification event.
    /// </summary>
    [DebuggerDisplay("Notification with ID: [{InternalValue}]")]
    public sealed class NotificationName : Id<NotificationName, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationName"/> class.
        /// </summary>
        /// <param name="name">The name of the notification.</param>
        public NotificationName(string name)
            : base(name)
        {
            {
                Enforce.Argument(() => name);
                Enforce.Argument(() => name, StringIs.NotEmpty);
            }
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override NotificationName Clone(string value)
        {
            return new NotificationName(value);
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
        protected override int CompareValues(string ourValue, string theirValue)
        {
            return string.Compare(ourValue, theirValue, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Notification with name {0}", InternalValue);
        }
    }
}
