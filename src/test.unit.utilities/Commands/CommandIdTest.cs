//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.Utilities.Commands
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CommandIdTest : EqualityContractVerifierTest
    {
        private sealed class CommandIdEqualityContractVerifier : EqualityContractVerifier<CommandId>
        {
            private readonly CommandId m_First = new CommandId("a");

            private readonly CommandId m_Second = new CommandId("b");

            protected override CommandId Copy(CommandId original)
            {
                return original.Clone();
            }

            protected override CommandId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override CommandId SecondInstance
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

        private sealed class CommandIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<CommandId> m_DistinctInstances
                = new List<CommandId> 
                     {
                        new CommandId(1.ToString(CultureInfo.InvariantCulture)),
                        new CommandId(2.ToString(CultureInfo.InvariantCulture)),
                        new CommandId(3.ToString(CultureInfo.InvariantCulture)),
                        new CommandId(4.ToString(CultureInfo.InvariantCulture)),
                        new CommandId(5.ToString(CultureInfo.InvariantCulture)),
                        new CommandId(6.ToString(CultureInfo.InvariantCulture)),
                        new CommandId(7.ToString(CultureInfo.InvariantCulture)),
                        new CommandId(8.ToString(CultureInfo.InvariantCulture)),
                        new CommandId(9.ToString(CultureInfo.InvariantCulture)),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly CommandIdHashcodeContractVerfier m_HashcodeVerifier = new CommandIdHashcodeContractVerfier();

        private readonly CommandIdEqualityContractVerifier m_EqualityVerifier = new CommandIdEqualityContractVerifier();

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
            CommandId first = null;
            var second = new CommandId("name");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new CommandId("name");
            CommandId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            CommandId first = null;
            CommandId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new CommandId("name");
            var second = new CommandId("name");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new CommandId("b");
            var second = new CommandId("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new CommandId("a");
            var second = new CommandId("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            CommandId first = null;
            var second = new CommandId("name");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new CommandId("name");
            CommandId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            CommandId first = null;
            CommandId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new CommandId("name");
            var second = new CommandId("name");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new CommandId("b");
            var second = new CommandId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new CommandId("a");
            var second = new CommandId("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new CommandId("name");
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            IComparable first = new CommandId("name");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            IComparable first = new CommandId("name");
            object second = new CommandId("name");

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            IComparable first = new CommandId("b");
            object second = new CommandId("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            IComparable first = new CommandId("a");
            object second = new CommandId("b");

            Assert.IsTrue(string.Compare("a", "b", StringComparison.Ordinal) < 0);
            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            IComparable first = new CommandId("name");
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
