//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CommandToToolTipTextConverterTest
    {
        [Test]
        public void ConvertWithCommand()
        {
            var text = "a";
            var name = "b";

            var gestures = new InputGestureCollection(
                new List<InputGesture>
                {
                    new KeyGesture(Key.A, ModifierKeys.Alt),
                });
            var command = new RoutedUICommand(text, name, typeof(Window), gestures);

            var converter = new CommandToToolTipTextConverter();
            var convertedValue = converter.Convert(command, null, null, null);
            
            var expected = string.Format(
                CultureInfo.CurrentCulture,
                "{0} ({1})",
                text,
                "Alt+A");
            Assert.AreEqual(expected, convertedValue);
        }

        [Test]
        public void ConvertWithCommandWithoutKeyGestures()
        {
            var text = "a";
            var name = "b";
            var command = new RoutedUICommand(text, name, typeof(Window));

            var converter = new CommandToToolTipTextConverter();
            var convertedValue = converter.Convert(command, null, null, null);

            Assert.AreEqual(text, convertedValue);
        }

        [Test]
        public void ConvertWithNullReference()
        {
            var converter = new CommandToToolTipTextConverter();
            var convertedValue = converter.Convert(null, null, null, null);
            Assert.AreEqual(string.Empty, convertedValue);
        }

        [Test]
        public void ConvertWithNonCommandObject()
        {
            var converter = new CommandToToolTipTextConverter();
            var convertedValue = converter.Convert(new object(), null, null, null);
            Assert.AreEqual(string.Empty, convertedValue);
        }
    }
}
