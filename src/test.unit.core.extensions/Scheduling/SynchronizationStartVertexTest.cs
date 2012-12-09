//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Moq;

namespace Apollo.Core.Extensions.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SynchronizationStartVertexTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SynchronizationStartVertex>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SynchronizationStartVertex> 
                        {
                            new SynchronizationStartVertex(0, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                            new SynchronizationStartVertex(1, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                            new SynchronizationStartVertex(2, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                            new SynchronizationStartVertex(3, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                            new SynchronizationStartVertex(4, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                            new SynchronizationStartVertex(5, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                            new SynchronizationStartVertex(6, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                            new SynchronizationStartVertex(7, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<IScheduleVertex>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new SynchronizationStartVertex(0, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(1, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(2, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(3, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(4, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(5, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(6, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(7, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                    },
        };

        [Test]
        public void Create()
        {
            var variable = new Mock<IScheduleVariable>();

            var index = 10;
            var variables = new List<IScheduleVariable> { variable.Object };
            var vertex = new SynchronizationStartVertex(index, variables);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreSame(variables, vertex.VariablesToSynchronizeOn);
        }
    }
}
