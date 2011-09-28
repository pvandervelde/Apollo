//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Utilities
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TypeEqualityComparerTest
    {
        [Test]
        public void EqualsWithFirstObjectNull()
        {
            var comparer = new TypeEqualityComparer();
            Assert.IsFalse(comparer.Equals(null, typeof(object)));
        }

        [Test]
        public void EqualsWithSecondObjectNull()
        {
            var comparer = new TypeEqualityComparer();
            Assert.IsFalse(comparer.Equals(typeof(object), null));
        }

        [Test]
        public void EqualsWithBothObjectsNull()
        {
            var comparer = new TypeEqualityComparer();
            Assert.IsFalse(comparer.Equals(null, null));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var comparer = new TypeEqualityComparer();
            Assert.IsFalse(comparer.Equals(typeof(string), typeof(object)));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var comparer = new TypeEqualityComparer();
            Assert.IsTrue(comparer.Equals(typeof(string), typeof(string)));
        }
    }
}
