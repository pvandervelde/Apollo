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
    public sealed class ValueAtTimeTest : EqualityContractVerifierTest
    {
        private sealed class ValueAtTimeEqualityContractVerifier : EqualityContractVerifier<ValueAtTime<int>>
        {
            private readonly ValueAtTime<int> m_First = new ValueAtTime<int>(new TimeMarker(10), 10);

            private readonly ValueAtTime<int> m_Second = new ValueAtTime<int>(new TimeMarker(20), 10);

            protected override ValueAtTime<int> Copy(ValueAtTime<int> original)
            {
                return new ValueAtTime<int>(original.Time, original.Value);
            }

            protected override ValueAtTime<int> FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ValueAtTime<int> SecondInstance
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

        private sealed class ValueAtTimeHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ValueAtTime<int>> m_DistinctInstances
                = new List<ValueAtTime<int>> 
                     {
                        new ValueAtTime<int>(new TimeMarker(10), 10),
                        new ValueAtTime<int>(new TimeMarker(20), 10),
                        new ValueAtTime<int>(new TimeMarker(30), 10),
                        new ValueAtTime<int>(new TimeMarker(40), 10),
                        new ValueAtTime<int>(new TimeMarker(50), 10),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ValueAtTimeHashcodeContractVerfier m_HashcodeVerifier = new ValueAtTimeHashcodeContractVerfier();

        private readonly ValueAtTimeEqualityContractVerifier m_EqualityVerifier = new ValueAtTimeEqualityContractVerifier();

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
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ValueAtTime<int>(new TimeMarker(10), 10);
            var second = new ValueAtTime<int>(new TimeMarker(10), 10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new ValueAtTime<int>(new TimeMarker(20), 10);
            var second = new ValueAtTime<int>(new TimeMarker(10), 10);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ValueAtTime<int>(new TimeMarker(10), 10);
            var second = new ValueAtTime<int>(new TimeMarker(20), 10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ValueAtTime<int>(new TimeMarker(10), 10);
            var second = new ValueAtTime<int>(new TimeMarker(10), 10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new ValueAtTime<int>(new TimeMarker(20), 10);
            var second = new ValueAtTime<int>(new TimeMarker(10), 10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ValueAtTime<int>(new TimeMarker(10), 10);
            var second = new ValueAtTime<int>(new TimeMarker(20), 10);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new ValueAtTime<int>(new TimeMarker(10), 10);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ValueAtTime<int>(new TimeMarker(10), 10);
            object second = new ValueAtTime<int>(new TimeMarker(10), 10);

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new ValueAtTime<int>(new TimeMarker(20), 10);
            object second = new ValueAtTime<int>(new TimeMarker(10), 10);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new ValueAtTime<int>(new TimeMarker(10), 10);
            object second = new ValueAtTime<int>(new TimeMarker(20), 10);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new ValueAtTime<int>(new TimeMarker(10), 10);
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
