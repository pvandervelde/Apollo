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
    public sealed class StartVertexTest : EqualityContractVerifierTest
    {
        private sealed class StartVertexEqualityContractVerifier : EqualityContractVerifier<StartVertex>
        {
            private readonly StartVertex m_First = new StartVertex(0);

            private readonly StartVertex m_Second = new StartVertex(1);

            protected override StartVertex Copy(StartVertex original)
            {
                return new StartVertex(original.Index);
            }

            protected override StartVertex FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override StartVertex SecondInstance
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

        private sealed class StartVertexHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<StartVertex> m_DistinctInstances
                = new List<StartVertex> 
                     {
                        new StartVertex(0),
                        new StartVertex(1),
                        new StartVertex(2),
                        new StartVertex(3),
                        new StartVertex(4),
                        new StartVertex(5),
                        new StartVertex(6),
                        new StartVertex(7),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly StartVertexHashcodeContractVerfier m_HashcodeVerifier = new StartVertexHashcodeContractVerfier();

        private readonly StartVertexEqualityContractVerifier m_EqualityVerifier = new StartVertexEqualityContractVerifier();

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
            var vertex = new StartVertex(index);

            Assert.AreEqual(index, vertex.Index);
        }
    }
}
