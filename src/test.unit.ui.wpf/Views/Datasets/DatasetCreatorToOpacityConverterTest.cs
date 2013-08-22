//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetCreatorToOpacityConverterTest
    {
        [Test]
        public void ConvertWithIncorrectTargetType()
        {
            var converter = new DatasetCreatorToOpacityConverter();
            Assert.Throws<InvalidOperationException>(() => converter.Convert(DatasetCreator.System, typeof(string), null, null));
        }

        [Test]
        public void ConvertWithIncorrectValueType()
        {
            var converter = new DatasetCreatorToOpacityConverter();
            Assert.Throws<InvalidOperationException>(() => converter.Convert(new object(), typeof(double), null, null));
        }

        [Test]
        public void ConvertWithIncorrectValue()
        {
            var converter = new DatasetCreatorToOpacityConverter();
            Assert.Throws<InvalidOperationException>(() => converter.Convert(DatasetCreator.None, typeof(double), null, null));
        }

        [Test]
        public void Convert()
        {
            var converter = new DatasetCreatorToOpacityConverter();

            var systemValue = (double)converter.Convert(DatasetCreator.System, typeof(double), null, null);
            Assert.AreEqual(0.65, systemValue);

            var userValue = (double)converter.Convert(DatasetCreator.User, typeof(double), null, null);
            Assert.AreEqual(1.0, userValue);
        }
    }
}
