﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Apollo.Utils;
using Lokad;
using Lokad.Rules;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// Defines the name of a service.
    /// </summary>
    /// <todo>
    /// This class should probably be renamed.
    /// </todo>
    [Serializable]
    public sealed class DnsName : Id<DnsName, string>
    {
        /// <summary>
        /// The unique identifier for none of the services.
        /// </summary>
        private static readonly DnsName s_Nobody = new DnsName(@"Nobody");

        /// <summary>
        /// Gets the <c>DnsName</c> for none of the services.
        /// Any messages send with this <c>DnsName</c> as recipient will
        /// be removed by the message pipeline.
        /// </summary>
        /// <value>The <c>DnsName</c> for none of the services.</value>
        public static DnsName Nobody
        {
            get 
            {
                return s_Nobody;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsName"/> class.
        /// </summary>
        /// <param name="name">The name for the <see cref="DnsName"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="name"/> is an empty string.
        /// </exception>
        public DnsName(string name)
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
        protected override DnsName Clone(string value)
        {
            return new DnsName(value);
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
            return string.Format(CultureInfo.InvariantCulture, @"DnsName: {0}", Equals(Nobody) ? "Nobody" : InternalValue);
        }
    }
}