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
    public sealed class MachineDistributionRangeTest
    {
        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<MachineDistributionRange>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<MachineDistributionRange> 
                        {
                            new MachineDistributionRange(1, 5),
                            new MachineDistributionRange(2, 5),
                            new MachineDistributionRange(1, 6),
                        },
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<MachineDistributionRange>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new MachineDistributionRange(1, 5),
                        new MachineDistributionRange(2, 5),
                        new MachineDistributionRange(1, 6),
                    },
        };

        [Test]
        [Description("Checks that an object can be constructed with the standard values.")]
        public void CreateDefault()
        {
            var range = new MachineDistributionRange();
            Assert.AreEqual(1, range.Minimum);
            Assert.AreEqual(1, range.Maximum);
        }

        [Test]
        [Description("Checks that an object can be constructed with only a maximum value.")]
        public void CreateWithMaximumOnly()
        {
            var maximum = 5;
            var range = new MachineDistributionRange(maximum);
            
            Assert.AreEqual(1, range.Minimum);
            Assert.AreEqual(maximum, range.Maximum);
        }

        [Test]
        [Description("Checks that an object can be constructed with a minimum and a maximum value.")]
        public void CreateWithMinimumAndMaximum()
        {
            var minimum = 2;
            var maximum = 5;
            var range = new MachineDistributionRange(minimum, maximum);

            Assert.AreEqual(minimum, range.Minimum);
            Assert.AreEqual(maximum, range.Maximum);
        }
    }
}
