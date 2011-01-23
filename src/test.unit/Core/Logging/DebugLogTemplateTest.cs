//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MbUnit.Framework;

namespace Apollo.Core.Logging
{
    [TestFixture]
    [Description("Tests the DebugLogTemplate class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DebugLogTemplateTest
    {
        private static DateTimeOffset GetDefaultDateTime()
        {
            return new DateTimeOffset(2000, 1, 1, 1, 1, 1, new TimeSpan(0));
        }

        [Test]
        [Description("Checks that a null message cannot be translated.")]
        public void TranslateWithNullMessage()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.Throws<ArgumentNullException>(() => template.Translate(null));
        }

        [Test]
        [Description("Checks that a message is translated correctly.")]
        public void Translate()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            var msg = new LogMessage("bla", LevelToLog.Info, "blabla");
            var text = template.Translate(msg);

            var expectedText = string.Format(CultureInfo.CurrentCulture, DebugLogTemplate.DebugLogFormat, GetDefaultDateTime(), msg.Origin, msg.Text());
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        [Description("Checks that the correct log type is returned.")]
        public void GetLogType()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.AreEqual(LogType.Debug, template.LogType);
        }

        [Test]
        [Description("Checks that a null object is not equal to a given template.")]
        public void EqualsWithNullObject()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals((object)null));
        }

        [Test]
        [Description("Checks that an object of a different type is not equal to a given template.")]
        public void EqualsWithNonEqualType()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals(new object()));
        }

        [Test]
        [Description("Checks that a non-equal object is not considered equal to a given template.")]
        public void EqualsWithNonEqualObject()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals((object)new CommandLogTemplate(GetDefaultDateTime)));
        }

        [Test]
        [Description("Checks that an equal object is considered equal to a given template.")]
        public void EqualsWithEqualObject()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.IsTrue(template.Equals((object)new DebugLogTemplate(GetDefaultDateTime)));
        }

        [Test]
        [Description("Checks that an equal object is considered equal to itself.")]
        public void EqualsWithSameObject()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.IsTrue(template.Equals((object)template));
        }

        [Test]
        [Description("Checks that a null ILogTemplate is not equal to a given template.")]
        public void EqualsWithNullILogTemplate()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals((ILogTemplate)null));
        }

        [Test]
        [Description("Checks that a non-equal ILogTemplate is not considered equal to a given template.")]
        public void EqualsWithNonEqualILogTemplate()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals(new CommandLogTemplate(GetDefaultDateTime)));
        }

        [Test]
        [Description("Checks that an equal IlogTemplate is considered equal to a given template.")]
        public void EqualsWithEqualILogTemplate()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.IsTrue(template.Equals((ILogTemplate)new DebugLogTemplate(GetDefaultDateTime)));
        }

        [Test]
        [Description("Checks that an equal IlogTemplate is considered equal to itself.")]
        public void EqualsWithSameILogTemplate()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.IsTrue(template.Equals((ILogTemplate)template));
        }

        [Test]
        [Description("Checks that a null DebugLogTemplate is not equal to a given template.")]
        public void EqualsWithNullDebugLogTemplate()
        {
            var template = new DebugLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals(null));
        }
    }
}