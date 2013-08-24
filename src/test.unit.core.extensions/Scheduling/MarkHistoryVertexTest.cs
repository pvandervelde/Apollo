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
    public sealed class MarkHistoryVertexTest : EqualityContractVerifierTest
    {
        private sealed class MarkHistoryVertexEqualityContractVerifier : EqualityContractVerifier<MarkHistoryVertex>
        {
            private readonly MarkHistoryVertex m_First = new MarkHistoryVertex(0);

            private readonly MarkHistoryVertex m_Second = new MarkHistoryVertex(1);

            protected override MarkHistoryVertex Copy(MarkHistoryVertex original)
            {
                return new MarkHistoryVertex(original.Index);
            }

            protected override MarkHistoryVertex FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override MarkHistoryVertex SecondInstance
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

        private sealed class MarkHistoryVertexHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<MarkHistoryVertex> m_DistinctInstances
                = new List<MarkHistoryVertex> 
                     {
                        new MarkHistoryVertex(0),
                        new MarkHistoryVertex(1),
                        new MarkHistoryVertex(2),
                        new MarkHistoryVertex(3),
                        new MarkHistoryVertex(4),
                        new MarkHistoryVertex(5),
                        new MarkHistoryVertex(6),
                        new MarkHistoryVertex(7),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly MarkHistoryVertexHashcodeContractVerfier m_HashcodeVerifier = new MarkHistoryVertexHashcodeContractVerfier();

        private readonly MarkHistoryVertexEqualityContractVerifier m_EqualityVerifier = new MarkHistoryVertexEqualityContractVerifier();

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
            var vertex = new MarkHistoryVertex(index);

            Assert.AreEqual(index, vertex.Index);
        }
    }
}
