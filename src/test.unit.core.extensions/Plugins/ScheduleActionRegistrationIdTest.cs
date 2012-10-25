//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Extensions.Plugins
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleActionRegistrationIdTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ScheduleActionRegistrationId>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ScheduleActionRegistrationId> 
                        {
                            new ScheduleActionRegistrationId(typeof(string), 0, "a"),
                            new ScheduleActionRegistrationId(typeof(int), 0, "a"),
                            new ScheduleActionRegistrationId(typeof(string), 1, "a"),
                            new ScheduleActionRegistrationId(typeof(string), 0, "b"),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ScheduleActionRegistrationId>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new ScheduleActionRegistrationId(typeof(string), 0, "a"),
                        new ScheduleActionRegistrationId(typeof(int), 0, "a"),
                        new ScheduleActionRegistrationId(typeof(string), 1, "a"),
                        new ScheduleActionRegistrationId(typeof(string), 0, "b"),
                    },
        };

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            ScheduleActionRegistrationId first = null;
            ScheduleActionRegistrationId second = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            ScheduleActionRegistrationId first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            ScheduleActionRegistrationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            ScheduleActionRegistrationId first = null;
            ScheduleActionRegistrationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ScheduleActionRegistrationId first = null;
            ScheduleActionRegistrationId second = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            ScheduleActionRegistrationId first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            ScheduleActionRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            ScheduleActionRegistrationId first = null;
            ScheduleActionRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            ScheduleActionRegistrationId first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            ScheduleActionRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            ScheduleActionRegistrationId first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            ScheduleActionRegistrationId first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
