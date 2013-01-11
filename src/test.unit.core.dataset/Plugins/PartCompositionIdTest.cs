//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Dataset.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class PartCompositionIdTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<PartCompositionId>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<PartCompositionId> 
                        {
                            new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("a", 0)),
                            new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("b", 1)),
                            new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("c", 2)),
                            new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("d", 3)),
                            new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("e", 4)),
                            new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("f", 5)),
                            new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("g", 6)),
                            new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("h", 7)),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<PartCompositionId>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("a", 0)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("b", 1)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("c", 2)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("d", 3)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("e", 4)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("f", 5)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("g", 6)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("h", 7)),
                    },
        };

        [Test]
        public void Create()
        {
            var group = new GroupCompositionId();
            var part = new PartRegistrationId("a", 1);
            var id = new PartCompositionId(group, part);

            Assert.AreEqual(group, id.Group);
            Assert.AreEqual(part, id.Part);
        }
    }
}
