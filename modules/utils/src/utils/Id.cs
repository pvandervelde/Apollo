﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Utils.Properties;

namespace Apollo.Utils
{
    /// <summary>
    /// Defines the base class for ID numbers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Derivative classes should define the type parameters as:
    /// </para>
    /// <example>
    /// public sealed class SomeId : Id&lt;SomeId, SomeValueType&gt;
    /// </example>
    /// </remarks>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TInternalValue">The type of object that is stored internally as the ID number.</typeparam>
    [Serializable]
    public abstract class Id<TId, TInternalValue> : IIsId<TId> 
        where TId : Id<TId, TInternalValue>
        where TInternalValue : IComparable<TInternalValue>, IEquatable<TInternalValue>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Id<TId, TInternalValue> first, Id<TId, TInternalValue> second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.Equals(second);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Id<TId, TInternalValue> first, Id<TId, TInternalValue> second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return !first.Equals(second);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Id<TId, TInternalValue> first, Id<TId, TInternalValue> second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null))
            {
                return false;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(second, null))
            {
                return false;
            }

            return first.CompareTo(second) > 0;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Id<TId, TInternalValue> first, Id<TId, TInternalValue> second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null))
            {
                return false;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(second, null))
            {
                return false;
            }

            return first.CompareTo(second) < 0;
        }

        /// <summary>
        /// The internal value which defines the value for the current ID.
        /// </summary>
        private readonly TInternalValue m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Id&lt;TId, TInternalValue&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <design>
        /// There is no way to check that the value is actually usable. This all
        /// depends on the type of the internal value. Unfortunately only the
        /// derivative class knows that. But using a virtual method in a constructor
        /// is not advisable. And so we can't call into the derivative class for
        /// checking.
        /// </design>
        protected Id(TInternalValue value)
        {
            m_Value = value;
        }

        /// <summary>
        /// Gets the internal value in a readonly fashion.
        /// </summary>
        /// <value>The internal value.</value>
        protected TInternalValue InternalValue
        {
            get 
            {
                return m_Value;
            }
        }

        /// <summary>
        /// Clones this ID number.
        /// </summary>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        public TId Clone()
        {
            return Clone(m_Value);
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected abstract TId Clone(TInternalValue value);

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
        public int CompareTo(TId other)
        {
            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            return m_Value.CompareTo(other.m_Value);
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
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            var id = obj as TId;
            if (ReferenceEquals(id, null))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_ErrorCode_CompareArgument,
                        obj.GetType().FullName,
                        this.GetType().FullName),
                    @"obj");
            }

            return CompareTo(id);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Id&lt;TId, TInternalValue&gt;"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Id&lt;TId, TInternalValue&gt;"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="Id&lt;TId, TInternalValue&gt;"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(TId other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return m_Value.Equals(other.m_Value);
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
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            var ConfigurationElementId = obj as TId;
            if (ReferenceEquals(ConfigurationElementId, null))
            {
                return false;
            }

            return Equals(ConfigurationElementId);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public sealed override int GetHashCode()
        {
            return m_Value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public abstract override string ToString();
    }
}