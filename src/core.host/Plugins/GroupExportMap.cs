//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Extensions.Plugins;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Stores a group ID and an export ID to uniquely identify an export.
    /// </summary>
    [Serializable]
    internal sealed class GroupExportMap : IEquatable<GroupExportMap>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(GroupExportMap first, GroupExportMap second)
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
        public static bool operator !=(GroupExportMap first, GroupExportMap second)
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
        /// The group that holds the export.
        /// </summary>
        private readonly GroupRegistrationId m_Group;

        /// <summary>
        /// The export.
        /// </summary>
        private readonly ExportRegistrationId m_Export;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupExportMap"/> class.
        /// </summary>
        /// <param name="export">The export for the map.</param>
        public GroupExportMap(ExportRegistrationId export)
            : this(null, export)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupExportMap"/> class.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="export">The export for the map.</param>
        public GroupExportMap(GroupRegistrationId group, ExportRegistrationId export)
        {
            {
                Debug.Assert(export != null, "To create an ExportMap the export ID should not be null.");
            }

            m_Group = group;
            m_Export = export;
        }

        /// <summary>
        /// Gets the group for this map.
        /// </summary>
        public GroupRegistrationId Group
        {
            get
            {
                return m_Group;
            }
        }

        /// <summary>
        /// Gets the export for this map.
        /// </summary>
        public ExportRegistrationId Export
        {
            get
            {
                return m_Export;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="GroupExportMap"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="GroupExportMap"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="GroupExportMap"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(GroupExportMap other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && Export == other.Export
                && (((Group != null) && (Group == other.Group)) || ((Group == null) && (other.Group == null)));
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

            var id = obj as GroupExportMap;
            return Equals(id);
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
                hash = (hash * 23) ^ Export.GetHashCode();
                if (Group != null)
                {
                    hash = (hash * 23) ^ Group.GetHashCode();
                }

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
            return Group != null
                ? string.Format(CultureInfo.InvariantCulture, "[{0}]-[{1}]", Group, Export)
                : string.Format(CultureInfo.InvariantCulture, "[{0}]", Export);
        }
    }
}
