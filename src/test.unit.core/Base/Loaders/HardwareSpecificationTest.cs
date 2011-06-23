//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Loaders;
using MbUnit.Framework;

namespace Apollo.Base.Loaders
{
    [TestFixture]
    [Description("Tests the HardwareSpecification class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class HardwareSpecificationTest
    {
        // Cache the WMI values for better performance
        private static readonly DiskSpecification[] s_Disks = DiskSpecification.ForLocalMachine();

        [Test]
        [Description("Checks that an object can be created.")]
        public void Create()
        {
            ulong maxPerProcessMemory = 10;
            ulong totalPhysicalMemory = 9;
            ulong totalVirtualMemory = 11;
            var disks = s_Disks;

            var hardware = new HardwareSpecification(maxPerProcessMemory, totalPhysicalMemory, totalVirtualMemory, disks);

            Assert.AreEqual(maxPerProcessMemory, hardware.PerProcessMemoryInKilobytes);
            Assert.AreEqual(totalPhysicalMemory, hardware.TotalPhysicalMemoryInKilobytes);
            Assert.AreEqual(totalVirtualMemory, hardware.TotalVirtualMemoryInKilobytes);
            Assert.AreSame(disks, hardware.Disks());
        }
    }
}
