//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetIdTest
    {
        private static DatasetId Create(int id)
        {
            return (DatasetId)Mirror.ForType<DatasetId>().Constructor.Invoke(id);
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<DatasetId>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<DatasetId> 
                    {
                        Create(0),
                        Create(1),
                        Create(2),
                        Create(3),
                        Create(4),
                        Create(5),
                        Create(6),
                        Create(7),
                        Create(8),
                        Create(9),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<DatasetId>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    Create(0),
                    Create(1),
                    Create(2),
                    Create(3),
                    Create(4),
                    Create(5),
                    Create(6),
                    Create(7),
                    Create(8),
                    Create(9),
                },
        };

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            DatasetId first = null;
            var second = new DatasetId();

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new DatasetId();
            DatasetId second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new DatasetId();
            var second = first.Clone();

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new DatasetId();
            var second = new DatasetId();

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            DatasetId first = null;
            var second = new DatasetId();

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new DatasetId();
            DatasetId second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new DatasetId();
            var second = first.Clone();

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new DatasetId();
            var second = new DatasetId();

            Assert.IsTrue(first != second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            DatasetId first = null;
            var second = new DatasetId();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new DatasetId();
            DatasetId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            DatasetId first = null;
            DatasetId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new DatasetId();
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = Create(2);
            var second = Create(1);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = Create(1);
            var second = Create(2);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            DatasetId first = null;
            var second = new DatasetId();

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new DatasetId();
            DatasetId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            DatasetId first = null;
            DatasetId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new DatasetId();
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = Create(2);
            var second = Create(1);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = Create(1);
            var second = Create(2);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new DatasetId();
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new DatasetId();
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new DatasetId();
            object second = first.Clone();

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new DatasetId();
            object second = new DatasetId();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new DatasetId();
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new DatasetId();
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new DatasetId();
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = Create(2);
            var second = Create(1);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = Create(1);
            var second = Create(2);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new DatasetId();
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
