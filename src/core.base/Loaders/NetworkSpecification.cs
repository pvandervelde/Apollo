//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Management;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Stores information about one physical network connection.
    /// </summary>
    [Serializable]
    public sealed class NetworkSpecification : IEquatable<NetworkSpecification>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(NetworkSpecification first, NetworkSpecification second)
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
        public static bool operator !=(NetworkSpecification first, NetworkSpecification second)
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
        /// Returns an array of objects describing the different physical network connections
        /// for the local machine.
        /// </summary>
        /// <returns>
        /// An array containing the information about the different physical network connections
        /// in the local machine.
        /// </returns>
        public static NetworkSpecification[] ForLocalMachine()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM Win32_NetworkAdapter");

                var networks = from ManagementObject queryObj in searcher.Get()
                               where ((bool)queryObj["PhysicalAdapter"])
                               select new NetworkSpecification(
                                    queryObj["Name"] as string,
                                    queryObj["MACAddress"] as string,
                                    (bool)queryObj["NetEnabled"],
                                    (ulong)queryObj["Speed"]);

                return networks.ToArray();
            }
            catch (ManagementException)
            {
                return new NetworkSpecification[0];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSpecification"/> class.
        /// </summary>
        /// <param name="macAddress">The MAC address of the network card.</param>
        /// <param name="isConnected">
        ///     <see langword="true" /> if the card is connected to a physical network;
        ///     otherwise, <see langword="false" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="macAddress"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="macAddress"/> is an empty string.
        /// </exception>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public NetworkSpecification(string macAddress, bool isConnected)
            : this(string.Empty, macAddress, isConnected, 0)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSpecification"/> class.
        /// </summary>
        /// <param name="name">The name of the network connection.</param>
        /// <param name="macAddress">The MAC address of the network card.</param>
        /// <param name="isConnected">
        ///     <see langword="true" /> if the card is connected to a physical network;
        ///     otherwise, <see langword="false" />.
        /// </param>
        /// <param name="speed">The speed of the network in bits per second.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="macAddress"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="macAddress"/> is an empty string.
        /// </exception>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        [CLSCompliant(false)]
        public NetworkSpecification(string name, string macAddress, bool isConnected, ulong speed)
        {
            {
                Enforce.Argument(() => name);
                Enforce.Argument(() => macAddress);
                Enforce.With<ArgumentException>(
                    !string.IsNullOrWhiteSpace(macAddress),
                    Resources.Exceptions_Messages_ANetworkConnectionNeedsAValidMacAddress);
            }

            Name = name;
            MacAddress = macAddress;
            IsConnected = isConnected;
            SpeedInBitsPerSecond = speed;
        }

        /// <summary>
        /// Gets the name of the physical network connector.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the MAC address of the physical network connector.
        /// </summary>
        public string MacAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether if the connector is actually
        /// connected to an external network.
        /// </summary>
        public bool IsConnected
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the speed of the network in bits per second.
        /// </summary>
        [CLSCompliant(false)]
        public ulong SpeedInBitsPerSecond
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the specified <see cref="NetworkSpecification"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="NetworkSpecification"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="NetworkSpecification"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(NetworkSpecification other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && string.Equals(MacAddress, other.MacAddress, StringComparison.OrdinalIgnoreCase);
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

            var id = obj as NetworkSpecification;
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
                hash = (hash * 23) ^ MacAddress.GetHashCode();

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
                "Network connector (MAC: {0})",
                MacAddress);
        }
    }
}
