//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores a <see cref="TimeMarker"/> and a value at that specific time.
    /// </summary>
    /// <typeparam name="T">The type of value that is stored.</typeparam>
    internal struct ValueAtTime<T> : IEquatable<ValueAtTime<T>>, IComparable<ValueAtTime<T>>, IComparable
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ValueAtTime<T> first, ValueAtTime<T> second)
        {
            var nonNullObject = first;
            var possibleNullObject = second;
            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ValueAtTime<T> first, ValueAtTime<T> second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            var nonNullObject = first;
            var possibleNullObject = second;
            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(ValueAtTime<T> first, ValueAtTime<T> second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return first.CompareTo(second) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(ValueAtTime<T> first, ValueAtTime<T> second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return first.CompareTo(second) >= 0;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(ValueAtTime<T> first, ValueAtTime<T> second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return first.CompareTo(second) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(ValueAtTime<T> first, ValueAtTime<T> second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return first.CompareTo(second) <= 0;
        }

        /// <summary>
        /// The point in time for which the value is stored.
        /// </summary>
        private readonly TimeMarker m_Time;

        /// <summary>
        /// The value.
        /// </summary>
        private readonly T m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueAtTime{T}"/> struct.
        /// </summary>
        /// <param name="time">The point in time for which the value is stored.</param>
        /// <param name="value">The value.</param>
        public ValueAtTime(TimeMarker time, T value)
        {
            {
                Debug.Assert(time != null, "The time marker should not be null.");
            }

            m_Time = time;
            m_Value = value;
        }

        /// <summary>
        /// Gets the marker that indicates for which point in time
        /// this structure is valid.
        /// </summary>
        public TimeMarker Time
        {
            get
            {
                return m_Time;
            }
        }

        /// <summary>
        /// Gets the value for the given point in time.
        /// </summary>
        public T Value
        {
            get
            {
                return m_Value;
            }
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that
        /// indicates whether the current instance precedes, follows, or occurs in the same position in the
        /// sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance is less than <paramref name="other"/>.
        /// Zero
        /// This instance is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This instance is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(ValueAtTime<T> other)
        {
            return m_Time.CompareTo(other.m_Time);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that
        /// indicates whether the current instance precedes, follows, or occurs in the same position in the
        /// sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
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
            // We don't strictly need to use the ReferenceEquals method but
            // it seems more consistent to use it.
            if (ReferenceEquals(obj, null))
            {
                return 1;
            }

            if (!(obj is ValueAtTime<T>))
            {
                throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_ErrorCode_CompareArgument,
                            obj.GetType().FullName,
                            GetType().FullName),
                        @"obj");
            }

            return CompareTo((ValueAtTime<T>)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="TimeMarker"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="TimeMarker"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="TimeMarker"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(ValueAtTime<T> other)
        {
            return m_Time == other.m_Time;
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
            if (obj is ValueAtTime<T>)
            {
                var value = (ValueAtTime<T>)obj;
                return Equals(value);
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
            // As obtained from the Jon Skeet answer to:
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                var hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ m_Time.GetHashCode();
                return hash;
            }
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
                "Time: {0}; Value: {1}",
                m_Time,
                m_Value);
        }
    }
}
