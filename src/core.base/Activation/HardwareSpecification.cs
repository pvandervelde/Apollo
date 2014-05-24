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

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Defines the hardware specifications of a given machine.
    /// </summary>
    [Serializable]
    public sealed class HardwareSpecification
    {
        private sealed class MemoryInformation
        {
            public ulong MaximumProcessMemorySizeInKiloBytes
            {
                get;
                set;
            }

            public ulong TotalPhysicalMemorySizeInKiloBytes
            {
                get;
                set;
            }

            public ulong TotalVirtualMemorySizeInKiloBytes
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Returns the amount of physical memory installed in the machine in bytes.
        /// </summary>
        /// <returns>
        /// A tuple containing the maximum amount of memory a process can use, the total
        /// amount of physical memory available to the operating system and the total
        /// amount of virtual memory the operating system provides.
        /// </returns>
        private static MemoryInformation LocalMachinePhysicalMemorySize()
        {
            try
            {
                ulong maxPerProcess = 0;
                ulong totalPhysical = 0;
                ulong totalVirtual = 0;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM CIM_OperatingSystem"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        maxPerProcess += (ulong)queryObj["MaxProcessMemorySize"];
                        totalPhysical += (ulong)queryObj["TotalVisibleMemorySize"];
                        totalVirtual += (ulong)queryObj["TotalVirtualMemorySize"];
                    }
                }

                return new MemoryInformation
                    {
                        MaximumProcessMemorySizeInKiloBytes = maxPerProcess,
                        TotalPhysicalMemorySizeInKiloBytes = totalPhysical,
                        TotalVirtualMemorySizeInKiloBytes = totalVirtual,
                    };
            }
            catch (ManagementException)
            {
                return new MemoryInformation();
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
            var memorySizes = LocalMachinePhysicalMemorySize();
            var disks = DiskSpecification.ForLocalMachine();

            return new HardwareSpecification(
                memorySizes.MaximumProcessMemorySizeInKiloBytes, 
                memorySizes.TotalPhysicalMemorySizeInKiloBytes, 
                memorySizes.TotalVirtualMemorySizeInKiloBytes, 
                disks);
        }

        /// <summary>
        /// The objects that describe the specification for the
        /// available disks.
        /// </summary>
        private readonly DiskSpecification[] m_Disks;

        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareSpecification"/> class.
        /// </summary>
        /// <param name="maxPerProcessMemory">The maximum amount of memory available to a process in Kb.</param>
        /// <param name="totalPhysicalMemory">The total amount of physical memory as reported by the OS in Kb.</param>
        /// <param name="totalVirtualMemory">The total amount of virtual memory the OS provides in Kb.</param>
        /// <param name="disks">The information describing the available disks.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="disks"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="disks"/> is an empty array.
        /// </exception>
        [CLSCompliant(false)]
        public HardwareSpecification(
            ulong maxPerProcessMemory,
            ulong totalPhysicalMemory,
            ulong totalVirtualMemory,
            DiskSpecification[] disks)
        {
            {
                Enforce.Argument(() => disks);
                Enforce.With<ArgumentException>(disks.Length > 0, Resources.Exceptions_Messages_AMachineNeedsAtLeastOneDisk);
            }

            PerProcessMemoryInKilobytes = maxPerProcessMemory;
            TotalPhysicalMemoryInKilobytes = totalPhysicalMemory;
            TotalVirtualMemoryInKilobytes = totalVirtualMemory;
            m_Disks = disks;
        }

        /// <summary>
        /// Gets a value indicating the maximum amount of memory a process can use.
        /// </summary>
        [CLSCompliant(false)]
        public ulong PerProcessMemoryInKilobytes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the total amount of memory the operating system reports.
        /// </summary>
        [CLSCompliant(false)]
        public ulong TotalPhysicalMemoryInKilobytes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the amount of virtual memory reported by the operating system.
        /// </summary>
        [CLSCompliant(false)]
        public ulong TotalVirtualMemoryInKilobytes
        {
            get;
            private set;
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
    }
}
