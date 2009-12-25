//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
    public sealed class DnsName : IEquatable<DnsName>, IComparable<DnsName>, IComparable
    {
        /// <summary>
        /// The unique identifier for none of the services.
        /// </summary>
        private static readonly DnsName s_Nobody = new DnsName(@"Nobody");

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DnsName first, DnsName second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.Equals(second);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DnsName first, DnsName second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && !first.Equals(second);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(DnsName first, DnsName second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.CompareTo(second) > 0;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(DnsName first, DnsName second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.CompareTo(second) < 0;
        }

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
        /// The identifier of the <c>DnsName</c>.
        /// </summary>
        private readonly string m_Name;

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
        { 
            {
                Enforce.Argument(() => name);
                Enforce.Argument(() => name, StringIs.NotEmpty);
            }

            m_Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsName"/> class.
        /// </summary>
        /// <param name="nameToCopy">The <c>DnsName</c> which should be copied.</param>
        public DnsName(DnsName nameToCopy) 
            : this(nameToCopy.m_Name)
        { 
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance is less than <paramref name="other"/>.
        /// Zero
        /// This instance is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This instance is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(DnsName other)
        {
            return other == null ? 1 : string.Compare(other.m_Name, m_Name, StringComparison.Ordinal);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance is less than <paramref name="obj"/>.
        /// Zero
        /// This instance is equal to <paramref name="obj"/>.
        /// Greater than zero
        /// This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="obj"/> is not the same type as this instance.
        /// </exception>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator.
            var name = obj as DnsName;
            if (ReferenceEquals(name, null))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, @"Cannot compare these objects: {0} and {1}", this, obj));
            }

            return CompareTo(name);
        }

        /// <summary>
        /// Determines whether the specified <c>DnsName</c> is equal to this instance.
        /// </summary>
        /// <param name="other">The <c>DnsName</c> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <c>DnsName</c> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(DnsName other)
        {
            return other != null && other.m_Name.Equals(m_Name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator.
            var name = obj as DnsName;
            if (!ReferenceEquals(name, null))
            {
                return Equals(name);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return m_Name.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, @"DnsName: {0}", m_Name);
        }
    }
}