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
    [Description("Tests the ProcessorSpecification class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ProcessorSpecificationTest
    {
        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ProcessorSpecification>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ProcessorSpecification> 
                        {
                            new ProcessorSpecification("a", 32, ProcessorArchitecture.x86, 1, 1, 10),
                            new ProcessorSpecification("b", 32, ProcessorArchitecture.x86, 1, 1, 10),
                            new ProcessorSpecification("a", 64, ProcessorArchitecture.x64, 1, 1, 10),
                            new ProcessorSpecification("a", 32, ProcessorArchitecture.x86, 2, 2, 10),
                            new ProcessorSpecification("a", 32, ProcessorArchitecture.x86, 1, 2, 10),
                            new ProcessorSpecification("a", 32, ProcessorArchitecture.x86, 1, 1, 20),
                        },
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<ProcessorSpecification>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new ProcessorSpecification("a", 32, ProcessorArchitecture.x86, 1, 1, 10),
                        new ProcessorSpecification("b", 32, ProcessorArchitecture.x86, 1, 1, 10),
                        new ProcessorSpecification("a", 64, ProcessorArchitecture.x64, 1, 1, 10),
                        new ProcessorSpecification("a", 32, ProcessorArchitecture.x86, 2, 2, 10),
                        new ProcessorSpecification("a", 32, ProcessorArchitecture.x86, 1, 2, 10),
                        new ProcessorSpecification("a", 32, ProcessorArchitecture.x86, 1, 1, 20),
                    },
        };

        [Test]
        [Description("Checks that an object can be constructed.")]
        public void Create()
        {
            var name = "a";
            int addressWidth = 32;
            var architecture = ProcessorArchitecture.PowerPC;
            long cores = 2;
            long logicalProcessors = 4;
            long clockSpeed = 10;
            var processor = new ProcessorSpecification(name, addressWidth, architecture, cores, logicalProcessors, clockSpeed);

            Assert.AreEqual(name, processor.Name);
            Assert.AreEqual(addressWidth, processor.AddressWidth);
            Assert.AreEqual(architecture, processor.Architecture);
            Assert.AreEqual(cores, processor.NumberOfCores);
            Assert.AreEqual(logicalProcessors, processor.NumberOfLogicalProcessors);
            Assert.AreEqual(clockSpeed, processor.ClockSpeedInMHz);
        }
    }
}
