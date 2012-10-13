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

namespace Apollo.Core.Host.Plugins.Definitions
{
    /// <summary>
    /// Stores a group ID and an import ID to uniquely identify an import.
    /// </summary>
    [Serializable]
    internal sealed class GroupImportMap
    {
        /// <summary>
        /// The group that holds the import.
        /// </summary>
        private readonly GroupRegistrationId m_Group;

        /// <summary>
        /// The import.
        /// </summary>
        private readonly ImportRegistrationId m_Import;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupImportMap"/> class.
        /// </summary>
        /// <param name="export">The import for the map.</param>
        public GroupImportMap(ImportRegistrationId export)
            : this(null, export)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupImportMap"/> class.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="import">The import for the map.</param>
        public GroupImportMap(GroupRegistrationId group, ImportRegistrationId import)
        {
            {
                Debug.Assert(import != null, "To create an ImportMap the import ID should not be null.");
            }

            m_Group = group;
            m_Import = import;
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
        /// Gets the import for this map.
        /// </summary>
        public ImportRegistrationId Import
        {
            get
            {
                return m_Import;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="GroupImportMap"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="GroupImportMap"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="GroupImportMap"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(GroupImportMap other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && Import == other.Import
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

            var id = obj as GroupImportMap;
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
                hash = (hash * 23) ^ Import.GetHashCode();
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
                ? string.Format(CultureInfo.InvariantCulture, "[{0}]-[{1}]", Group, Import)
                : string.Format(CultureInfo.InvariantCulture, "[{0}]", Import);
        }
    }
}
