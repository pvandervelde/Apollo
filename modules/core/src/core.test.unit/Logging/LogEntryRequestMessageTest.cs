//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.Logging
{
    [TestFixture]
    [Description("Tests the LogEntryRequestMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LogEntryRequestMessageTest
    {
        #region Internal class - MockMessage

        /// <summary>
        /// A mock implementation of <see cref="ILogMessage"/>.
        /// </summary>
        private sealed class MockMessage : ILogMessage
        {
            #region Implementation of ILogMessage

            /// <summary>
            /// Gets the origin of the message. The origin can for instance be the
            /// type from which the message came.
            /// </summary>
            /// <value>The type of the owner.</value>
            public string Origin
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Gets the desired log level for this message.
            /// </summary>
            /// <value>The desired level.</value>
            public LevelToLog Level
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Returns the message text for this message.
            /// </summary>
            /// <returns>
            /// The text for this message.
            /// </returns>
            public string Text()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        [Test]
        [Description("Checks that the log type is properly stored.")]
        public void CheckLogType()
        {
            var message = new LogEntryRequestMessage(new MockMessage(), LogType.Command);
            Assert.AreEqual(LogType.Command, message.LogType);
        }

        [Test]
        [Description("Checks that the message is properly stored.")]
        public void Message()
        {
            var logMessage = new MockMessage();
            var message = new LogEntryRequestMessage(logMessage, LogType.Command);
            Assert.AreSame(logMessage, message.Message);
        }

        [Test]
        [Description("Checks that a message is not equal to a null reference.")]
        public void EqualsWithNullObject()
        {
            var message = new LogEntryRequestMessage(new MockMessage(), LogType.Command);
            object nullReference = null;

            Assert.IsFalse(message.Equals(nullReference));
        }

        [Test]
        [Description("Checks that a message is not equal to an object of a different type.")]
        public void EqualsWithDifferentType()
        {
            var message = new LogEntryRequestMessage(new MockMessage(), LogType.Command);
            var obj = new object();

            Assert.IsFalse(message.Equals(obj));
        }

        [Test]
        [Description("Checks that a message is not equal to a non-equal object of equal type.")]
        public void EqualsWithNonEqualObjects()
        {
            var logMessage = new MockMessage();

            var message1 = new LogEntryRequestMessage(logMessage, LogType.Command);
            var message2 = new LogEntryRequestMessage(logMessage, LogType.Debug);

            Assert.IsFalse(message1.Equals((object)message2));
            Assert.IsFalse(message2.Equals((object)message1));
        }

        [Test]
        [Description("Checks that a message is equal to an equal object of equal type.")]
        public void EqualsWithEqualObjects()
        {
            var logMessage = new MockMessage();

            var message1 = new LogEntryRequestMessage(logMessage, LogType.Command);
            var message2 = (LogEntryRequestMessage)message1.Copy();

            Assert.IsTrue(message1.Equals((object)message2));
            Assert.IsTrue(message2.Equals((object)message1));
        }

        [Test]
        [Description("Checks that a message is equal to itself.")]
        public void EqualsWithSameObjects()
        {
            var logMessage = new MockMessage();
            var message1 = new LogEntryRequestMessage(logMessage, LogType.Command);

            Assert.IsTrue(message1.Equals((object)message1));
        }

        [Test]
        [Description("Checks that a message is not equal to a null reference.")]
        public void EqualsWithNullMessage()
        {
            var message = new LogEntryRequestMessage(new MockMessage(), LogType.Command);
            LogEntryRequestMessage nullReference = null;

            Assert.IsFalse(message.Equals(nullReference));
        }

        [Test]
        [Description("Checks that a message is not equal to an message of a different type.")]
        public void EqualsWithDifferentMessageType()
        {
            var message = new LogEntryRequestMessage(new MockMessage(), LogType.Command);
            var msg = new LogLevelChangeRequestMessage(LevelToLog.Fatal);

            Assert.IsFalse(message.Equals(msg));
        }

        [Test]
        [Description("Checks that a message is not equal to a non-equal message of equal type.")]
        public void EqualsWithNonEqualMessages()
        {
            var logMessage = new MockMessage();

            var message1 = new LogEntryRequestMessage(logMessage, LogType.Command);
            var message2 = new LogEntryRequestMessage(logMessage, LogType.Debug);

            Assert.IsFalse(message1.Equals(message2));
            Assert.IsFalse(message2.Equals(message1));
        }

        [Test]
        [Description("Checks that a message is equal to an equal message of equal type.")]
        public void EqualsWithEqualMessages()
        {
            var logMessage = new MockMessage();

            var message1 = new LogEntryRequestMessage(logMessage, LogType.Command);
            var message2 = (LogEntryRequestMessage)message1.Copy();

            Assert.IsTrue(message1.Equals(message2));
            Assert.IsTrue(message2.Equals(message1));
        }

        [Test]
        [Description("Checks that a message is equal to itself.")]
        public void EqualsWithSameMessage()
        {
            var logMessage = new MockMessage();
            var message1 = new LogEntryRequestMessage(logMessage, LogType.Command);

            Assert.IsTrue(message1.Equals(message1));
        }
    }
}