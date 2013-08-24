//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.Core.Extensions.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ExecutingActionVertexTest : EqualityContractVerifierTest
    {
        private sealed class ExecutingActionVertexEqualityContractVerifier : EqualityContractVerifier<ExecutingActionVertex>
        {
            private readonly ExecutingActionVertex m_First = new ExecutingActionVertex(0, new ScheduleElementId());

            private readonly ExecutingActionVertex m_Second = new ExecutingActionVertex(1, new ScheduleElementId());

            protected override ExecutingActionVertex Copy(ExecutingActionVertex original)
            {
                return new ExecutingActionVertex(original.Index, original.ActionToExecute);
            }

            protected override ExecutingActionVertex FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ExecutingActionVertex SecondInstance
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

        private sealed class ExecutingActionVertexHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ExecutingActionVertex> m_DistinctInstances
                = new List<ExecutingActionVertex> 
                     {
                        new ExecutingActionVertex(0, new ScheduleElementId()),
                        new ExecutingActionVertex(1, new ScheduleElementId()),
                        new ExecutingActionVertex(2, new ScheduleElementId()),
                        new ExecutingActionVertex(3, new ScheduleElementId()),
                        new ExecutingActionVertex(4, new ScheduleElementId()),
                        new ExecutingActionVertex(5, new ScheduleElementId()),
                        new ExecutingActionVertex(6, new ScheduleElementId()),
                        new ExecutingActionVertex(7, new ScheduleElementId()),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ExecutingActionVertexHashcodeContractVerfier m_HashcodeVerifier = new ExecutingActionVertexHashcodeContractVerfier();

        private readonly ExecutingActionVertexEqualityContractVerifier m_EqualityVerifier = new ExecutingActionVertexEqualityContractVerifier();

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
            var index = 10;
            var actionId = new ScheduleElementId();
            var vertex = new ExecutingActionVertex(index, actionId);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreSame(actionId, vertex.ActionToExecute);
        }
    }
}
