﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Provides the base class for classes that store information about a MEF export in a serializable form, i.e. without 
    /// requiring the owning type to be loaded.
    /// </summary>
    /// <design>
    /// It would be nice if this class would derrive from the <c>System.ComponentModel.Composition.Primitives.ExportDefinition</c> but
    /// that class is not serializable.
    /// </design>
    [Serializable]
    public abstract class SerializableExportDefinition : IEquatable<SerializableExportDefinition>
    {
        /// <summary>
        /// The name of the contract for the export.
        /// </summary>
        private readonly string m_ContractName;

        /// <summary>
        /// The serialized description of the type that owns the current export.
        /// </summary>
        private readonly TypeIdentity m_DeclaringType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableExportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current export.</param>
        /// <param name="declaringType">The type that declares the current export.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="declaringType"/> is <see langword="null" />.
        /// </exception>
        protected SerializableExportDefinition(string contractName, TypeIdentity declaringType)
        {
            {
                Lokad.Enforce.Argument(() => declaringType);
            }

            m_ContractName = contractName;
            m_DeclaringType = declaringType;
        }

        /// <summary>
        /// Gets the contract name for the export.
        /// </summary>
        public string ContractName
        {
            get
            {
                return m_ContractName;
            }
        }

        /// <summary>
        /// Gets the serialized definition of the type that declares the export.
        /// </summary>
        public TypeIdentity DeclaringType
        {
            get
            {
                return m_DeclaringType;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializableExportDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializableExportDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializableExportDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public abstract bool Equals(SerializableExportDefinition other);

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
