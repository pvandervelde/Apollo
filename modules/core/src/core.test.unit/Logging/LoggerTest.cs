//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Apollo.Core.Utils;
using Apollo.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Logging
{
    [TestFixture]
    [Description("Tests the Logger class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LoggerTest
    {
        [Test]
        [Description("Checks that a logger cannot be created without a configuration.")]
        public void CreateWithoutConfiguration()
        {
            Assert.Throws<ArgumentNullException>(() => new Logger(null, new DebugLogTemplate(() => DateTime.Now), new FileConstants(new ApplicationConstants())));
        }

        [Test]
        [Description("Checks that a logger cannot be created without a template.")]
        public void CreateWithoutTemplate()
        {
            Assert.Throws<ArgumentNullException>(() => new Logger(new LoggerConfiguration("someDir", 1, 1), null, new FileConstants(new ApplicationConstants())));
        }

        [Test]
        [Description("Checks that a logger cannot be created without the file constants.")]
        public void CreateWithoutFileConstants()
        {
            Assert.Throws<ArgumentNullException>(() => new Logger(new LoggerConfiguration("someDir", 1, 1), new DebugLogTemplate(() => DateTime.Now), null));
        }

        [Test]
        [Description("Checks that a change of the log level stores the correct level.")]
        public void ChangeLevel()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(LevelToLog.Error, logger.Level);

            logger.ChangeLevel(LevelToLog.Trace);
            Assert.AreEqual(LevelToLog.Trace, logger.Level);
        }

        [Test]
        [Description("Checks that a message with a log level below the threshold does not get logged.")]
        public void ShouldLogWithMessageThatShouldNotBeLogged()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(logger.ShouldLog(new LogMessage("fromhere", LevelToLog.Trace, "This is an interesting message")));
        }

        [Test]
        [Description("Checks that a message with a log level equal or higher than the threshold gets logged.")]
        public void ShouldLogWithMessageThatShouldBeLogged()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsTrue(logger.ShouldLog(new LogMessage("fromhere", LevelToLog.Error, "This is an interesting message")));
        }
    }
}