//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the CoreProxy class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LogMessageForKernelCommandTest
    {
        [Test]
        [Description("Checks that an object cannot be created with a null LogSink DnsName.")]
        public void CreateWithNullLogSinkName()
        {
            SendMessageWithoutResponse messageSender = (recipient, msg, id) => { };
            Assert.Throws<ArgumentNullException>(() => new LogMessageForKernelCommand(null, messageSender));
        }

        [Test]
        [Description("Checks that an object cannot be created with a null message sending delegate.")]
        public void CreateWithNullSenderDelegate()
        {
            Assert.Throws<ArgumentNullException>(() => new LogMessageForKernelCommand(new DnsName("logger"), null));
        }

        [Test]
        [Description("Checks that the Invoke method sends the log message correctly.")]
        public void Invoke()
        {
            DnsName target = null;
            MessageBody body = null;
            MessageId responseId = null;

            var dnsName = new DnsName("logger");
            SendMessageWithoutResponse messageSender = 
                (recipient, msg, id) =>
                    {
                        target = recipient;
                        body = msg;
                        responseId = id;
                    };

            var command = new LogMessageForKernelCommand(dnsName, messageSender);

            var level = LevelToLog.Fatal;
            var text = "Some fatal thing happened.";
            var context = new LogMessageForKernelContext(level, text);

            command.Invoke(context);

            // Check that the message was send correctly
            {
                Assert.AreEqual(MessageId.None, responseId);
                Assert.AreEqual(dnsName, target);

                LogEntryRequestMessage message = (LogEntryRequestMessage)body;
                Assert.AreEqual(LogType.Debug, message.LogType);
                Assert.AreEqual(level, message.Message.Level);
                Assert.AreEqual(text, message.Message.Text());
            }
        }
    }
}
