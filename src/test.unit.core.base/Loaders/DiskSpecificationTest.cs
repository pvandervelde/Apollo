//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.Core.Base.Loaders
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class DiskSpecificationTest : EqualityContractVerifierTest
    {
        private sealed class DiskSpecificationEqualityContractVerifier : EqualityContractVerifier<DiskSpecification>
        {
            private readonly DiskSpecification m_First = new DiskSpecification("a", "b", 10, 5);

            private readonly DiskSpecification m_Second = new DiskSpecification("a", "c", 10, 5);

            protected override DiskSpecification Copy(DiskSpecification original)
            {
                return new DiskSpecification(original.Name, original.SerialNumber, original.TotalSpaceInBytes, original.AvailableSpaceInBytes);
            }

            protected override DiskSpecification FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override DiskSpecification SecondInstance
            {
                get
                {
                    return m_Second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class DiskSpecificationHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<DiskSpecification> m_DistinctInstances
                = new List<DiskSpecification> 
                     {
                        new DiskSpecification("a", "b", 10, 5),
                        new DiskSpecification("a", "c", 10, 5),
                        new DiskSpecification("a", "d", 10, 5),
                        new DiskSpecification("a", "e", 10, 5),
                        new DiskSpecification("a", "f", 10, 5),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly DiskSpecificationHashcodeContractVerfier m_HashcodeVerifier = new DiskSpecificationHashcodeContractVerfier();

        private readonly DiskSpecificationEqualityContractVerifier m_EqualityVerifier = new DiskSpecificationEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return m_HashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return m_EqualityVerifier;
            }
        }

        [Test]
        public void Create()
        {
            var name = "a";
            var serial = "b";
            ulong totalSpace = 10;
            ulong freeSpace = 5;
            var disk = new DiskSpecification(name, serial, totalSpace, freeSpace);

            Assert.AreEqual(name, disk.Name);
            Assert.AreEqual(serial, disk.SerialNumber);
            Assert.AreEqual(totalSpace, disk.TotalSpaceInBytes);
            Assert.AreEqual(freeSpace, disk.AvailableSpaceInBytes);
        }
    }
}
