//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Host.Plugins.Definitions
{
    /// <summary>
    /// Stores information about an import for a component group in serialized form, i.e. without requiring the
    /// assembly which defines the group to be loaded.
    /// </summary>
    [Serializable]
    internal sealed class SerializedGroupImportDefinition : IEquatable<SerializedGroupImportDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SerializedGroupImportDefinition first, SerializedGroupImportDefinition second)
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
        public static bool operator !=(SerializedGroupImportDefinition first, SerializedGroupImportDefinition second)
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
        /// Creates a new instance of the <see cref="SerializedGroupImportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name for the import.</param>
        /// <param name="containingGroup">The ID of the group that has registered the import.</param>
        /// <param name="insertPoint">The schedule insert point at which a sub-schedule can be provided.</param>
        /// <param name="importsToMatch">The object imports that have to be provided for the current import.</param>
        /// <returns>The serialized import definition for the group.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="containingGroup"/> is <see langword="null" />.
        /// </exception>
        public static SerializedGroupImportDefinition CreateDefinition(
            string contractName, 
            GroupRegistrationId containingGroup, 
            EditableInsertVertex insertPoint, 
            IEnumerable<ImportRegistrationId> importsToMatch)
        {
            {
                Lokad.Enforce.Argument(() => contractName);
                Lokad.Enforce.Argument(() => contractName, Lokad.Rules.StringIs.NotEmpty);
                Lokad.Enforce.Argument(() => containingGroup);
            }

            return new SerializedGroupImportDefinition(
                contractName, 
                containingGroup, 
                insertPoint, 
                importsToMatch ?? Enumerable.Empty<ImportRegistrationId>());
        }

        /// <summary>
        /// The ID of the group that has registered the import.
        /// </summary>
        private readonly GroupRegistrationId m_ContainingGroup;

        /// <summary>
        /// The contract name for the import.
        /// </summary>
        private readonly string m_ContractName;

        /// <summary>
        /// The schedule import point at which a sub-schedule can be provided.
        /// </summary>
        private readonly EditableInsertVertex m_InsertPoint;

        /// <summary>
        /// The object imports that have to be provided for the current import.
        /// </summary>
        private readonly IEnumerable<ImportRegistrationId> m_ImportsToMatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedGroupImportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name for the import.</param>
        /// <param name="containingGroup">The ID of the group that has registered the import.</param>
        /// <param name="insertPoint">The schedule import point at which a sub-schedule can be provided.</param>
        /// <param name="importsToMatch">The object imports that need to be provided for the current imports.</param>
        private SerializedGroupImportDefinition(
            string contractName, 
            GroupRegistrationId containingGroup, 
            EditableInsertVertex insertPoint, 
            IEnumerable<ImportRegistrationId> importsToMatch)
        {
            {
                Debug.Assert(!string.IsNullOrEmpty(contractName), "The contract name for the import should not be empty.");
                Debug.Assert(importsToMatch != null, "The collection of object imports should not be null.");
                Debug.Assert(containingGroup != null, "The ID of the group registering the import should not be null.");
            }

            m_ContainingGroup = containingGroup;
            m_ContractName = contractName;
            m_InsertPoint = insertPoint;
            m_ImportsToMatch = importsToMatch;
        }

        /// <summary>
        /// Gets the contract name for the import.
        /// </summary>
        public string ContractName
        {
            get
            {
                return m_ContractName;
            }
        }

        /// <summary>
        /// Gets the ID of the group that has registered the current import.
        /// </summary>
        public GroupRegistrationId ContainingGroup
        {
            get
            {
                return m_ContainingGroup;
            }
        }

        /// <summary>
        /// Gets the schedule import point at which a sub-schedule can be provided.
        /// </summary>
        public EditableInsertVertex ScheduleInsertPosition
        {
            get
            {
                return m_InsertPoint;
            }
        }

        /// <summary>
        /// Gets the object imports that need to be provided for the current imports.
        /// </summary>
        public IEnumerable<ImportRegistrationId> ImportsToMatch
        {
            get
            {
                return m_ImportsToMatch;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializedGroupImportDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializedGroupImportDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializedGroupImportDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(SerializedGroupImportDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) && string.Equals(ContractName, other.ContractName, StringComparison.OrdinalIgnoreCase);
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

            var id = obj as SerializedGroupImportDefinition;
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
                hash = (hash * 23) ^ ContractName.GetHashCode();

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
                "Importing [{0}] on {1}",
                ContractName,
                ContainingGroup);
        }
    }
}
