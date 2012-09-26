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
using System.Reflection;

namespace Apollo.Core.Host.Plugins.Definitions
{
    /// <summary>
    /// Stores information about a constructor in a serializable form, i.e. without requiring the
    /// owning type to be loaded.
    /// </summary>
    [Serializable]
    internal sealed class SerializedConstructorDefinition : IEquatable<SerializedConstructorDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SerializedConstructorDefinition first, SerializedConstructorDefinition second)
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
        public static bool operator !=(SerializedConstructorDefinition first, SerializedConstructorDefinition second)
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
        /// Creates a new instance of the <see cref="SerializedConstructorDefinition"/> class based on the given <see cref="ConstructorInfo"/>.
        /// </summary>
        /// <param name="constructor">The constructor for which a serialized definition needs to be created.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given constructor.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="constructor"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        public static SerializedConstructorDefinition CreateDefinition(
            ConstructorInfo constructor, 
            Func<Type, SerializedTypeIdentity> identityGenerator)
        {
            {
                Lokad.Enforce.Argument(() => constructor);
                Lokad.Enforce.Argument(() => identityGenerator);
            }

            return new SerializedConstructorDefinition(
                identityGenerator(constructor.DeclaringType),
                constructor.GetParameters().Select(p => SerializedParameterDefinition.CreateDefinition(p, identityGenerator)).ToArray());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SerializedConstructorDefinition"/> class based on the given <see cref="ConstructorInfo"/>.
        /// </summary>
        /// <param name="constructor">The constructor for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given constructor.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="constructor"/> is <see langword="null" />.
        /// </exception>
        public static SerializedConstructorDefinition CreateDefinition(ConstructorInfo constructor)
        {
            return CreateDefinition(constructor, t => SerializedTypeIdentity.CreateDefinition(t));
        }

        /// <summary>
        /// The type that owns the constructor.
        /// </summary>
        private readonly SerializedTypeIdentity m_DeclaringType;

        /// <summary>
        /// The collection of parameters for the constructor.
        /// </summary>
        private readonly SerializedParameterDefinition[] m_Parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedConstructorDefinition"/> class.
        /// </summary>
        /// <param name="declaringType">The serialized definition for the type that declares the constructor.</param>
        /// <param name="parameters">The array containing the definitions for the constructor parameters.</param>
        private SerializedConstructorDefinition(
            SerializedTypeIdentity declaringType,
            SerializedParameterDefinition[] parameters)
        {
            {
                Debug.Assert(declaringType != null, "The declaring type should not be null.");
                Debug.Assert(parameters != null, "The parameter array should not be null.");
            }

            m_DeclaringType = declaringType;
            m_Parameters = parameters;
        }

        /// <summary>
        /// Gets the type that owns the current constructor.
        /// </summary>
        public SerializedTypeIdentity DeclaringType
        {
            get
            {
                return m_DeclaringType;
            }
        }

        /// <summary>
        /// Gets the collection containing the parameters for the constructor.
        /// </summary>
        public IEnumerable<SerializedParameterDefinition> Parameters
        {
            get
            {
                return m_Parameters;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializedConstructorDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializedConstructorDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializedConstructorDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(SerializedConstructorDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && DeclaringType.Equals(other.DeclaringType)
                && Parameters.SequenceEqual(other.Parameters);
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

            var id = obj as SerializedConstructorDefinition;
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
                hash = (hash * 23) ^ DeclaringType.GetHashCode();
                foreach (var parameter in Parameters)
                {
                    hash = (hash * 23) ^ parameter.GetHashCode();
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
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}.ctr({1})",
                DeclaringType,
                string.Join(", ", Parameters.Select(p => p.ToString())));
        }
    }
}
