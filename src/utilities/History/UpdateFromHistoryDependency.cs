//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores information about a change set dependency which determines if a change set can be rolled back or
    /// rolled forward successfully.
    /// </summary>
    public sealed class UpdateFromHistoryDependency : IEquatable<UpdateFromHistoryDependency>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(UpdateFromHistoryDependency first, UpdateFromHistoryDependency second)
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
        public static bool operator !=(UpdateFromHistoryDependency first, UpdateFromHistoryDependency second)
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
        /// The function that indicates if the desired change can be made.
        /// </summary>
        private readonly Func<bool> m_CanExecuteChange;

        /// <summary>
        /// Indicates if roll-back, roll-forward or both can be blocked.
        /// </summary>
        private readonly ChangeBlocker m_Blocks;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFromHistoryDependency"/> class.
        /// </summary>
        /// <param name="canExecuteChange">The function that indicates if the desired change can be made.</param>
        /// <param name="blocks">Indicates if roll-back, roll-forward or both can be blocked.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="canExecuteChange"/> is <see langword="null" />.
        /// </exception>
        public UpdateFromHistoryDependency(Func<bool> canExecuteChange, ChangeBlocker blocks)
        {
            {
                Lokad.Enforce.Argument(() => canExecuteChange);
            }

            m_CanExecuteChange = canExecuteChange;
            m_Blocks = blocks;
        }

        /// <summary>
        /// Indicates if the change set belonging to the current dependency can be executed.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the change set can be rolled back or rolled forward; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanExecuteChange()
        {
            return m_CanExecuteChange();
        }

        /// <summary>
        /// Gets a value indicating which change type is influenced by this dependency.
        /// </summary>
        public ChangeBlocker Blocks
        {
            get
            {
                return m_Blocks;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="UpdateFromHistoryDependency"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="UpdateFromHistoryDependency"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="UpdateFromHistoryDependency"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(UpdateFromHistoryDependency other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && ReferenceEquals(m_CanExecuteChange, other.m_CanExecuteChange)
                && (m_Blocks == other.m_Blocks);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var dependency = obj as UpdateFromHistoryDependency;
            return Equals(dependency);
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
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ m_CanExecuteChange.GetHashCode();
                hash = (hash * 23) ^ m_Blocks.GetHashCode();

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
                "History change dependency blocking {0}",
                m_Blocks);
        }
    }
}
