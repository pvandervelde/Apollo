//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using MbUnit.Framework;

namespace Apollo.Utils
{
    [TestFixture]
    [Description("Tests the TypeEqualityComparer class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TypeEqualityComparerTest
    {
        [Test]
        [Description("Checks that a null object is not equal to a non-null object.")]
        public void EqualsWithFirstObjectNull()
        {
            var comparer = new TypeEqualityComparer();
            Assert.IsFalse(comparer.Equals(null, typeof(object)));
        }

        [Test]
        [Description("Checks that a null object is not equal to a non-null object.")]
        public void EqualsWithSecondObjectNull()
        {
            var comparer = new TypeEqualityComparer();
            Assert.IsFalse(comparer.Equals(typeof(object), null));
        }

        [Test]
        [Description("Checks that two null objects are not considered equal.")]
        public void EqualsWithBothObjectsNull()
        {
            var comparer = new TypeEqualityComparer();
            Assert.IsFalse(comparer.Equals(null, null));
        }

        [Test]
        [Description("Checks that a two unequal objects are not considered equal.")]
        public void EqualsWithUnequalObjects()
        {
            var comparer = new TypeEqualityComparer();
            Assert.IsFalse(comparer.Equals(typeof(string), typeof(object)));
        }

        [Test]
        [Description("Checks that two equal objects are considered equal.")]
        public void EqualsWithEqualObjects()
        {
            var comparer = new TypeEqualityComparer();
            Assert.IsTrue(comparer.Equals(typeof(string), typeof(string)));
        }
    }
}
