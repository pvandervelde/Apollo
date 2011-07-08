﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationNameTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<NotificationName>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = DataGenerators.Random.Strings(100, RandomStringStock.EnUSMaleNames).Select(o => new NotificationName(o)),
        };

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            NotificationName first = null;
            var second = new NotificationName("name");

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new NotificationName("name");
            NotificationName second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new NotificationName("name");
            var second = first.Clone();

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new NotificationName("name1");
            var second = new NotificationName("name2");

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            NotificationName first = null;
            var second = new NotificationName("name");

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new NotificationName("name");
            NotificationName second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new NotificationName("name");
            var second = first.Clone();

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new NotificationName("name1");
            var second = new NotificationName("name2");

            Assert.IsTrue(first != second);
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

            Assert.IsFalse(first < second);
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
