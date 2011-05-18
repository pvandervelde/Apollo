//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Management;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines the hardware specifications of a given machine.
    /// </summary>
    [Serializable]
    public sealed class HardwareSpecification
    {
        /// <summary>
        /// Returns the amount of physical memory installed in the machine in bytes.
        /// </summary>
        /// <returns>
        /// The amount of physical memory of the local machine in bytes.
        /// </returns>
        private static ulong LocalMachinePhysicalMemorySize()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM Win32_PhysicalMemory");

                ulong total = 0;
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    total += (ulong)queryObj["Capacity"];
                }

                return total;
            }
            catch (ManagementException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the hardware specification for the local machine.
        /// </summary>
        /// <returns>
        /// The hardware specification for the local machine.
        /// </returns>
        public static HardwareSpecification ForLocalMachine()
        {
            ulong memory = LocalMachinePhysicalMemorySize();
            var processors = ProcessorSpecification.ForLocalMachine();
            var disks = DiskSpecification.ForLocalMachine();
            var networks = NetworkSpecification.ForLocalMachine();

            return new HardwareSpecification(memory, processors, disks, networks);
        }

        /// <summary>
        /// The objects that describe the specifications for the 
        /// available processors.
        /// </summary>
        private readonly ProcessorSpecification[] m_Processors;

        /// <summary>
        /// The objects that describe the specification for the
        /// available disks.
        /// </summary>
        private readonly DiskSpecification[] m_Disks;

        /// <summary>
        /// The objects that describe the specification for the
        /// available network connections.
        /// </summary>
        private readonly NetworkSpecification[] m_Networks;

        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareSpecification"/> class.
        /// </summary>
        /// <param name="memoryInBytes">The amount of RAM memory in bytes.</param>
        /// <param name="processors">The information describing the available processors.</param>
        /// <param name="disks">The information describing the available disks.</param>
        /// <param name="networks">The information describing the available networks.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="processors"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="processors"/> an empty array.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="disks"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="disks"/> is an empty array.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="networks"/> is <see langword="null" />.
        /// </exception>
        [CLSCompliant(false)]
        public HardwareSpecification(
            ulong memoryInBytes,
            ProcessorSpecification[] processors,
            DiskSpecification[] disks,
            NetworkSpecification[] networks)
        {
            {
                Enforce.Argument(() => processors);
                Enforce.With<ArgumentException>(processors.Length > 0, Resources.Exceptions_Messages_AMachineNeedsAtLeastOneProcessor);

                Enforce.Argument(() => disks);
                Enforce.With<ArgumentException>(disks.Length > 0, Resources.Exceptions_Messages_AMachineNeedsAtLeastOneDisk);

                Enforce.Argument(() => networks);
            }

            MemoryInBytes = memoryInBytes;
            m_Processors = processors;
            m_Disks = disks;
            m_Networks = networks;
        }

        /// <summary>
        /// Gets a value indicating the amount of RAM memory in bytes for the given machine.
        /// </summary>
        [CLSCompliant(false)]
        public ulong MemoryInBytes
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the collection of objects describing the available 
        /// processors.
        /// </summary>
        /// <returns>
        /// The collection describing the available processors.
        /// </returns>
        public IEnumerable<ProcessorSpecification> Processors()
        {
            return m_Processors;
        }

        /// <summary>
        /// Returns the collection of objects describing the available 
        /// disks.
        /// </summary>
        /// <returns>
        /// The collection describing the available disk.
        /// </returns>
        public IEnumerable<DiskSpecification> Disks()
        {
            return m_Disks;
        }

        /// <summary>
        /// Returns the collection of objects describing the available 
        /// network connections.
        /// </summary>
        /// <returns>
        /// The collection describing the available network connections.
        /// </returns>
        public IEnumerable<NetworkSpecification> Network()
        {
            return m_Networks;
        }
    }
}
