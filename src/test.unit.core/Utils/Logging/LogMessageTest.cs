//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Utils.Logging
{
    [TestFixture]
    [Description("Tests the LogMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LogMessageTest
    {
        [Test]
        [Description("Checks a LogMessage cannot be created with a null origin.")]
        public void CreateWithNullOrigin()
        {
            Assert.Throws<ArgumentNullException>(() => new LogMessage(null, LevelToLog.Debug, "Dummy text"));
        }

        [Test]
        [Description("Checks a LogMessage cannot be created with an empty origin.")]
        public void CreateWithEmptyOrigin()
        {
            Assert.Throws<ArgumentException>(() => new LogMessage(string.Empty, LevelToLog.Debug, "Dummy text"));
        }

        [Test]
        [Description("Checks a LogMessage cannot be created with a log level set to None.")]
        public void CreateWithLogLevelNone()
        {
            Assert.Throws<ArgumentException>(() => new LogMessage("Origin", LevelToLog.None, "Dummy text"));
        }

        [Test]
        [Description("Checks a LogMessage cannot be created with a null text.")]
        public void CreateWithNullText()
        {
            Assert.Throws<ArgumentNullException>(() => new LogMessage("Origin", LevelToLog.Debug, null));
        }

        [Test]
        [Description("Checks a LogMessage can be created.")]
        public void Create()
        {
            var origin = "origin";
            var level = LevelToLog.Debug;
            var text = "text";

            var message = new LogMessage(origin, level, text);

            Assert.AreEqual(origin, message.Origin);
            Assert.AreEqual(level, message.Level);
            Assert.AreEqual(text, message.Text());
            Assert.IsFalse(message.HasAdditionalInformation);
        }

        [Test]
        [Description("Checks a LogMessage can be created.")]
        public void CreateWithProperties()
        {
            var origin = "origin";
            var level = LevelToLog.Debug;
            var text = "text";
            var properties = new Dictionary<string, object>();

            var message = new LogMessage(origin, level, text, properties);

            Assert.AreEqual(origin, message.Origin);
            Assert.AreEqual(level, message.Level);
            Assert.AreEqual(text, message.Text());
            Assert.IsTrue(message.HasAdditionalInformation);
            Assert.AreSame(properties, message.Properties);
        }
    }
}