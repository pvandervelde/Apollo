﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    /// <design>
    /// Note that there is no <c>DnsName</c> that allows sending messages 
    /// to everybody in one go because:
    /// 1) Every message must have a unique ID
    /// 2) The sender should know which message went to which recipient
    ///    (even if the sender doesn't care)
    /// The combination of both means that we can't send one message to 
    /// multiple recipients because then we'd have to return a collection
    /// of ID numbers, and thus the sender wouldn't know which recipient
    /// got which message. Obviously we could return a collection of 
    /// maps but that seems to get very complicated very quickly.
    /// </design>
    [Serializable]
    [DebuggerDisplay("DnsName: [{InternalValue}]")]
    public sealed class DnsName : Id<DnsName, string>
    {
        /// <summary>
        /// The unique identifier for none of the services.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly DnsName s_Nobody = new DnsName(@"Nobody");

        /// <summary>
        /// Gets the <c>DnsName</c> for none of the services.
        /// Any messages send with this <c>DnsName</c> as recipient will
        /// be removed by the message pipeline.
        /// </summary>
        /// <value>The <c>DnsName</c> for none of the services.</value>
        public static DnsName Nobody
        {
            [DebuggerStepThrough]
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
        /// Determines whether the specified values to see if they are equal.
        /// </summary>
        /// <param name="ourValue">The value owned by the current ID.</param>
        /// <param name="theirValue">The value owned by the other ID.</param>
        /// <returns>
        ///     <see langword="true"/> if <paramref name="theirValue"/> is equal to the value owned by this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected override bool AreValuesEqual(string ourValue, string theirValue)
        {
            return string.Equals(ourValue, theirValue, StringComparison.Ordinal);
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