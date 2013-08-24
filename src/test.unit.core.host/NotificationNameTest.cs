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

namespace Apollo.Core.Host
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationNameTest : EqualityContractVerifierTest
    {
        private sealed class NotificationNameEqualityContractVerifier : EqualityContractVerifier<NotificationName>
        {
            private readonly NotificationName m_First = new NotificationName("a");

            private readonly NotificationName m_Second = new NotificationName("b");

            protected override NotificationName Copy(NotificationName original)
            {
                return original.Clone();
            }

            protected override NotificationName FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override NotificationName SecondInstance
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

        private sealed class NotificationNameHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<NotificationName> m_DistinctInstances
                = new List<NotificationName> 
                     {
                        new NotificationName("a"),
                        new NotificationName("b"),
                        new NotificationName("c"),
                        new NotificationName("d"),
                        new NotificationName("e"),
                        new NotificationName("f"),
                        new NotificationName("g"),
                        new NotificationName("h"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly NotificationNameHashcodeContractVerfier m_HashcodeVerifier = new NotificationNameHashcodeContractVerfier();

        private readonly NotificationNameEqualityContractVerifier m_EqualityVerifier = new NotificationNameEqualityContractVerifier();

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
            NotificationName first = null;
            var second = new NotificationName("name");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new NotificationName("name");
            NotificationName second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            NotificationName first = null;
            NotificationName second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new NotificationName("name");
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new NotificationName("b");
            var second = new NotificationName("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new NotificationName("a");
            var second = new NotificationName("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            NotificationName first = null;
            var second = new NotificationName("name");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new NotificationName("name");
            NotificationName second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            NotificationName first = null;
            NotificationName second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new NotificationName("name");
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new NotificationName("b");
            var second = new NotificationName("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new NotificationName("a");
            var second = new NotificationName("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new NotificationName("name");
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new NotificationName("name");
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new NotificationName("name");
            object second = first.Clone();

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new NotificationName("name1");
            object second = new NotificationName("name2");

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new NotificationName("name");
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new NotificationName("name");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new NotificationName("name");
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new NotificationName("b");
            var second = new NotificationName("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new NotificationName("a");
            var second = new NotificationName("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new NotificationName("name");
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
