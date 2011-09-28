﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Base
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
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
        public void LargerThanOperatorWithFirstObjectNull()
        {
            DatasetId first = null;
            DatasetId second = Create(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            DatasetId first = Create(10);
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
            DatasetId first = Create(10);
            DatasetId second = Create(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            DatasetId first = Create(11);
            DatasetId second = Create(10);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            DatasetId first = Create(9);
            DatasetId second = Create(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            DatasetId first = null;
            DatasetId second = Create(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            DatasetId first = Create(10);
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
            DatasetId first = Create(10);
            DatasetId second = Create(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            DatasetId first = Create(11);
            DatasetId second = Create(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            DatasetId first = Create(9);
            DatasetId second = Create(10);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            DatasetId first = Create(10);
            DatasetId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            DatasetId first = Create(10);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            DatasetId first = Create(10);
            object second = Create(10);

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            DatasetId first = Create(11);
            object second = Create(10);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            DatasetId first = Create(10);
            object second = Create(11);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            DatasetId first = Create(10);
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}