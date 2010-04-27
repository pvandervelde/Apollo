//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Messaging;
using Apollo.Core.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Logging
{
    [TestFixture]
    [Description("Tests the LogSink class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LogSinkTest
    {
        #region Internal class - MockKernelService

        /// <summary>
        /// Defines a mock implementation of the <see cref="KernelService"/> abstract class.
        /// </summary>
        /// <design>
        /// This class is defined because Moq is unable to create Mock objects for 
        /// classes that are not publicly available (like the KernelService class),
        /// even if those interfaces are reachable through an InternalsVisibleToAttribute.
        /// </design>
        private sealed class MockKernelService : KernelService
        {
            #region Overrides of KernelService

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform startup tasks.
            /// </summary>
            protected override void StartService()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform shutdown tasks.
            /// </summary>
            protected override void StopService()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        [Test]
        [Description("Checks that the object returns the correct names for services that should be available.")]
        public void ServicesToBeAvailable()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(0, service.ServicesToBeAvailable().Count());
        }

        [Test]
        [Description("Checks that the object returns the correct names for services to which it should be connected.")]
        public void ServicesToConnectTo()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.AreElementsEqual(new[] { typeof(IMessagePipeline) }, service.ServicesToConnectTo());
        }

        [Test]
        [Description("Checks that the object can be connected to the dependencies.")]
        public void ConnectTo()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from an unknown dependency.")]
        public void DisconnectFromWithNonMatchingServiceType()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MockKernelService());
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from a non-matching reference.")]
        public void DisconnectFromWithNonMatchingObjectReference()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MessagePipeline(new DnsNameConstants()));
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object can be disconnected from the dependencies.")]
        public void DisconnectFrom()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);

            service.DisconnectFrom(pipeline);
            Assert.IsFalse(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the log level for the debug logger can be obtained.")]
        public void LogLevelForTheDebugLogger()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(LevelToLog.Error, service.Level(LogType.Debug));
        }

        [Test]
        [Description("Checks that the log level for the command logger can be obtained.")]
        public void LogLevelForTheCommandLogger()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(LevelToLog.Error, service.Level(LogType.Command));
        }

        [Test]
        [Description("Checks that the object never logs messages for unknown log types.")]
        public void ShouldLogWithUnknownLogType()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
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
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(service.ShouldLogMessage(LogType.Debug, new LogMessage("fromHere", LevelToLog.Trace, "Panic!")));
        }

        [Test]
        [Description("Checks that the object does log messages for log levels that are equal or higher than the loggers level.")]
        public void ShouldLog()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsTrue(service.ShouldLogMessage(LogType.Debug, new LogMessage("fromHere", LevelToLog.Error, "Panic!")));
        }
    }
}