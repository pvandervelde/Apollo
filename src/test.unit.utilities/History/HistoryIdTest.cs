//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.Utilities.History
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class HistoryIdTest : EqualityContractVerifierTest
    {
        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<HistoryId>
        {
            private readonly HistoryId m_First = new HistoryId(1);

            private readonly HistoryId m_Second = new HistoryId(2);

            protected override HistoryId Copy(HistoryId original)
            {
                return original.Clone();
            }

            protected override HistoryId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override HistoryId SecondInstance
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

        private sealed class EndpointIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<HistoryId> m_DistinctInstances
                = new List<HistoryId> 
                     {
                        new HistoryId(1),
                        new HistoryId(2),
                        new HistoryId(3),
                        new HistoryId(4),
                        new HistoryId(5),
                        new HistoryId(6),
                        new HistoryId(7),
                        new HistoryId(8),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly EndpointIdHashcodeContractVerfier m_HashcodeVerifier = new EndpointIdHashcodeContractVerfier();

        private readonly EndpointIdEqualityContractVerifier m_EqualityVerifier = new EndpointIdEqualityContractVerifier();

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
        public void LargerThanOperatorWithFirstObjectNull()
        {
            HistoryId first = null;
            var second = new HistoryId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new HistoryId(10);
            HistoryId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            HistoryId first = null;
            HistoryId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new HistoryId(10);
            var second = new HistoryId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new HistoryId(11);
            var second = new HistoryId(10);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new HistoryId(9);
            var second = new HistoryId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            HistoryId first = null;
            var second = new HistoryId(10);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new HistoryId(10);
            HistoryId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            HistoryId first = null;
            HistoryId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new HistoryId(10);
            var second = new HistoryId(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new HistoryId(11);
            var second = new HistoryId(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new HistoryId(9);
            var second = new HistoryId(10);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new HistoryId(10);
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new HistoryId(10);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new HistoryId(10);
            object second = new HistoryId(10);

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new HistoryId(11);
            object second = new HistoryId(10);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new HistoryId(10);
            object second = new HistoryId(11);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new HistoryId(10);
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
