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
    [Description("Tests the CommandLogTemplate class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CommandLogTemplateTest
    {
        private static DateTimeOffset GetDefaultDateTime()
        {
            return new DateTimeOffset(2000, 1, 1, 1, 1, 1, new TimeSpan(0));
        }

        [Test]
        [Description("Checks that a null message cannot be translated.")]
        public void TranslateWithNullMessage()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.Throws<ArgumentNullException>(() => template.Translate(null));
        }

        [Test]
        [Description("Checks that a message is translated correctly.")]
        public void Translate()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            var msg = new LogMessage("bla", LevelToLog.Info, "blabla");
            var text = template.Translate(msg);

            var expectedText = string.Format(CultureInfo.CurrentCulture, CommandLogTemplate.CommandLogFormat, GetDefaultDateTime(), msg.Origin, msg.Text());
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        [Description("Checks that the correct log type is returned.")]
        public void GetLogType()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.AreEqual(LogType.Command, template.LogType);
        }

        [Test]
        [Description("Checks that a null object is not equal to a given template.")]
        public void EqualsWithNullObject()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals((object)null));
        }

        [Test]
        [Description("Checks that an object of a different type is not equal to a given template.")]
        public void EqualsWithNonEqualType()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals(new object()));
        }

        [Test]
        [Description("Checks that a non-equal object is not considered equal to a given template.")]
        public void EqualsWithNonEqualObject()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals((object)new DebugLogTemplate(GetDefaultDateTime)));
        }

        [Test]
        [Description("Checks that an equal object is considered equal to a given template.")]
        public void EqualsWithEqualObject()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.IsTrue(template.Equals((object)new CommandLogTemplate(GetDefaultDateTime)));
        }

        [Test]
        [Description("Checks that an equal object is considered equal to itself.")]
        public void EqualsWithSameObject()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.IsTrue(template.Equals((object)template));
        }

        [Test]
        [Description("Checks that a null ILogTemplate is not equal to a given template.")]
        public void EqualsWithNullILogTemplate()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals((ILogTemplate)null));
        }

        [Test]
        [Description("Checks that a non-equal ILogTemplate is not considered equal to a given template.")]
        public void EqualsWithNonEqualILogTemplate()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals(new DebugLogTemplate(GetDefaultDateTime)));
        }

        [Test]
        [Description("Checks that an equal IlogTemplate is considered equal to a given template.")]
        public void EqualsWithEqualILogTemplate()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.IsTrue(template.Equals((ILogTemplate)new CommandLogTemplate(GetDefaultDateTime)));
        }

        [Test]
        [Description("Checks that an equal IlogTemplate is considered equal to itself.")]
        public void EqualsWithSameILogTemplate()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.IsTrue(template.Equals((ILogTemplate)template));
        }

        [Test]
        [Description("Checks that a null CommandLogTemplate is not equal to a given template.")]
        public void EqualsWithNullCommandLogTemplate()
        {
            var template = new CommandLogTemplate(GetDefaultDateTime);
            Assert.IsFalse(template.Equals(null));
        }
    }
}