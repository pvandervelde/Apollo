//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Stores information about the identity of a type in a serializable form, i.e. without requiring the
    /// type in question to be loaded.
    /// </summary>
    [Serializable]
    public sealed class TypeIdentity : IEquatable<TypeIdentity>, IEquatable<Type>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TypeIdentity first, TypeIdentity second)
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
        public static bool operator !=(TypeIdentity first, TypeIdentity second)
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
        /// Creates a new instance of the <see cref="TypeIdentity"/> class based on the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given type.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        public static TypeIdentity CreateDefinition(Type type)
        {
            {
                Lokad.Enforce.Argument(() => type);
            }

            return new TypeIdentity(type);
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
        private readonly AssemblyDefinition m_Assembly;

        /// <summary>
        /// The type definition for the type that declares the current nested type or
        /// generic parameter.
        /// </summary>
        private readonly TypeIdentity m_DeclaringType;

        /// <summary>
        /// The collection that defines all the generic type arguments.
        /// </summary>
        private readonly TypeIdentity[] m_TypeArguments
            = new TypeIdentity[0];

        /// <summary>
        /// A flag indicating that the current type has generic parameter, some of which 
        /// have not been replaced by real types.
        /// </summary>
        private readonly bool m_IsOpenGeneric;

        /// <summary>
        /// A flag indicating that the current type is actually a generic parameter (e.g. T)
        /// for an generic type.
        /// </summary>
        private readonly bool m_IsGenericParameter;

        /// <summary>
        /// A flag indicating if the current type is a nested type or not.
        /// </summary>
        private readonly bool m_IsNested;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeIdentity"/> class.
        /// </summary>
        /// <param name="type">The Type that should be serialized.</param>
        private TypeIdentity(Type type)
        {
            {
                Debug.Assert(type != null, "The type object should not be a null reference.");
            }

            m_Name = type.Name;
            m_Namespace = type.Namespace;
            m_Assembly = AssemblyDefinition.CreateDefinition(type.Assembly);

            m_IsGenericParameter = type.IsGenericParameter;
            if (type.IsGenericParameter)
            {
                // It turns out that if the type is a generic parameter all kinds of crazy stuff happens
                // For instance generic parameters are nested, which means that we'll try to get
                // the identity of the declaring type, which then means we have to get the 
                // generic parameters, which ... que infinite loop. Hence if we are a generic 
                // parameter, then we bail early.
                return;
            }

            m_IsNested = type.IsNested;
            m_IsOpenGeneric = type.ContainsGenericParameters;
            if (type.IsNested)
            {
                m_DeclaringType = new TypeIdentity(type.DeclaringType);
            }

            if (type.IsGenericType)
            {
                // Given that it is not possible for a generic type to use itself as a generic parameter
                // we should be safe from infinite loops here ...
                m_TypeArguments = type.GetGenericArguments().Select(t => new TypeIdentity(t)).ToArray();
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
        /// Gets the assembly qualified name which includes the full name of the assembly that contains the type.
        /// </summary>
        public string AssemblyQualifiedName
        {
            get
            {
                if (m_IsGenericParameter)
                {
                    return null;
                }

                return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", FullName, Assembly);
            }
        }

        /// <summary>
        /// Gets the fully qualified name of the type.
        /// </summary>
        public string FullName
        {
            get
            {
                if (m_IsGenericParameter)
                {
                    return null;
                }

                if (m_IsOpenGeneric)
                {
                    return FormatWithoutTypeParameters();
                }

                return FormatWithTypeParameters(true);
            }
        }

        private string FormatWithoutTypeParameters()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Namespace, FormatNestedName());
        }

        private string FormatWithTypeParameters(bool areTypeParametersFullyQualified)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}{2}",
                Namespace,
                FormatNestedName(),
                FormatTypeParameters(areTypeParametersFullyQualified));
        }

        private string FormatNestedName()
        {
            return m_IsNested
                ? string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}+{1}",
                    m_DeclaringType.FormatNestedName(),
                    Name)
                : Name;
        }

        private string FormatTypeParameters(bool isFullyQualified)
        {
            var builder = new StringBuilder();
            if (m_TypeArguments.Length > 0)
            {
                builder.Append("[");
                for (int i = 0; i < m_TypeArguments.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(",");
                    }

                    var arg = m_TypeArguments[i];
                    if (arg.m_IsGenericParameter)
                    {
                        builder.Append(arg.Name);
                    }
                    else
                    {
                        builder.Append(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "[{0}]",
                                isFullyQualified ? arg.AssemblyQualifiedName : arg.FullName));
                    }
                }

                builder.Append("]");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the assembly information for the type.
        /// </summary>
        public AssemblyDefinition Assembly
        {
            get
            {
                return m_Assembly;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="TypeIdentity"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="TypeIdentity"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="TypeIdentity"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(TypeIdentity other)
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
                && Assembly.Equals(other.Assembly)
                && m_TypeArguments.SequenceEqual(other.m_TypeArguments);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Type"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Type"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="Type"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(Type other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && string.Equals(AssemblyQualifiedName, other.AssemblyQualifiedName, StringComparison.OrdinalIgnoreCase);
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

            var id = obj as TypeIdentity;
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
                foreach (var arg in m_TypeArguments)
                {
                    hash = (hash * 23) ^ arg.GetHashCode();
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
            return FormatWithTypeParameters(false);
        }
    }
}
