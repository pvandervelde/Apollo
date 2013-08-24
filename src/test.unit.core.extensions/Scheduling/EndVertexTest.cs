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
    public sealed class EndVertexTest : EqualityContractVerifierTest
    {
        private sealed class EndVertexEqualityContractVerifier : EqualityContractVerifier<EndVertex>
        {
            private readonly EndVertex m_First = new EndVertex(0);

            private readonly EndVertex m_Second = new EndVertex(1);

            protected override EndVertex Copy(EndVertex original)
            {
                return new EndVertex(original.Index);
            }

            protected override EndVertex FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override EndVertex SecondInstance
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

        private sealed class EndVertexHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<EndVertex> m_DistinctInstances
                = new List<EndVertex> 
                     {
                        new EndVertex(0),
                        new EndVertex(1),
                        new EndVertex(2),
                        new EndVertex(3),
                        new EndVertex(4),
                        new EndVertex(5),
                        new EndVertex(6),
                        new EndVertex(7),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly EndVertexHashcodeContractVerfier m_HashcodeVerifier = new EndVertexHashcodeContractVerfier();

        private readonly EndVertexEqualityContractVerifier m_EqualityVerifier = new EndVertexEqualityContractVerifier();

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
        public void Index()
        {
            var index = 10;
            var vertex = new EndVertex(index);

            Assert.AreEqual(index, vertex.Index);
        }
    }
}
