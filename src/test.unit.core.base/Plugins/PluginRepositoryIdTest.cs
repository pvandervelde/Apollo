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

namespace Apollo.Core.Base.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class PluginRepositoryIdTest : EqualityContractVerifierTest
    {
        private sealed class PluginRepositoryIdEqualityContractVerifier : EqualityContractVerifier<PluginRepositoryId>
        {
            private readonly PluginRepositoryId m_First = new PluginRepositoryId("a");

            private readonly PluginRepositoryId m_Second = new PluginRepositoryId("b");

            protected override PluginRepositoryId Copy(PluginRepositoryId original)
            {
                return original.Clone();
            }

            protected override PluginRepositoryId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override PluginRepositoryId SecondInstance
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

        private sealed class PluginRepositoryIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<PluginRepositoryId> m_DistinctInstances
                = new List<PluginRepositoryId> 
                     {
                        new PluginRepositoryId("a"),
                        new PluginRepositoryId("b"),
                        new PluginRepositoryId("c"),
                        new PluginRepositoryId("d"),
                        new PluginRepositoryId("e"),
                        new PluginRepositoryId("f"),
                        new PluginRepositoryId("g"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly PluginRepositoryIdHashcodeContractVerfier m_HashcodeVerifier = new PluginRepositoryIdHashcodeContractVerfier();

        private readonly PluginRepositoryIdEqualityContractVerifier m_EqualityVerifier = new PluginRepositoryIdEqualityContractVerifier();

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
            PluginRepositoryId first = null;
            PluginRepositoryId second = new PluginRepositoryId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            PluginRepositoryId first = new PluginRepositoryId("a");
            PluginRepositoryId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            PluginRepositoryId first = null;
            PluginRepositoryId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new PluginRepositoryId("a");
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new PluginRepositoryId("b");
            var second = new PluginRepositoryId("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new PluginRepositoryId("a");
            var second = new PluginRepositoryId("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            PluginRepositoryId first = null;
            PluginRepositoryId second = new PluginRepositoryId("a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            PluginRepositoryId first = new PluginRepositoryId("a");
            PluginRepositoryId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            PluginRepositoryId first = null;
            PluginRepositoryId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new PluginRepositoryId("a");
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new PluginRepositoryId("b");
            var second = new PluginRepositoryId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new PluginRepositoryId("a");
            var second = new PluginRepositoryId("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            PluginRepositoryId first = new PluginRepositoryId("a");
            PluginRepositoryId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            PluginRepositoryId first = new PluginRepositoryId("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new PluginRepositoryId("a");
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new PluginRepositoryId("b");
            var second = new PluginRepositoryId("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new PluginRepositoryId("a");
            var second = new PluginRepositoryId("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            PluginRepositoryId first = new PluginRepositoryId("a");
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
