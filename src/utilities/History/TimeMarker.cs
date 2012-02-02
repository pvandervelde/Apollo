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
using Lokad;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores information about a specific point on the time line at which values have been 
    /// stored.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>TimeMarker</c> class uses <see cref="ulong"/> values internally to uniquely identify
    /// different points in the history. We don't expect to ever overflow it given that just iterating
    /// over a ulong from start to end takes (on average) about 500+ years on an intel i7 920 (in
    /// debug mode). We don't expect to have that many marks on the timeline.
    /// </para>
    /// <para>
    /// Also note that timeline data is reset when we open a new project or shut down the
    /// application.
    /// </para>
    /// </remarks>
    public sealed class TimeMarker : IEquatable<TimeMarker>, IComparable<TimeMarker>, IComparable
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TimeMarker first, TimeMarker second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(TimeMarker first, TimeMarker second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(TimeMarker first, TimeMarker second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null))
            {
                return false;
            }

            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.CompareTo(second) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(TimeMarker first, TimeMarker second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null))
            {
                return false;
            }

            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.CompareTo(second) >= 0;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(TimeMarker first, TimeMarker second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null))
            {
                return true;
            }

            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.CompareTo(second) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(TimeMarker first, TimeMarker second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null))
            {
                return true;
            }

            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.CompareTo(second) <= 0;
        }

        /// <summary>
        /// The time marker that signifies the start of time.
        /// </summary>
        private static readonly TimeMarker s_TheBeginOfTime = new TimeMarker(0);

        /// <summary>
        /// Gets the time marker that signifies the start of time.
        /// </summary>
        public static TimeMarker TheBeginOfTime
        {
            get
            {
                return s_TheBeginOfTime;
            }
        }

        /// <summary>
        /// The value that indicates what the position of this time marker is on the timeline.
        /// </summary>
        private readonly ulong m_PositionInTime;

        /// <summary>
        /// The tag name of the marker. May be <c>string.Empty</c>.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeMarker"/> class.
        /// </summary>
        /// <param name="positionInTime">The value that indicates what position this time marker has on the timeline.</param>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public TimeMarker(ulong positionInTime)
            : this(positionInTime, string.Empty)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeMarker"/> class.
        /// </summary>
        /// <param name="positionInTime">The value that indicates what position this time marker has on the timeline.</param>
        /// <param name="name">The tag name of the marker. May be an empty string.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public TimeMarker(ulong positionInTime, string name)
        {
            {
                Enforce.Argument(() => name);
            }

            m_PositionInTime = positionInTime;
            m_Name = name;
        }

        /// <summary>
        /// Creates a new <see cref="TimeMarker"/> which is the logical successor to the current time marker.
        /// </summary>
        /// <returns>The new time marker.</returns>
        public TimeMarker Next()
        {
            return new TimeMarker(m_PositionInTime + 1);
        }

        /// <summary>
        /// Creates a new <see cref="TimeMarker"/> which is the logical successor to the current time marker.
        /// </summary>
        /// <param name="name">The tag name of the marker. May be an empty string.</param>
        /// <returns>The new time marker.</returns>
        public TimeMarker Next(string name)
        {
            return new TimeMarker(m_PositionInTime + 1, name);
        }

        /// <summary>
        /// Gets the tag name of the marker. Maybe an empty string.
        /// </summary>
        public string Name
        {
            [DebuggerStepThrough]
            get
            {
                return m_Name;
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
        public int CompareTo(TimeMarker other)
        {
            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return ReferenceEquals(other, null) ? 1 : m_PositionInTime.CompareTo(other.m_PositionInTime);
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

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            var id = obj as TimeMarker;
            if (ReferenceEquals(id, null))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_ErrorCode_CompareArgument,
                        obj.GetType().FullName,
                        GetType().FullName),
                    @"obj");
            }

            return CompareTo(id);
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
        public bool Equals(TimeMarker other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) && (m_PositionInTime == other.m_PositionInTime);
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
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            var id = obj as TimeMarker;
            return !ReferenceEquals(id, null) && Equals(id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return m_PositionInTime.GetHashCode();
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
                "Time mark: [{0}]. Tag name: {1}",
                m_PositionInTime,
                !string.IsNullOrEmpty(m_Name) ? m_Name : "No tag given");
        }
    }
}
