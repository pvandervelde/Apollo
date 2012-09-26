//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Apollo.Core.Host.Plugins.Definitions
{
    /// <summary>
    /// Stores information about an exported property in serialized form, i.e. without requiring the
    /// owning type in question to be loaded.
    /// </summary>
    [Serializable]
    internal sealed class SerializedExportOnPropertyDefinition : SerializedExportDefinition
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SerializedExportOnPropertyDefinition first, SerializedExportOnPropertyDefinition second)
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
        public static bool operator !=(SerializedExportOnPropertyDefinition first, SerializedExportOnPropertyDefinition second)
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
        /// Creates a new instance of the <see cref="SerializedExportOnPropertyDefinition"/> class based on 
        /// the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current export.</param>
        /// <param name="contractType">The exported type for the contract.</param>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="contractName"/> is an empty string..
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        public static SerializedExportOnPropertyDefinition CreateDefinition(
            string contractName,
            Type contractType,
            PropertyInfo property,
            Func<Type, SerializedTypeIdentity> identityGenerator)
        {
            {
                Lokad.Enforce.Argument(() => property);
                Lokad.Enforce.Argument(() => identityGenerator);
            }

            return new SerializedExportOnPropertyDefinition(
                contractName,
                identityGenerator(contractType),
                identityGenerator(property.DeclaringType),
                SerializedPropertyDefinition.CreateDefinition(property, identityGenerator));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SerializedExportOnPropertyDefinition"/> class based on the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current export.</param>
        /// <param name="contractType">The exported type for the contract.</param>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="contractName"/> is an empty string..
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        public static SerializedExportOnPropertyDefinition CreateDefinition(string contractName, Type contractType, PropertyInfo property)
        {
            return CreateDefinition(contractName, contractType, property, t => SerializedTypeIdentity.CreateDefinition(t));
        }

        /// <summary>
        /// The property.
        /// </summary>
        private readonly SerializedPropertyDefinition m_Property;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedExportOnPropertyDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current export.</param>
        /// <param name="contractType">The exported type for the contract.</param>
        /// <param name="declaringType">The type that declares the property on which the import is placed.</param>
        /// <param name="property">The property for which the current object stores the serialized data.</param>
        private SerializedExportOnPropertyDefinition(
            string contractName, 
            SerializedTypeIdentity contractType, 
            SerializedTypeIdentity declaringType,
            SerializedPropertyDefinition property)
            : base(contractName, contractType, declaringType)
        {
            {
                Debug.Assert(property != null, "The property object shouldn't be null.");
            }

            m_Property = property;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        public SerializedPropertyDefinition Property
        {
            get
            {
                return m_Property;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializedExportDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializedExportDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializedExportDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(SerializedExportDefinition other)
        {
            var otherType = other as SerializedExportOnPropertyDefinition;
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(otherType, null)
                && string.Equals(ContractName, otherType.ContractName, StringComparison.OrdinalIgnoreCase)
                && ContractType == otherType.ContractType
                && Property.Equals(otherType.Property);
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

            var id = obj as SerializedExportOnPropertyDefinition;
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
                hash = (hash * 23) ^ ContractType.GetHashCode();
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
                "Exporting [{0}, {1}] on {2}", 
                ContractName, 
                ContractType, 
                Property);
        }
    }
}
