﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Stores information about a property that is used as a schedule condition in a serializable form, 
    /// i.e. without requiring the owning type to be loaded.
    /// </summary>
    [Serializable]
    public sealed class PropertyBasedScheduleConditionDefinition : ScheduleConditionDefinition
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(PropertyBasedScheduleConditionDefinition first, PropertyBasedScheduleConditionDefinition second)
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
        public static bool operator !=(PropertyBasedScheduleConditionDefinition first, PropertyBasedScheduleConditionDefinition second)
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
        /// Creates a new instance of the <see cref="PropertyBasedScheduleConditionDefinition"/> class based on 
        /// the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identity the current condition.</param>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        public static PropertyBasedScheduleConditionDefinition CreateDefinition(
            string contractName,
            PropertyInfo property,
            Func<Type, TypeIdentity> identityGenerator)
        {
            {
                Lokad.Enforce.Argument(() => contractName);
                Lokad.Enforce.Argument(() => contractName, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => property);
                Lokad.Enforce.Argument(() => identityGenerator);
            }

            return new PropertyBasedScheduleConditionDefinition(
                contractName,
                PropertyDefinition.CreateDefinition(property, identityGenerator));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MethodDefinition"/> class based on the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identity the current condition.</param>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        public static PropertyBasedScheduleConditionDefinition CreateDefinition(string contractName, PropertyInfo property)
        {
            return CreateDefinition(contractName, property, t => TypeIdentity.CreateDefinition(t));
        }

        /// <summary>
        /// The method that will provide the condition result.
        /// </summary>
        private readonly PropertyDefinition m_Property;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBasedScheduleConditionDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current condition.</param>
        /// <param name="property">The property that will provide the condition result.</param>
        private PropertyBasedScheduleConditionDefinition(string contractName, PropertyDefinition property)
            : base(contractName)
        {
            {
                Debug.Assert(property != null, "The property object should not be null.");
            }

            m_Property = property;
        }

        /// <summary>
        /// Gets the method that will provide the condition result.
        /// </summary>
        public PropertyDefinition Property
        {
            get
            {
                return m_Property;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="ScheduleConditionDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="ScheduleConditionDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="ScheduleConditionDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "There is no need to validate the parameter because it is implicitly verified.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(ScheduleConditionDefinition other)
        {
            var otherType = other as PropertyBasedScheduleConditionDefinition;
            if (ReferenceEquals(this, otherType))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(otherType, null)
                && string.Equals(ContractName, other.ContractName, StringComparison.Ordinal)
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

            var id = obj as PropertyBasedScheduleConditionDefinition;
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
                "Condition {0} on {1}",
                ContractName,
                Property);
        }
    }
}
