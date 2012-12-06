//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Extensions.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class EndVertexTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<EndVertex>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<EndVertex> 
                        {
                            new EndVertex(0),
                            new EndVertex(1),
                            new EndVertex(2),
                            new EndVertex(3),
                            new EndVertex(4),
                            new EndVertex(5),
                            new EndVertex(6),
                            new EndVertex(7),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<IScheduleVertex>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new EndVertex(0),
                        new EndVertex(1),
                        new EndVertex(2),
                        new EndVertex(3),
                        new EndVertex(4),
                        new EndVertex(5),
                        new EndVertex(6),
                        new EndVertex(7),
                    },
        };

        [Test]
        public void Index()
        {
            var index = 10;
            var vertex = new EndVertex(index);

            Assert.AreEqual(index, vertex.Index);
        }
    }
}
