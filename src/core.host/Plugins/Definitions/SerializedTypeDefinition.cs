//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Apollo.Core.Host.Plugins.Definitions
{
    /// <summary>
    /// Stores information about an type in a serializable form, i.e. without requiring the
    /// type in question to be loaded.
    /// </summary>
    [Serializable]
    internal sealed class SerializedTypeDefinition : IEquatable<SerializedTypeDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SerializedTypeDefinition first, SerializedTypeDefinition second)
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
        public static bool operator !=(SerializedTypeDefinition first, SerializedTypeDefinition second)
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
        /// The identity information for the type.
        /// </summary>
        private readonly SerializedTypeIdentity m_Identity;

        /// <summary>
        /// The base class for the current type. Is <c>null</c> if the current type
        /// doesn't have a base class.
        /// </summary>
        private readonly SerializedTypeIdentity m_Base;

        /// <summary>
        /// The interfaces that are inherited or implemented by the current type.
        /// </summary>
        private readonly SerializedTypeIdentity[] m_BaseInterfaces;

        /// <summary>
        /// A flag indicating if the current type is a class or not.
        /// </summary>
        private readonly bool m_IsClass;

        /// <summary>
        /// A flag indicating if the current type is an interface or not.
        /// </summary>
        private readonly bool m_IsInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedTypeDefinition"/> class.
        /// </summary>
        /// <param name="type">The Type that should be serialized.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        public SerializedTypeDefinition(Type type)
        {
            {
                Lokad.Enforce.Argument(() => type);
            }

            m_Identity = new SerializedTypeIdentity(type);
            m_BaseInterfaces = type.GetInterfaces().Select(i => new SerializedTypeIdentity(i)).ToArray();
            if (type.BaseType != null)
            {
                m_Base = new SerializedTypeIdentity(type.BaseType);
            }

            m_IsClass = type.IsClass;
            m_IsInterface = type.IsInterface;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public SerializedTypeIdentity Identity
        {
            get
            {
                return m_Identity;
            }
        }

        /// <summary>
        /// Gets the base or parent type for the type.
        /// </summary>
        public SerializedTypeIdentity BaseType
        {
            get
            {
                return m_Base;
            }
        }

        /// <summary>
        /// Gets the type information for all the interfaces that are implemented by the current type.
        /// </summary>
        public IEnumerable<SerializedTypeIdentity> BaseInterfaces
        {
            get
            {
                return m_BaseInterfaces;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the type is a class.
        /// </summary>
        public bool IsClass
        {
            get
            {
                return m_IsClass;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the type is an interface.
        /// </summary>
        public bool IsInterface
        {
            get
            {
                return m_IsInterface;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializedTypeDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializedTypeDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializedTypeDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(SerializedTypeDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) && Identity.Equals(other.Identity);
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

            var id = obj as SerializedTypeDefinition;
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
                hash = (hash * 23) ^ Identity.GetHashCode();
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
            return Identity.ToString();
        }
    }
}
