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
    public sealed class ScheduleIdTest : EqualityContractVerifierTest
    {
        private sealed class ScheduleIdEqualityContractVerifier : EqualityContractVerifier<ScheduleId>
        {
            private readonly ScheduleId m_First = new ScheduleId();

            private readonly ScheduleId m_Second = new ScheduleId();

            protected override ScheduleId Copy(ScheduleId original)
            {
                return original.Clone();
            }

            protected override ScheduleId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ScheduleId SecondInstance
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

        private sealed class ScheduleIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ScheduleId> m_DistinctInstances
                = new List<ScheduleId> 
                     {
                        new ScheduleId(),
                        new ScheduleId(),
                        new ScheduleId(),
                        new ScheduleId(),
                        new ScheduleId(),
                        new ScheduleId(),
                        new ScheduleId(),
                        new ScheduleId(),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ScheduleIdHashcodeContractVerfier m_HashcodeVerifier = new ScheduleIdHashcodeContractVerfier();

        private readonly ScheduleIdEqualityContractVerifier m_EqualityVerifier = new ScheduleIdEqualityContractVerifier();

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
            ScheduleId first = null;
            ScheduleId second = new ScheduleId();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            ScheduleId first = new ScheduleId();
            ScheduleId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            ScheduleId first = null;
            ScheduleId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleId();
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ScheduleId first = null;
            ScheduleId second = new ScheduleId();

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            ScheduleId first = new ScheduleId();
            ScheduleId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            ScheduleId first = null;
            ScheduleId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleId();
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            ScheduleId first = new ScheduleId();
            ScheduleId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            ScheduleId first = new ScheduleId();
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ScheduleId();
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? new ScheduleId(firstGuid) : new ScheduleId(secondGuid);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            ScheduleId first = new ScheduleId();
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
