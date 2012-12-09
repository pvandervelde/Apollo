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
    public sealed class SubScheduleVertexTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SubScheduleVertex>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SubScheduleVertex> 
                        {
                            new SubScheduleVertex(0, new ScheduleId()),
                            new SubScheduleVertex(1, new ScheduleId()),
                            new SubScheduleVertex(2, new ScheduleId()),
                            new SubScheduleVertex(3, new ScheduleId()),
                            new SubScheduleVertex(4, new ScheduleId()),
                            new SubScheduleVertex(5, new ScheduleId()),
                            new SubScheduleVertex(6, new ScheduleId()),
                            new SubScheduleVertex(7, new ScheduleId()),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<IScheduleVertex>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new SubScheduleVertex(0, new ScheduleId()),
                        new SubScheduleVertex(1, new ScheduleId()),
                        new SubScheduleVertex(2, new ScheduleId()),
                        new SubScheduleVertex(3, new ScheduleId()),
                        new SubScheduleVertex(4, new ScheduleId()),
                        new SubScheduleVertex(5, new ScheduleId()),
                        new SubScheduleVertex(6, new ScheduleId()),
                        new SubScheduleVertex(7, new ScheduleId()),
                    },
        };

        [Test]
        public void Create()
        {
            var index = 10;
            var subScheduleId = new ScheduleId();
            var vertex = new SubScheduleVertex(index, subScheduleId);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreSame(subScheduleId, vertex.ScheduleToExecute);
        }
    }
}
