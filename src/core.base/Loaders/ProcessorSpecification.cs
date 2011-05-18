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
    /// Stores information about one physical CPU.
    /// </summary>
    [Serializable]
    public sealed class ProcessorSpecification : IEquatable<ProcessorSpecification>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ProcessorSpecification first, ProcessorSpecification second)
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
        public static bool operator !=(ProcessorSpecification first, ProcessorSpecification second)
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

        private static ProcessorArchitecture ProcessorArchitectureFromWmiIdentifier(int architectureNumber)
        {
            switch (architectureNumber)
            {
                case 0: return ProcessorArchitecture.x86;
                case 1: return ProcessorArchitecture.Mips;
                case 2: return ProcessorArchitecture.Alpha;
                case 3: return ProcessorArchitecture.PowerPC;
                case 6: return ProcessorArchitecture.Itanium;
                case 9: return ProcessorArchitecture.x64;
                default:
                    return ProcessorArchitecture.Unknown;
            }
        }

        /// <summary>
        /// Returns an array of objects describing all the processors for the current machine.
        /// </summary>
        /// <returns>
        /// An array describing all the processors for the current machine.
        /// </returns>
        public static ProcessorSpecification[] ForLocalMachine()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM Win32_Processor");

                // TODO: some of these may not be available on the certain platforms (e.g. WinXp)
                var results = from ManagementObject queryObj in searcher.Get()
                              select new ProcessorSpecification(
                                    queryObj["Name"] as string,
                                    (ushort)queryObj["AddressWidth"],
                                    ProcessorArchitectureFromWmiIdentifier((ushort)queryObj["Architecture"]),
                                    (uint)queryObj["NumberOfCores"],
                                    (uint)queryObj["NumberOfLogicalProcessors"],
                                    (uint)queryObj["MaxClockSpeed"]);

                return results.ToArray();
            }
            catch (ManagementException)
            {
                return new ProcessorSpecification[0];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorSpecification"/> class.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <param name="addressWidth">The address width for the processor.</param>
        /// <param name="architecture">The architecture of the processor.</param>
        /// <param name="clockSpeed">The clock speed of the processor in MHz.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="addressWidth"/> is smaller or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="clockSpeed"/> is smaller or equal to zero.
        /// </exception>
        public ProcessorSpecification(
            string name,
            int addressWidth,
            ProcessorArchitecture architecture,
            long clockSpeed)
            : this(name, addressWidth, architecture, 1, 1, clockSpeed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorSpecification"/> class.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <param name="addressWidth">The address width for the processor.</param>
        /// <param name="architecture">The architecture of the processor.</param>
        /// <param name="cores">The number of physical cores in the processor.</param>
        /// <param name="logicalProcessors">The number of logical processors in the processor.</param>
        /// <param name="clockSpeed">The clock speed of the processor in MHz.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="addressWidth"/> is smaller or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="cores"/> is smaller or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="logicalProcessors"/> is smaller than <paramref name="cores"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="clockSpeed"/> is smaller or equal to zero.
        /// </exception>
        public ProcessorSpecification(
            string name, 
            int addressWidth, 
            ProcessorArchitecture architecture, 
            long cores, 
            long logicalProcessors, 
            long clockSpeed)
        {
            {
                Enforce.Argument(() => name);
                Enforce.With<ArgumentOutOfRangeException>(addressWidth > 0, Resources.Exceptions_Messages_ProcessorAddressWidthMustBeLargerThanZero);
                Enforce.With<ArgumentOutOfRangeException>(cores > 0, Resources.Exceptions_Messages_NumberOfPhysicalCoresMustBeLargerThanZero);
                Enforce.With<ArgumentOutOfRangeException>(logicalProcessors >= cores, Resources.Exceptions_Messages_NumberOfLogicalProcessorsMustBeLargerOrEqualToNumberOfPhysicalCores);
                Enforce.With<ArgumentOutOfRangeException>(clockSpeed > 0, Resources.Exceptions_Messages_ProcessorClockSpeedMustBeLargerThanZero);
            }

            Name = name;
            AddressWidth = addressWidth;
            Architecture = architecture;
            NumberOfCores = cores;
            NumberOfLogicalProcessors = logicalProcessors;
            ClockSpeedInMHz = clockSpeed;
        }

        /// <summary>
        /// Gets a value indicating the name of the processor.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the address width for the processor.
        /// </summary>
        public int AddressWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the processor architecture.
        /// </summary>
        internal ProcessorArchitecture Architecture
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the number of physical cores on the processor.
        /// </summary>
        public long NumberOfCores
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the number of logical processors for the given
        /// processor.
        /// </summary>
        public long NumberOfLogicalProcessors
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the clock speed in MHz for the given processor.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hz",
            Justification = "Hz is the correct casing for Hertz.")]
        public long ClockSpeedInMHz
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the specified <see cref="ProcessorSpecification"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="ProcessorSpecification"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="ProcessorSpecification"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(ProcessorSpecification other)
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
                && Architecture.Equals(other.Architecture)
                && AddressWidth.Equals(other.AddressWidth)
                && NumberOfCores.Equals(other.NumberOfCores)
                && NumberOfLogicalProcessors.Equals(other.NumberOfLogicalProcessors)
                && ClockSpeedInMHz.Equals(other.ClockSpeedInMHz);
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

            var id = obj as ProcessorSpecification;
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
                hash = (hash * 23) ^ Name.GetHashCode();
                hash = (hash * 23) ^ Architecture.GetHashCode();
                hash = (hash * 23) ^ AddressWidth.GetHashCode();
                hash = (hash * 23) ^ NumberOfCores.GetHashCode();
                hash = (hash * 23) ^ NumberOfLogicalProcessors.GetHashCode();
                hash = (hash * 23) ^ ClockSpeedInMHz.GetHashCode();

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
                "Processor: {0}",
                Name);
        }
    }
}
