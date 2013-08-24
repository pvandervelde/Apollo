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

namespace Apollo.Core.Extensions.Scheduling
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleElementIdTest : EqualityContractVerifierTest
    {
        private sealed class ScheduleElementIdEqualityContractVerifier : EqualityContractVerifier<ScheduleElementId>
        {
            private readonly ScheduleElementId m_First = new ScheduleElementId();

            private readonly ScheduleElementId m_Second = new ScheduleElementId();

            protected override ScheduleElementId Copy(ScheduleElementId original)
            {
                return original.Clone();
            }

            protected override ScheduleElementId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ScheduleElementId SecondInstance
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

        private sealed class ScheduleElementIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ScheduleElementId> m_DistinctInstances
                = new List<ScheduleElementId> 
                     {
                        new ScheduleElementId(),
                        new ScheduleElementId(),
                        new ScheduleElementId(),
                        new ScheduleElementId(),
                        new ScheduleElementId(),
                        new ScheduleElementId(),
                        new ScheduleElementId(),
                        new ScheduleElementId(),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ScheduleElementIdHashcodeContractVerfier m_HashcodeVerifier = new ScheduleElementIdHashcodeContractVerfier();

        private readonly ScheduleElementIdEqualityContractVerifier m_EqualityVerifier = new ScheduleElementIdEqualityContractVerifier();

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
            ScheduleElementId first = null;
            ScheduleElementId second = new ScheduleElementId();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            ScheduleElementId first = new ScheduleElementId();
            ScheduleElementId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            ScheduleElementId first = null;
            ScheduleElementId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleElementId();
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ScheduleElementId first = null;
            ScheduleElementId second = new ScheduleElementId();

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            ScheduleElementId first = new ScheduleElementId();
            ScheduleElementId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            ScheduleElementId first = null;
            ScheduleElementId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleElementId();
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            ScheduleElementId first = new ScheduleElementId();
            ScheduleElementId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            ScheduleElementId first = new ScheduleElementId();
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ScheduleElementId();
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleElementId(firstGuid) : new ScheduleElementId(secondGuid);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            ScheduleElementId first = new ScheduleElementId();
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
