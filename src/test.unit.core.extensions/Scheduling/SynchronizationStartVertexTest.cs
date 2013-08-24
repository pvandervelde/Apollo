//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Moq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.Core.Extensions.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SynchronizationStartVertexTest : EqualityContractVerifierTest
    {
        private sealed class SynchronizationStartVertexEqualityContractVerifier : EqualityContractVerifier<SynchronizationStartVertex>
        {
            private readonly SynchronizationStartVertex m_First 
                = new SynchronizationStartVertex(0, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object });

            private readonly SynchronizationStartVertex m_Second 
                = new SynchronizationStartVertex(1, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object });

            protected override SynchronizationStartVertex Copy(SynchronizationStartVertex original)
            {
                return new SynchronizationStartVertex(original.Index, original.VariablesToSynchronizeOn);
            }

            protected override SynchronizationStartVertex FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override SynchronizationStartVertex SecondInstance
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

        private sealed class SynchronizationStartVertexHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<SynchronizationStartVertex> m_DistinctInstances
                = new List<SynchronizationStartVertex> 
                     {
                        new SynchronizationStartVertex(0, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(1, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(2, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(3, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(4, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(5, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(6, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                        new SynchronizationStartVertex(7, new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object }),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly SynchronizationStartVertexHashcodeContractVerfier m_HashcodeVerifier 
            = new SynchronizationStartVertexHashcodeContractVerfier();

        private readonly SynchronizationStartVertexEqualityContractVerifier m_EqualityVerifier 
            = new SynchronizationStartVertexEqualityContractVerifier();

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
            var variable = new Mock<IScheduleVariable>();

            var index = 10;
            var variables = new List<IScheduleVariable> { variable.Object };
            var vertex = new SynchronizationStartVertex(index, variables);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreSame(variables, vertex.VariablesToSynchronizeOn);
        }
    }
}
