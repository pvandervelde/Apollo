﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Stores information about an assembly in a serializable form, i.e. without requiring the
    /// assembly in question to be loaded.
    /// </summary>
    [Serializable]
    public sealed class AssemblyDefinition : IEquatable<AssemblyDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(AssemblyDefinition first, AssemblyDefinition second)
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
        public static bool operator !=(AssemblyDefinition first, AssemblyDefinition second)
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
        /// Creates a new instance of the <see cref="AssemblyDefinition"/> class based on the given <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given assembly.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assembly"/> is <see langword="null" />.
        /// </exception>
        public static AssemblyDefinition CreateDefinition(Assembly assembly)
        {
            return new AssemblyDefinition(assembly);
        }

        /// <summary>
        /// The name of the assembly.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// The version of the assembly.
        /// </summary>
        private readonly Version m_Version;

        /// <summary>
        /// The culture of the assembly.
        /// </summary>
        private readonly CultureInfo m_Culture;

        /// <summary>
        /// The public key token of the assembly.
        /// </summary>
        private readonly string m_PublicKeyToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyDefinition"/> class.
        /// </summary>
        /// <param name="assembly">The assembly for which the data should be stored.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assembly"/> is <see langword="null" />.
        /// </exception>
        private AssemblyDefinition(Assembly assembly)
        {
            {
                Lokad.Enforce.Argument(() => assembly);
            }

            var assemblyName = assembly.GetName();
            var publicKeyToken = string.Join(
                string.Empty, 
                assemblyName.GetPublicKeyToken().Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));

            m_Name = assemblyName.Name;
            m_Version = assemblyName.Version;
            m_Culture = assemblyName.CultureInfo;
            m_PublicKeyToken = publicKeyToken;
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the version of the assembly.
        /// </summary>
        public Version Version
        {
            get
            {
                return m_Version;
            }
        }

        /// <summary>
        /// Gets the culture for the assembly.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return m_Culture;
            }
        }

        /// <summary>
        /// Gets the public key token for the assembly.
        /// </summary>
        public string PublicKeyToken
        {
            get
            {
                return m_PublicKeyToken;
            }
        }

        /// <summary>
        /// Gets the full name of the assembly.
        /// </summary>
        public string FullName
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}, Version={1}, Culture={2}, PublicKeyToken={3}",
                    Name,
                    Version,
                    !Culture.Equals(CultureInfo.InvariantCulture) ? Culture.Name : "neutral",
                    PublicKeyToken);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="AssemblyDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="AssemblyDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="AssemblyDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(AssemblyDefinition other)
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
                && Version.Equals(other.Version)
                && Culture.Equals(other.Culture)
                && string.Equals(PublicKeyToken, other.PublicKeyToken, StringComparison.OrdinalIgnoreCase);
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

            var id = obj as AssemblyDefinition;
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
                hash = (hash * 23) ^ Version.GetHashCode();
                hash = (hash * 23) ^ Culture.GetHashCode();
                if (PublicKeyToken != null)
                {
                    hash = (hash * 23) ^ PublicKeyToken.GetHashCode();
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
            return FullName;
        }
    }
}
