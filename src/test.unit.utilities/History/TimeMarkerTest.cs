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
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class TimeMarkerTest : EqualityContractVerifierTest
    {
        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<TimeMarker>
        {
            private readonly TimeMarker m_First = new TimeMarker(1);

            private readonly TimeMarker m_Second = new TimeMarker(2);

            protected override TimeMarker Copy(TimeMarker original)
            {
                return new TimeMarker(original);
            }

            protected override TimeMarker FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override TimeMarker SecondInstance
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
            private readonly IEnumerable<TimeMarker> m_DistinctInstances
                = new List<TimeMarker> 
                     {
                        new TimeMarker(1),
                        new TimeMarker(2),
                        new TimeMarker(3),
                        new TimeMarker(4),
                        new TimeMarker(5),
                        new TimeMarker(6),
                        new TimeMarker(7),
                        new TimeMarker(8),
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
            TimeMarker first = null;
            var second = new TimeMarker(1);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new TimeMarker(1);
            TimeMarker second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            TimeMarker first = null;
            TimeMarker second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new TimeMarker(1);
            var second = new TimeMarker(1);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new TimeMarker(2);
            var second = new TimeMarker(1);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new TimeMarker(1);
            var second = new TimeMarker(2);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            TimeMarker first = null;
            var second = new TimeMarker(1);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new TimeMarker(1);
            TimeMarker second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            TimeMarker first = null;
            TimeMarker second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new TimeMarker(1);
            var second = new TimeMarker(1);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new TimeMarker(2);
            var second = new TimeMarker(1);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new TimeMarker(1);
            var second = new TimeMarker(2);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new TimeMarker(1);
            var second = new TimeMarker(first);

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new TimeMarker(1);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new TimeMarker(1);
            object second = new TimeMarker(1);

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new TimeMarker(2);
            object second = new TimeMarker(1);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new TimeMarker(1);
            object second = new TimeMarker(2);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new TimeMarker(1);
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
