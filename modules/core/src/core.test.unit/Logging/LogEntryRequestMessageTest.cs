//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Messaging;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

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
        [Serializable]
        private sealed class MockMessage : ILogMessage
        {
            private readonly string m_Text;

            public MockMessage()
            { 
            }

            public MockMessage(string origin, LevelToLog level, string text)
            {
                Origin = origin;
                Level = level;
                m_Text = text;
            }

            #region Implementation of ILogMessage

            /// <summary>
            /// Gets the origin of the message. The origin can for instance be the
            /// type from which the message came.
            /// </summary>
            /// <value>The type of the owner.</value>
            public string Origin
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the desired log level for this message.
            /// </summary>
            /// <value>The desired level.</value>
            public LevelToLog Level
            {
                get;
                private set;
            }

            /// <summary>
            /// Returns the message text for this message.
            /// </summary>
            /// <returns>
            /// The text for this message.
            /// </returns>
            public string Text()
            {
                return m_Text;
            }

            #endregion

            public override bool Equals(object obj)
            {
                var other = obj as MockMessage;
                return (other != null) && (other.Text() == Text()) && (other.Level == Level) && (other.Origin == Origin);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #endregion

        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<LogEntryRequestMessage>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = DataGenerators.Join(
                    new List<ILogMessage> 
                        {
                            new MockMessage(),
                            new MockMessage(),
                            new MockMessage(),
                            new MockMessage(),
                            new MockMessage(),
                        },
                    new List<LogType> 
                        {
                            LogType.Command,
                            LogType.Debug,
                        })
                .Select(o => new LogEntryRequestMessage(o.First, o.Second)),
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<MessageBody>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection<MessageBody> 
                { 
                    new LogEntryRequestMessage(new MockMessage("a", LevelToLog.Info, "b"), LogType.Debug),
                    new LogEntryRequestMessage(new MockMessage("c", LevelToLog.Warn, "d"), LogType.Debug),
                    new LogEntryRequestMessage(new MockMessage("a", LevelToLog.Info, "b"), LogType.Command),
                },
        };

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
        [Description("Checks that the message serialises and deserialises correctly.")]
        public void RoundTripSerialise()
        {
            var msg = new LogEntryRequestMessage(new MockMessage("a", LevelToLog.Info, "b"), LogType.Command);
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            AssertEx.That(
               () => msg.IsResponseRequired == otherMsg.IsResponseRequired
                  && msg.LogType == otherMsg.LogType
                  && msg.Message.Equals(otherMsg.Message));
        }
    }
}