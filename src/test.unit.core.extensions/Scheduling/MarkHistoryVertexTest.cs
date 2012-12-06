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
    public sealed class MarkHistoryVertexTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<MarkHistoryVertex>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<MarkHistoryVertex> 
                        {
                            new MarkHistoryVertex(0),
                            new MarkHistoryVertex(1),
                            new MarkHistoryVertex(2),
                            new MarkHistoryVertex(3),
                            new MarkHistoryVertex(4),
                            new MarkHistoryVertex(5),
                            new MarkHistoryVertex(6),
                            new MarkHistoryVertex(7),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<IScheduleVertex>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new MarkHistoryVertex(0),
                        new MarkHistoryVertex(1),
                        new MarkHistoryVertex(2),
                        new MarkHistoryVertex(3),
                        new MarkHistoryVertex(4),
                        new MarkHistoryVertex(5),
                        new MarkHistoryVertex(6),
                        new MarkHistoryVertex(7),
                    },
        };

        [Test]
        public void Index()
        {
            var index = 10;
            var vertex = new MarkHistoryVertex(index);

            Assert.AreEqual(index, vertex.Index);
        }
    }
}
