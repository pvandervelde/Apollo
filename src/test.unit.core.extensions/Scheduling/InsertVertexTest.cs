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
    public sealed class InsertVertexTest : EqualityContractVerifierTest
    {
        private sealed class InsertVertexEqualityContractVerifier : EqualityContractVerifier<InsertVertex>
        {
            private readonly InsertVertex m_First = new InsertVertex(0);

            private readonly InsertVertex m_Second = new InsertVertex(1);

            protected override InsertVertex Copy(InsertVertex original)
            {
                return new InsertVertex(original.Index);
            }

            protected override InsertVertex FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override InsertVertex SecondInstance
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

        private sealed class InsertVertexHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<InsertVertex> m_DistinctInstances
                = new List<InsertVertex> 
                     {
                        new InsertVertex(0),
                        new InsertVertex(1),
                        new InsertVertex(2),
                        new InsertVertex(3),
                        new InsertVertex(4),
                        new InsertVertex(5),
                        new InsertVertex(6),
                        new InsertVertex(7),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly InsertVertexHashcodeContractVerfier m_HashcodeVerifier = new InsertVertexHashcodeContractVerfier();

        private readonly InsertVertexEqualityContractVerifier m_EqualityVerifier = new InsertVertexEqualityContractVerifier();

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
            var vertex = new InsertVertex(index);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreEqual(-1, vertex.RemainingInserts);
        }

        [Test]
        public void InsertCount()
        {
            var index = 10;
            var insertCount = 5;
            var vertex = new InsertVertex(index, insertCount);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreEqual(insertCount, vertex.RemainingInserts);
        }
    }
}
