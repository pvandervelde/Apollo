﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Stores information about an imported property in serialized form, i.e. without requiring the
    /// owning type in question to be loaded.
    /// </summary>
    [Serializable]
    public sealed class PropertyBasedImportDefinition : SerializableImportDefinition
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(PropertyBasedImportDefinition first, PropertyBasedImportDefinition second)
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
        public static bool operator !=(PropertyBasedImportDefinition first, PropertyBasedImportDefinition second)
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
        /// Creates a new instance of the <see cref="PropertyBasedImportDefinition"/> class based on 
        /// the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current import.</param>
        /// <param name="requiredTypeIdentity">The type identity of the export type expected.</param>
        /// <param name="cardinality">
        ///     One of the enumeration values that indicates the cardinality of the export object required by the import definition.
        /// </param>
        /// <param name="isRecomposable">
        ///     <see langword="true" /> to specify that the import definition can be satisfied multiple times throughout the lifetime of a parts; 
        ///     otherwise, <see langword="false" />.
        /// </param>
        /// <param name="creationPolicy">
        ///     A value that indicates that the importer requires a specific creation policy for the exports used to satisfy this import.
        /// </param>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Recomposable",
            Justification = "MEF uses the same term, so we're not going to make up some other one.")]
        public static PropertyBasedImportDefinition CreateDefinition(
            string contractName,
            TypeIdentity requiredTypeIdentity,
            ImportCardinality cardinality, 
            bool isRecomposable, 
            CreationPolicy creationPolicy,
            PropertyInfo property,
            Func<Type, TypeIdentity> identityGenerator)
        {
            {
                Lokad.Enforce.Argument(() => property);
                Lokad.Enforce.Argument(() => identityGenerator);
            }

            return new PropertyBasedImportDefinition(
                contractName,
                requiredTypeIdentity,
                cardinality,
                isRecomposable,
                creationPolicy,
                identityGenerator(property.DeclaringType),
                PropertyDefinition.CreateDefinition(property, identityGenerator));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PropertyBasedImportDefinition"/> class based on the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current import.</param>
        /// <param name="requiredTypeIdentity">The type identity of the export type expected.</param>
        /// <param name="cardinality">
        ///     One of the enumeration values that indicates the cardinality of the export object required by the import definition.
        /// </param>
        /// <param name="isRecomposable">
        ///     <see langword="true" /> to specify that the import definition can be satisfied multiple times throughout the lifetime of a parts; 
        ///     otherwise, <see langword="false" />.
        /// </param>
        /// <param name="creationPolicy">
        ///     A value that indicates that the importer requires a specific creation policy for the exports used to satisfy this import.
        /// </param>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Recomposable",
            Justification = "MEF uses the same term, so we're not going to make up some other one.")]
        public static PropertyBasedImportDefinition CreateDefinition(
            string contractName,
            TypeIdentity requiredTypeIdentity,
            ImportCardinality cardinality, 
            bool isRecomposable, 
            CreationPolicy creationPolicy,
            PropertyInfo property)
        {
            return CreateDefinition(
                contractName,
                requiredTypeIdentity,
                cardinality,
                isRecomposable,
                creationPolicy,
                property, 
                t => TypeIdentity.CreateDefinition(t));
        }

        /// <summary>
        /// The name of the property.
        /// </summary>
        private readonly PropertyDefinition m_Property;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBasedImportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current import.</param>
        /// <param name="requiredTypeIdentity">The type identity of the export type expected.</param>
        /// <param name="cardinality">
        ///     One of the enumeration values that indicates the cardinality of the export object required by the import definition.
        /// </param>
        /// <param name="isRecomposable">
        ///     <see langword="true" /> to specify that the import definition can be satisfied multiple times throughout the lifetime of a parts; 
        ///     otherwise, <see langword="false" />.
        /// </param>
        /// <param name="creationPolicy">
        ///     A value that indicates that the importer requires a specific creation policy for the exports used to satisfy this import.
        /// </param>
        /// <param name="declaringType">The type that defines the property.</param>
        /// <param name="property">The property for which the current object stores the serialized data.</param>
        private PropertyBasedImportDefinition(
            string contractName,
            TypeIdentity requiredTypeIdentity,
            ImportCardinality cardinality, 
            bool isRecomposable, 
            CreationPolicy creationPolicy, 
            TypeIdentity declaringType,
            PropertyDefinition property)
            : base(
                contractName, 
                requiredTypeIdentity, 
                cardinality,
                isRecomposable,
                false,
                creationPolicy,
                declaringType)
        {
            {
                Lokad.Enforce.Argument(() => property);
            }

            m_Property = property;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        public PropertyDefinition Property
        {
            get
            {
                return m_Property;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializableImportDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializableImportDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializableImportDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(SerializableImportDefinition other)
        {
            var otherType = other as PropertyBasedImportDefinition;
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(otherType, null)
                && string.Equals(ContractName, otherType.ContractName, StringComparison.OrdinalIgnoreCase)
                && RequiredTypeIdentity.Equals(otherType.RequiredTypeIdentity)
                && Property == otherType.Property;
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
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as PropertyBasedImportDefinition;
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
                hash = (hash * 23) ^ RequiredTypeIdentity.GetHashCode();
                hash = (hash * 23) ^ Property.GetHashCode();

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
                Property);
        }
    }
}
