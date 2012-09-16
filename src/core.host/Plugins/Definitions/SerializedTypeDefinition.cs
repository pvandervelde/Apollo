//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

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
        /// The name of the type.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// The namespace of the type.
        /// </summary>
        private readonly string m_Namespace;

        /// <summary>
        /// The assembly information for the assembly that contains the type.
        /// </summary>
        private readonly SerializedAssemblyDefinition m_Assembly;

        /// <summary>
        /// The base class for the current type. Is <c>null</c> if the current type
        /// doesn't have a base class.
        /// </summary>
        private readonly SerializedTypeDefinition m_Base;

        /// <summary>
        /// The interfaces that are inherited or implemented by the current type.
        /// </summary>
        private readonly IEnumerable<SerializedTypeDefinition> m_BaseInterfaces;

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

            m_Name = type.Name;
            m_Namespace = type.Namespace;
            m_Assembly = new SerializedAssemblyDefinition(type.Assembly);
            
            m_IsClass = type.IsClass;
            m_IsInterface = type.IsInterface;
            if (type.BaseType != null)
            {
                m_Base = new SerializedTypeDefinition(type.BaseType);
            }

            var list = new List<SerializedTypeDefinition>();
            m_BaseInterfaces = list;
            foreach (var @interface in type.GetInterfaces())
            {
                list.Add(new SerializedTypeDefinition(@interface));
            }
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public string Name
        {
            get 
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the namespace of the type.
        /// </summary>
        public string Namespace
        {
            get
            {
                return m_Namespace;
            }
        }

        /// <summary>
        /// Gets the fully qualified name of the type.
        /// </summary>
        public string FullName
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Namespace, Name);
            }
        }

        /// <summary>
        /// Gets the assembly information for the type.
        /// </summary>
        public SerializedAssemblyDefinition Assembly
        {
            get
            {
                return m_Assembly;
            }
        }

        /// <summary>
        /// Gets the base or parent type for the type.
        /// </summary>
        public SerializedTypeDefinition BaseType
        {
            get
            {
                return m_Base;
            }
        }

        /// <summary>
        /// Gets the type information for all the interfaces that are implemented by the current type.
        /// </summary>
        public IEnumerable<SerializedTypeDefinition> BaseInterfaces
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
            return !ReferenceEquals(other, null) 
                && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) 
                && string.Equals(Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase)
                && Assembly.Equals(other.Assembly);
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
                hash = (hash * 23) ^ Name.GetHashCode();
                hash = (hash * 23) ^ Namespace.GetHashCode();
                hash = (hash * 23) ^ Assembly.GetHashCode();

                return hash;
            }
        }
    }
}
