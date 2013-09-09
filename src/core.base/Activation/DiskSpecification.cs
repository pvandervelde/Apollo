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

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Stores information about one logical hard-drive.
    /// </summary>
    [Serializable]
    public sealed class DiskSpecification : IEquatable<DiskSpecification>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DiskSpecification first, DiskSpecification second)
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
        public static bool operator !=(DiskSpecification first, DiskSpecification second)
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
        /// Returns an array of information for all the disks in the local machine.
        /// </summary>
        /// <returns>
        /// An array containing all the information about all the fixed disks in the machine.
        /// </returns>
        public static DiskSpecification[] ForLocalMachine()
        {
            try
            {
                // @Todo: Need to allow configurations to block out drives that we can't touch
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM Win32_LogicalDisk");

                // Select all drives that are fixed local disks (drivetype == 3)
                var drives = from ManagementObject queryObj in searcher.Get()
                             where ((uint)queryObj["DriveType"] == 3)
                             select new DiskSpecification(
                                    queryObj["Caption"] as string,
                                    queryObj["VolumeSerialNumber"] as string,
                                    (ulong)queryObj["Size"],
                                    (ulong)queryObj["FreeSpace"]);

                return drives.ToArray();
            }
            catch (ManagementException)
            {
                return new DiskSpecification[0];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskSpecification"/> class.
        /// </summary>
        /// <param name="serialNumber">The serial number of the disk.</param>
        /// <param name="totalSpace">The total amount of space on the disk in bytes.</param>
        /// <param name="availableSpace">The available space on the disk in bytes.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="serialNumber"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="serialNumber"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="totalSpace"/> is smaller than <paramref name="availableSpace"/>.
        /// </exception>
        [CLSCompliant(false)]
        public DiskSpecification(string serialNumber, ulong totalSpace, ulong availableSpace)
            : this(string.Empty, serialNumber, totalSpace, availableSpace)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskSpecification"/> class.
        /// </summary>
        /// <param name="name">The name of the disk.</param>
        /// <param name="serialNumber">The serial number of the disk.</param>
        /// <param name="totalSpace">The total amount of space on the disk in bytes.</param>
        /// <param name="availableSpace">The available space on the disk in bytes.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="serialNumber"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="serialNumber"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="totalSpace"/> is smaller than <paramref name="availableSpace"/>.
        /// </exception>
        [CLSCompliant(false)]
        public DiskSpecification(string name, string serialNumber, ulong totalSpace, ulong availableSpace)
        {
            {
                Enforce.Argument(() => serialNumber);
                Enforce.With<ArgumentException>(
                    !string.IsNullOrWhiteSpace(serialNumber), 
                    Resources.Exceptions_Messages_ADiskNeedsToHaveASerial);
                Enforce.With<ArgumentException>(
                    totalSpace >= availableSpace, 
                    Resources.Exceptions_Messages_ADiskCannotHaveMoreFreeSpaceThanTotalSpace);
            }

            Name = name;
            SerialNumber = serialNumber;
            TotalSpaceInBytes = totalSpace;
            AvailableSpaceInBytes = availableSpace;
        }

        /// <summary>
        /// Gets a value indicating the name of the logical disk.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the serial number of the logical disk.
        /// </summary>
        public string SerialNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the total space of the logical disk.
        /// </summary>
        [CLSCompliant(false)]
        public ulong TotalSpaceInBytes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the available space of the logical disk.
        /// </summary>
        [CLSCompliant(false)]
        public ulong AvailableSpaceInBytes
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the specified <see cref="DiskSpecification"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="DiskSpecification"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="DiskSpecification"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(DiskSpecification other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && string.Equals(SerialNumber, other.SerialNumber, StringComparison.OrdinalIgnoreCase);
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

            var id = obj as DiskSpecification;
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
                hash = (hash * 23) ^ SerialNumber.GetHashCode();

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
                "Disk[Volume: {0}] {1}. Capacity (free / total) {2} / {3}",
                SerialNumber,
                Name,
                AvailableSpaceInBytes,
                TotalSpaceInBytes);
        }
    }
}
