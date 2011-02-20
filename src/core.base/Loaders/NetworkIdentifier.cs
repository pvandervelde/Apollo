//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Uniquely identifies a computer on a network.
    /// </summary>
    [Serializable]
    public sealed class NetworkIdentifier : IEquatable<NetworkIdentifier>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(NetworkIdentifier first, NetworkIdentifier second)
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
        public static bool operator !=(NetworkIdentifier first, NetworkIdentifier second)
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
        /// Returns the <see cref="NetworkIdentifier"/> for the local machine.
        /// </summary>
        /// <returns>
        /// The network identifier for the local machine.
        /// </returns>
        public static NetworkIdentifier ForLocalMachine()
        {
            return new NetworkIdentifier(Environment.MachineName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkIdentifier"/> class.
        /// </summary>
        /// <param name="domainName">The domain name of the machine.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="domainName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="domainName"/> is an empty string or a string that only contains whitespace.
        /// </exception>
        public NetworkIdentifier(string domainName)
            : this(domainName, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkIdentifier"/> class.
        /// </summary>
        /// <param name="domainName">The domain name of the machine.</param>
        /// <param name="groupName">The name of the group to which the machine belongs.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="domainName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="domainName"/> is an empty string or a string that only contains whitespace.
        /// </exception>
        public NetworkIdentifier(string domainName, string groupName)
        {
            {
                Enforce.Argument(() => domainName);
                Enforce.With<ArgumentException>(!string.IsNullOrWhiteSpace(domainName), Resources.Exceptions_Messages_MachineDomainNameMustNotBeEmpty);
            }

            DomainName = domainName;
            Group = groupName;
        }

        /// <summary>
        /// Gets a value indicating the domain name for the given machine.
        /// </summary>
        public string DomainName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the given machine belongs to a machine group.
        /// </summary>
        public bool IsPartOfGroup
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Group);
            }
        }

        /// <summary>
        /// Gets a value indicating the name of the group to which the machine belongs.
        /// </summary>
        public string Group
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the given machine is the local machine.
        /// </summary>
        public bool IsLocalMachine
        {
            get 
            {
                return string.Equals(DomainName, Environment.MachineName, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="NetworkIdentifier"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="NetworkIdentifier"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="NetworkIdentifier"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(NetworkIdentifier other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) 
                && string.Equals(DomainName, other.DomainName, StringComparison.OrdinalIgnoreCase) 
                && string.Equals(Group, other.Group, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            var id = obj as NetworkIdentifier;
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
            // As obtained from the Jon Skeet answer to:  http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ DomainName.GetHashCode();
                hash = (hash * 23) ^ Group.GetHashCode();

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
            return string.Format(CultureInfo.InvariantCulture, "Machine {0} [{1}]", DomainName, Group);
        }
    }
}
