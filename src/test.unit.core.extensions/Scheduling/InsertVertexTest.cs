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
    public sealed class InsertVertexTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<InsertVertex>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<InsertVertex> 
                        {
                            new InsertVertex(0),
                            new InsertVertex(1),
                            new InsertVertex(2),
                            new InsertVertex(3),
                            new InsertVertex(4),
                            new InsertVertex(5),
                            new InsertVertex(6),
                            new InsertVertex(7),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<IScheduleVertex>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new InsertVertex(0),
                        new InsertVertex(1),
                        new InsertVertex(2),
                        new InsertVertex(3),
                        new InsertVertex(4),
                        new InsertVertex(5),
                        new InsertVertex(6),
                        new InsertVertex(7),
                    },
        };

        [Test]
        public void Index()
        {
            var index = 10;
            var vertex = new InsertVertex(index);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreEqual(-1, vertex.RemainingInserts);
        }

        [Test]
        public void InsertCount()
        {
            var index = 10;
            var insertCount = 5;
            var vertex = new InsertVertex(index, insertCount);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreEqual(insertCount, vertex.RemainingInserts);
        }
    }
}
