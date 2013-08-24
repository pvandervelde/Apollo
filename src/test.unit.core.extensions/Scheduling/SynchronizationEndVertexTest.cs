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
    public sealed class SynchronizationEndVertexTest : EqualityContractVerifierTest
    {
        private sealed class SynchronizationEndVertexEqualityContractVerifier : EqualityContractVerifier<SynchronizationEndVertex>
        {
            private readonly SynchronizationEndVertex m_First = new SynchronizationEndVertex(0);

            private readonly SynchronizationEndVertex m_Second = new SynchronizationEndVertex(1);

            protected override SynchronizationEndVertex Copy(SynchronizationEndVertex original)
            {
                return new SynchronizationEndVertex(original.Index);
            }

            protected override SynchronizationEndVertex FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override SynchronizationEndVertex SecondInstance
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

        private sealed class SynchronizationEndVertexHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<SynchronizationEndVertex> m_DistinctInstances
                = new List<SynchronizationEndVertex> 
                     {
                        new SynchronizationEndVertex(0),
                        new SynchronizationEndVertex(1),
                        new SynchronizationEndVertex(2),
                        new SynchronizationEndVertex(3),
                        new SynchronizationEndVertex(4),
                        new SynchronizationEndVertex(5),
                        new SynchronizationEndVertex(6),
                        new SynchronizationEndVertex(7),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly SynchronizationEndVertexHashcodeContractVerfier m_HashcodeVerifier = new SynchronizationEndVertexHashcodeContractVerfier();

        private readonly SynchronizationEndVertexEqualityContractVerifier m_EqualityVerifier = new SynchronizationEndVertexEqualityContractVerifier();

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
            var vertex = new SynchronizationEndVertex(index);

            Assert.AreEqual(index, vertex.Index);
        }
    }
}
