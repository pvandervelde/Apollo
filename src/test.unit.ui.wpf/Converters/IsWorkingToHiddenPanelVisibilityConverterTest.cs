using System.Diagnostics.CodeAnalysis;
using System.Windows;
using MbUnit.Framework;

namespace Apollo.UI.Wpf.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class IsWorkingToHiddenPanelVisibilityConverterTest
    {
        [Test]
        public void ConvertWithTrueBoolean()
        {
            var converter = new IsWorkingToHiddenPanelVisibilityConverter();
            var convertedValue = converter.Convert(true, null, null, null);
            Assert.AreEqual(Visibility.Visible, convertedValue);
        }

        [Test]
        public void ConvertWithFalseBoolean()
        {
            var converter = new IsWorkingToHiddenPanelVisibilityConverter();
            var convertedValue = converter.Convert(false, null, null, null);
            Assert.AreEqual(Visibility.Collapsed, convertedValue);
        }

        [Test]
        public void ConvertWithNullReference()
        {
            var converter = new IsWorkingToHiddenPanelVisibilityConverter();
            var convertedValue = converter.Convert(null, null, null, null);
            Assert.AreEqual(Visibility.Hidden, convertedValue);
        }

        [Test]
        public void ConvertWithNonBooleanObject()
        {
            var converter = new IsWorkingToHiddenPanelVisibilityConverter();
            var convertedValue = converter.Convert(new object(), null, null, null);
            Assert.AreEqual(Visibility.Hidden, convertedValue);
        }
    }
}
