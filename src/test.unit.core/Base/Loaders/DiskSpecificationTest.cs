//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Loaders;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Base.Loaders
{
    [TestFixture]
    [Description("Tests the DiskSpecification class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class DiskSpecificationTest
    {
        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<DiskSpecification>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<DiskSpecification> 
                        {
                            new DiskSpecification("a", "b", 10, 5),
                            new DiskSpecification("a", "c", 10, 5),
                            new DiskSpecification("a", "d", 10, 5),
                            new DiskSpecification("a", "e", 10, 5),
                            new DiskSpecification("a", "f", 10, 5),
                        },
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<DiskSpecification>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new DiskSpecification("a", "b", 10, 5),
                        new DiskSpecification("a", "c", 10, 5),
                        new DiskSpecification("a", "d", 10, 5),
                        new DiskSpecification("a", "e", 10, 5),
                        new DiskSpecification("a", "f", 10, 5),
                    },
        };

        [Test]
        [Description("Checks that an object cannot be constructed without a serial number.")]
        public void CreateWithNullSerial()
        {
            Assert.Throws<ArgumentNullException>(() => new DiskSpecification(null, 10, 5));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with an empty string as serial number.")]
        public void CreateWithEmptySerial()
        {
            Assert.Throws<ArgumentException>(() => new DiskSpecification(string.Empty, 10, 5));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with more free space than total space.")]
        public void CreateWithMoreFreeSpacenThanTotalSpace()
        {
            Assert.Throws<ArgumentException>(() => new DiskSpecification("a", 5, 10));
        }

        [Test]
        [Description("Checks that an object can be constructed.")]
        public void Create()
        {
            var name = "a";
            var serial = "b";
            ulong totalSpace = 10;
            ulong freeSpace = 5;
            var disk = new DiskSpecification(name, serial, totalSpace, freeSpace);

            Assert.AreEqual(name, disk.Name);
            Assert.AreEqual(serial, disk.SerialNumber);
            Assert.AreEqual(totalSpace, disk.TotalSpace);
            Assert.AreEqual(freeSpace, disk.AvailableSpace);
        }
    }
}
