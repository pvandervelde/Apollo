//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Lokad;
using Lokad.Rules;

namespace Apollo.Utils.Commands
{
    /// <summary>
    /// Defines the ID of a <see cref="ICommand"/> object.
    /// </summary>
    [Serializable]
    public sealed class CommandId : Id<CommandId, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandId"/> class.
        /// </summary>
        /// <param name="name">The ID of the command.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="name"/> is an empty string.
        /// </exception>
        public CommandId(string name)
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
        protected override CommandId Clone(string value)
        {
            return new CommandId(value);
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
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, @"CommandId: {0}", InternalValue);
        }
    }
}
