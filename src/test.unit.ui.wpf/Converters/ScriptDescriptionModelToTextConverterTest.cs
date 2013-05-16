//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Scripting;
using Apollo.UI.Wpf.Views.Scripting;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptDescriptionModelToTextConverterTest
    {
        [Test]
        public void ConvertWithScriptDescriptionModel()
        {
            var context = new Mock<IContextAware>();
            var value = new ScriptDescriptionModel(context.Object, ScriptLanguage.IronPython);

            var converter = new ScriptDescriptionModelToTextConverter();
            var convertedValue = converter.Convert(value, null, null, null);
            Assert.AreEqual(value.Description, convertedValue);
        }

        [Test]
        public void ConvertWithNullReference()
        {
            var converter = new ScriptDescriptionModelToTextConverter();
            var convertedValue = converter.Convert(null, null, null, null);
            Assert.AreEqual(string.Empty, convertedValue);
        }

        [Test]
        public void ConvertWithNonScriptDescriptionModel()
        {
            var converter = new ScriptDescriptionModelToTextConverter();
            var convertedValue = converter.Convert(new object(), null, null, null);
            Assert.AreEqual(string.Empty, convertedValue);
        }
    }
}
