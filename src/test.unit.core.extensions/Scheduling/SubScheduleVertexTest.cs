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
    public sealed class SubScheduleVertexTest : EqualityContractVerifierTest
    {
        private sealed class SubScheduleVertexEqualityContractVerifier : EqualityContractVerifier<SubScheduleVertex>
        {
            private readonly SubScheduleVertex m_First = new SubScheduleVertex(0, new ScheduleId());

            private readonly SubScheduleVertex m_Second = new SubScheduleVertex(1, new ScheduleId());

            protected override SubScheduleVertex Copy(SubScheduleVertex original)
            {
                return new SubScheduleVertex(original.Index, original.ScheduleToExecute);
            }

            protected override SubScheduleVertex FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override SubScheduleVertex SecondInstance
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

        private sealed class SubScheduleVertexHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<SubScheduleVertex> m_DistinctInstances
                = new List<SubScheduleVertex> 
                     {
                        new SubScheduleVertex(0, new ScheduleId()),
                        new SubScheduleVertex(1, new ScheduleId()),
                        new SubScheduleVertex(2, new ScheduleId()),
                        new SubScheduleVertex(3, new ScheduleId()),
                        new SubScheduleVertex(4, new ScheduleId()),
                        new SubScheduleVertex(5, new ScheduleId()),
                        new SubScheduleVertex(6, new ScheduleId()),
                        new SubScheduleVertex(7, new ScheduleId()),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly SubScheduleVertexHashcodeContractVerfier m_HashcodeVerifier = new SubScheduleVertexHashcodeContractVerfier();

        private readonly SubScheduleVertexEqualityContractVerifier m_EqualityVerifier = new SubScheduleVertexEqualityContractVerifier();

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
            var subScheduleId = new ScheduleId();
            var vertex = new SubScheduleVertex(index, subScheduleId);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreSame(subScheduleId, vertex.ScheduleToExecute);
        }
    }
}
