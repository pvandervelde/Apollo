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
        private static readonly ProcessorSpecification[] s_Processors = ProcessorSpecification.ForLocalMachine();
        private static readonly DiskSpecification[] s_Disks = DiskSpecification.ForLocalMachine();
        private static readonly NetworkSpecification[] s_Networks = NetworkSpecification.ForLocalMachine();

        [Test]
        [Description("Checks that an object cannot be constructed with a null processors collection.")]
        public void CreateWithNullProcessorsCollection()
        {
            Assert.Throws<ArgumentNullException>(() => new HardwareSpecification(10, null, s_Disks, s_Networks));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with an empty processors collection.")]
        public void CreateWithEmptyProcessorsCollection()
        {
            Assert.Throws<ArgumentException>(() => new HardwareSpecification(10, new ProcessorSpecification[0], s_Disks, s_Networks));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with a null disks collection.")]
        public void CreateWithNullDisksCollection()
        {
            Assert.Throws<ArgumentNullException>(() => new HardwareSpecification(10, s_Processors, null, s_Networks));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with an empty disk collection.")]
        public void CreateWithEmptyDisksCollection()
        {
            Assert.Throws<ArgumentException>(() => new HardwareSpecification(10, s_Processors, new DiskSpecification[0], s_Networks));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with a null network collection.")]
        public void CreateWithNullNetworkCollection()
        {
            Assert.Throws<ArgumentNullException>(() => new HardwareSpecification(10, s_Processors, s_Disks, null));
        }

        [Test]
        [Description("Checks that an object can be created.")]
        public void Create()
        {
            ulong memory = 10;
            var processors = s_Processors;
            var disks = s_Disks;
            var network = s_Networks;

            var hardware = new HardwareSpecification(memory, processors, disks, network);

            Assert.AreEqual(memory, hardware.MemoryInBytes);
            Assert.AreSame(processors, hardware.Processors());
            Assert.AreSame(disks, hardware.Disks());
            Assert.AreSame(network, hardware.Network());
        }
    }
}
