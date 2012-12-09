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
    public sealed class ExecutingActionVertexTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ExecutingActionVertex>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ExecutingActionVertex> 
                        {
                            new ExecutingActionVertex(0, new ScheduleElementId()),
                            new ExecutingActionVertex(1, new ScheduleElementId()),
                            new ExecutingActionVertex(2, new ScheduleElementId()),
                            new ExecutingActionVertex(3, new ScheduleElementId()),
                            new ExecutingActionVertex(4, new ScheduleElementId()),
                            new ExecutingActionVertex(5, new ScheduleElementId()),
                            new ExecutingActionVertex(6, new ScheduleElementId()),
                            new ExecutingActionVertex(7, new ScheduleElementId()),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<IScheduleVertex>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new ExecutingActionVertex(0, new ScheduleElementId()),
                        new ExecutingActionVertex(1, new ScheduleElementId()),
                        new ExecutingActionVertex(2, new ScheduleElementId()),
                        new ExecutingActionVertex(3, new ScheduleElementId()),
                        new ExecutingActionVertex(4, new ScheduleElementId()),
                        new ExecutingActionVertex(5, new ScheduleElementId()),
                        new ExecutingActionVertex(6, new ScheduleElementId()),
                        new ExecutingActionVertex(7, new ScheduleElementId()),
                    },
        };

        [Test]
        public void Create()
        {
            var index = 10;
            var actionId = new ScheduleElementId();
            var vertex = new ExecutingActionVertex(index, actionId);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreSame(actionId, vertex.ActionToExecute);
        }
    }
}
