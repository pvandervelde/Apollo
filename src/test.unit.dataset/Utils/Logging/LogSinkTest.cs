//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Utils;
using MbUnit.Framework;

namespace Apollo.Core.Logging
{
    [TestFixture]
    [Description("Tests the LogSink class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LogSinkTest
    {
        [Test]
        [Description("Checks that the object returns the correct names for services to which it should be connected.")]
        public void ServicesToConnectTo()
        {
            var service = new LogSink(
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(0, service.ServicesToConnectTo().Count());
        }

        [Test]
        [Description("Checks that the log level for the debug logger can be obtained.")]
        public void LogLevelForTheDebugLogger()
        {
            var template = new DebugLogTemplate(() => DateTime.Now);
            var service = new LogSink(
                new LoggerConfiguration("someDir", 1, 1),
                template,
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), service.Level(LogType.Debug));
        }

        [Test]
        [Description("Checks that the log level for the command logger can be obtained.")]
        public void LogLevelForTheCommandLogger()
        {
            var template = new CommandLogTemplate(() => DateTime.Now);
            var service = new LogSink(
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                template,
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), service.Level(LogType.Command));
        }

        [Test]
        [Description("Checks that the object never logs messages for unknown log types.")]
        public void ShouldLogWithUnknownLogType()
        {
            var service = new LogSink(
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(service.ShouldLogMessage(LogType.None, new LogMessage("fromHere", LevelToLog.Fatal, "Panic!")));
        }

        [Test]
        [Description("Checks that the object does not log messages for log levels that are below the loggers level.")]
        public void ShouldLogWithTooLowLevel()
        {
            var service = new LogSink(
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(service.ShouldLogMessage(LogType.Debug, new LogMessage("fromHere", LevelToLog.Trace, "Panic!")));
        }

        [Test]
        [Description("Checks that the object does not log messages if the service is not fully functional.")]
        public void ShouldLogWhileNotFullyFunctional()
        {
            var service = new LogSink(
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(service.ShouldLogMessage(LogType.Debug, new LogMessage("fromHere", LevelToLog.Error, "Panic!")));
        }

        [Test]
        [Description("Checks that the object logs messages for log levels that are equal or higher than the loggers level.")]
        public void ShouldLog()
        {
            var service = new LogSink(
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            service.Start();
            Assert.IsTrue(service.ShouldLogMessage(LogType.Debug, new LogMessage("fromHere", LevelToLog.Error, "Panic!")));
        }

        [Test]
        [Description("Checks that LogLevelChangeRequestMessage is handled correctly.")]
        public void HandleLogLevelChangeRequestMessage()
        {
            var service = new LogSink(
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            service.Start();

            var newLevel = LevelToLog.Fatal;
            service.Level(LogType.Command, newLevel);
            Assert.AreEqual(newLevel, service.Level(LogType.Command));

            service.Level(LogType.Debug, newLevel);
            Assert.AreEqual(newLevel, service.Level(LogType.Debug));
        }
    }
}