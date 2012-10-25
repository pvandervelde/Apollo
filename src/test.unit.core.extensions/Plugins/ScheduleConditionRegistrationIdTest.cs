//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public sealed class ScheduleConditionRegistrationIdTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ScheduleConditionRegistrationId>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ScheduleConditionRegistrationId> 
                        {
                            new ScheduleConditionRegistrationId(typeof(string), 0, "a"),
                            new ScheduleConditionRegistrationId(typeof(int), 0, "a"),
                            new ScheduleConditionRegistrationId(typeof(string), 1, "a"),
                            new ScheduleConditionRegistrationId(typeof(string), 0, "b"),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ScheduleConditionRegistrationId>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new ScheduleConditionRegistrationId(typeof(string), 0, "a"),
                        new ScheduleConditionRegistrationId(typeof(int), 0, "a"),
                        new ScheduleConditionRegistrationId(typeof(string), 1, "a"),
                        new ScheduleConditionRegistrationId(typeof(string), 0, "b"),
                    },
        };

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            ScheduleConditionRegistrationId first = null;
            ScheduleConditionRegistrationId second = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            ScheduleConditionRegistrationId first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            ScheduleConditionRegistrationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            ScheduleConditionRegistrationId first = null;
            ScheduleConditionRegistrationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ScheduleConditionRegistrationId first = null;
            ScheduleConditionRegistrationId second = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            ScheduleConditionRegistrationId first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            ScheduleConditionRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            ScheduleConditionRegistrationId first = null;
            ScheduleConditionRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            ScheduleConditionRegistrationId first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            ScheduleConditionRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            ScheduleConditionRegistrationId first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            ScheduleConditionRegistrationId first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
