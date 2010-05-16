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
        #region internal class - MockLogMessage

        private sealed class MockLogMessage : ILogMessage
        {
            private readonly string m_Text;

            public MockLogMessage(string origin, LevelToLog level, string text)
            {
                Origin = origin;
                Level = level;
                m_Text = text;
            }

            public string Origin
            {
                get;
                private set;
            }

            public LevelToLog Level
            {
                get;
                private set;
            }

            public string Text()
            {
                return m_Text;
            }
        }
        
        #endregion

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
            var template = new DebugLogTemplate(() => DateTime.Now);
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                template,
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), logger.Level);

            logger.ChangeLevel(LevelToLog.Trace);
            Assert.AreEqual(LevelToLog.Trace, logger.Level);
        }

        [Test]
        [Description("Checks that a null message does not get logged.")]
        public void ShouldLogWithNullMessage()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(logger.ShouldLog(null));
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

        [Test]
        [Description("Checks that messages are not logged if the LevelToLog is set to None.")]
        public void ShouldLogMessageAtLevelNone()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            logger.ChangeLevel(LevelToLog.None);
            Assert.IsFalse(logger.ShouldLog(new MockLogMessage("fromhere", LevelToLog.None, "This is an interesting message")));
        }

        [Test]
        [Description("Checks that messages are not logged if the LevelToLog is set to None.")]
        public void ShouldLogMessageWithLevelNone()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(logger.ShouldLog(new MockLogMessage("fromhere", LevelToLog.None, "This is an interesting message")));
        }

        [Test]
        [Description("Checks that the LevelToLog can be switched to Trace.")]
        public void SwitchLevelToLogToTrace()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var template = new DebugLogTemplate(() => DateTime.Now);
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                template,
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), logger.Level);

            logger.ChangeLevel(LevelToLog.Trace);
            Assert.AreEqual(LevelToLog.Trace, logger.Level);
        }

        [Test]
        [Description("Checks that the LevelToLog can be switched to Debug.")]
        public void SwitchLevelToLogToDebug()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var template = new DebugLogTemplate(() => DateTime.Now);
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                template,
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), logger.Level);

            logger.ChangeLevel(LevelToLog.Debug);
            Assert.AreEqual(LevelToLog.Debug, logger.Level);
        }

        [Test]
        [Description("Checks that the LevelToLog can be switched to Info.")]
        public void SwitchLevelToLogToInfo()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var template = new DebugLogTemplate(() => DateTime.Now);
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                template,
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), logger.Level);

            logger.ChangeLevel(LevelToLog.Info);
            Assert.AreEqual(LevelToLog.Info, logger.Level);
        }

        [Test]
        [Description("Checks that the LevelToLog can be switched to Warn.")]
        public void SwitchLevelToLogToWarn()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var template = new DebugLogTemplate(() => DateTime.Now);
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                template,
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), logger.Level);

            logger.ChangeLevel(LevelToLog.Warn);
            Assert.AreEqual(LevelToLog.Warn, logger.Level);
        }

        [Test]
        [Description("Checks that the LevelToLog can be switched to Error.")]
        public void SwitchLevelToLogToError()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var template = new DebugLogTemplate(() => DateTime.Now);
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                template,
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), logger.Level);

            logger.ChangeLevel(LevelToLog.Error);
            Assert.AreEqual(LevelToLog.Error, logger.Level);
        }

        [Test]
        [Description("Checks that the LevelToLog can be switched to Fatal.")]
        public void SwitchLevelToLogToFatal()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var template = new DebugLogTemplate(() => DateTime.Now);
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                template,
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), logger.Level);

            logger.ChangeLevel(LevelToLog.Fatal);
            Assert.AreEqual(LevelToLog.Fatal, logger.Level);
        }

        [Test]
        [Description("Checks that the LevelToLog can be switched to Off.")]
        public void SwitchLevelToLogToOff()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().LocalFilePath());
            var template = new DebugLogTemplate(() => DateTime.Now);
            var logger = new Logger(
                new LoggerConfiguration(assemblyDirectory, 1, 1),
                template,
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), logger.Level);

            logger.ChangeLevel(LevelToLog.None);
            Assert.AreEqual(LevelToLog.None, logger.Level);
        }
    }
}