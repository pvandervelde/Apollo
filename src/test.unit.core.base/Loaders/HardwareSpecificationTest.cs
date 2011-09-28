//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.Base.Loaders
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class HardwareSpecificationTest
    {
        // Cache the WMI values for better performance
        private static readonly DiskSpecification[] s_Disks = DiskSpecification.ForLocalMachine();

        [Test]
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
