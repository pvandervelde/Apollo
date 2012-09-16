﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Apollo.Core.Host.Plugins.Definitions
{
    /// <summary>
    /// Stores information about an imported method parameter in serialized form, i.e. without requiring the
    /// owning type in question to be loaded.
    /// </summary>
    [Serializable]
    internal sealed class SerializedImportOnMethodDefinition : SerializedImportDefinition
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SerializedImportOnMethodDefinition first, SerializedImportOnMethodDefinition second)
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
        public static bool operator !=(SerializedImportOnMethodDefinition first, SerializedImportOnMethodDefinition second)
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
        /// The parameter on which the import is defined.
        /// </summary>
        private readonly SerializedParameterDefinition m_Parameter;

        /// <summary>
        /// The method on which the import is defined.
        /// </summary>
        private readonly SerializedMethodDefinition m_Method;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedImportOnMethodDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current export.</param>
        /// <param name="contractType">The exported type for the contract.</param>
        /// <param name="parameter">The parameter on which the import is defined.</param>
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
        ///     Thrown if <paramref name="parameter"/> is <see langword="null" />.
        /// </exception>
        public SerializedImportOnMethodDefinition(
            string contractName, 
            Type contractType, 
            ParameterInfo parameter)
            : base(contractName, contractType, parameter.Member.DeclaringType)
        {
            {
                Lokad.Enforce.Argument(() => parameter);
            }

            m_Method = new SerializedMethodDefinition(parameter.Member as MethodInfo);
            m_Parameter = new SerializedParameterDefinition(parameter);
        }

        /// <summary>
        /// Gets the parameter definition.
        /// </summary>
        public SerializedParameterDefinition Parameter
        {
            get
            {
                return m_Parameter;
            }
        }

        /// <summary>
        /// Gets the method definition.
        /// </summary>
        public SerializedMethodDefinition Method
        {
            get
            {
                return m_Method;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializedImportDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializedImportDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializedImportDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(SerializedImportDefinition other)
        {
            var otherType = other as SerializedImportOnMethodDefinition;
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
                && Method == otherType.Method
                && Parameter == otherType.Parameter;
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

            var id = obj as SerializedImportOnMethodDefinition;
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
                hash = (hash * 23) ^ DeclaringType.GetHashCode();
                hash = (hash * 23) ^ Method.GetHashCode();
                hash = (hash * 23) ^ Parameter.GetHashCode();

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
                "Importing [{0}, {1}] on {2}",
                ContractName,
                ContractType,
                Method);
        }
    }
}
