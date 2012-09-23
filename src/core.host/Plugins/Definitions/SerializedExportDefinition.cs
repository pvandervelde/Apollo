//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Host.Plugins.Definitions
{
    /// <summary>
    /// Provides the base class for classes that store information about a MEF export in a serializable form, i.e. without 
    /// requiring the owning type to be loaded.
    /// </summary>
    [Serializable]
    internal abstract class SerializedExportDefinition : IEquatable<SerializedExportDefinition>
    {
        /// <summary>
        /// The contract name that is used with the export.
        /// </summary>
        private readonly string m_ContractName;

        /// <summary>
        /// The contract type that is used with the export.
        /// </summary>
        private readonly SerializedTypeIdentity m_ContractType;

        /// <summary>
        /// The serialized description of the type that owns the current export.
        /// </summary>
        private readonly SerializedTypeIdentity m_DeclaringType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedExportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current export.</param>
        /// <param name="contractType">The exported type for the contract.</param>
        /// <param name="declaringType">The type that declares the current export.</param>
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
        ///     Thrown if <paramref name="declaringType"/> is <see langword="null" />.
        /// </exception>
        protected SerializedExportDefinition(string contractName, Type contractType, Type declaringType)
        {
            {
                Lokad.Enforce.Argument(() => contractName);
                Lokad.Enforce.Argument(() => contractName, Lokad.Rules.StringIs.NotEmpty);
                Lokad.Enforce.Argument(() => contractType);
                Lokad.Enforce.Argument(() => declaringType);
            }

            m_ContractName = contractName;
            m_ContractType = new SerializedTypeIdentity(contractType);
            m_DeclaringType = new SerializedTypeIdentity(declaringType);
        }

        /// <summary>
        /// Gets the contract name of the export.
        /// </summary>
        public string ContractName
        {
            get
            {
                return m_ContractName;
            }
        }

        /// <summary>
        /// Gets the serialized definition of the contract type.
        /// </summary>
        public SerializedTypeIdentity ContractType
        {
            get
            {
                return m_ContractType;
            }
        }

        /// <summary>
        /// Gets the serialized definition of the type that declares the export.
        /// </summary>
        public SerializedTypeIdentity DeclaringType
        {
            get
            {
                return m_DeclaringType;
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
        public abstract bool Equals(SerializedExportDefinition other);

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
        public abstract override bool Equals(object obj);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public abstract override int GetHashCode();

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public abstract override string ToString();
    }
}
