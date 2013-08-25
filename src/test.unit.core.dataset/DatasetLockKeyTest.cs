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

namespace Apollo.Core.Dataset
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetLockKeyTest : EqualityContractVerifierTest
    {
        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<DatasetLockKey>
        {
            private readonly DatasetLockKey m_First = new DatasetLockKey(1);

            private readonly DatasetLockKey m_Second = new DatasetLockKey(2);

            protected override DatasetLockKey Copy(DatasetLockKey original)
            {
                return original.Clone();
            }

            protected override DatasetLockKey FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override DatasetLockKey SecondInstance
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
            private readonly IEnumerable<DatasetLockKey> m_DistinctInstances
                = new List<DatasetLockKey> 
                     {
                        new DatasetLockKey(1),
                        new DatasetLockKey(2),
                        new DatasetLockKey(3),
                        new DatasetLockKey(4),
                        new DatasetLockKey(5),
                        new DatasetLockKey(6),
                        new DatasetLockKey(7),
                        new DatasetLockKey(8),
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
            DatasetLockKey first = null;
            var second = new DatasetLockKey(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new DatasetLockKey(10);
            DatasetLockKey second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            DatasetLockKey first = null;
            DatasetLockKey second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new DatasetLockKey(10);
            var second = new DatasetLockKey(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new DatasetLockKey(11);
            var second = new DatasetLockKey(10);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new DatasetLockKey(9);
            var second = new DatasetLockKey(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            DatasetLockKey first = null;
            var second = new DatasetLockKey(10);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new DatasetLockKey(10);
            DatasetLockKey second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            DatasetLockKey first = null;
            DatasetLockKey second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new DatasetLockKey(10);
            var second = new DatasetLockKey(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new DatasetLockKey(11);
            var second = new DatasetLockKey(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new DatasetLockKey(9);
            var second = new DatasetLockKey(10);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new DatasetLockKey(10);
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new DatasetLockKey(10);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new DatasetLockKey(10);
            object second = new DatasetLockKey(10);

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new DatasetLockKey(11);
            object second = new DatasetLockKey(10);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new DatasetLockKey(10);
            object second = new DatasetLockKey(11);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new DatasetLockKey(10);
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
