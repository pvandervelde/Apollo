//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Logging
{
    [TestFixture]
    [Description("Tests the LogLevelChangeRequestMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LogLevelChangeRequestMessageTest
    {
        private static List<LevelToLog> LogLevelEnumerator()
        {
            return new List<LevelToLog> 
                {
                    LevelToLog.Trace,
                    LevelToLog.Debug,
                    LevelToLog.Info,
                    LevelToLog.Warn,
                    LevelToLog.Error,
                    LevelToLog.Fatal,
                };
        }

        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<LogLevelChangeRequestMessage>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = LogLevelEnumerator().Select(o => new LogLevelChangeRequestMessage(o)),
        };

        [Test]
        [Description("Checks that the new log level is properly stored.")]
        public void Level()
        {
            var message = new LogLevelChangeRequestMessage(LevelToLog.Info);
            Assert.AreEqual(LevelToLog.Info, message.Level);
        }

        [Test]
        [Description("Checks that a message is not equal to a null reference.")]
        public void EqualsWithNullObject()
        {
            var message = new LogLevelChangeRequestMessage(LevelToLog.Info);
            object nullReference = null;

            Assert.IsFalse(message.Equals(nullReference));
        }

        [Test]
        [Description("Checks that a message is not equal to an object of a different type.")]
        public void EqualsWithDifferentType()
        {
            var message = new LogLevelChangeRequestMessage(LevelToLog.Info);
            var obj = new object();

            Assert.IsFalse(message.Equals(obj));
        }

        [Test]
        [Description("Checks that a message is not equal to a non-equal object of equal type.")]
        public void EqualsWithNonEqualObjects()
        {
            var message1 = new LogLevelChangeRequestMessage(LevelToLog.Info);
            var message2 = new LogLevelChangeRequestMessage(LevelToLog.Warn);

            Assert.IsFalse(message1.Equals((object)message2));
            Assert.IsFalse(message2.Equals((object)message1));
        }

        [Test]
        [Description("Checks that a message is equal to an equal object of equal type.")]
        public void EqualsWithEqualObjects()
        {
            var message1 = new LogLevelChangeRequestMessage(LevelToLog.Info);
            var message2 = (LogLevelChangeRequestMessage)message1.Copy();

            Assert.IsTrue(message1.Equals((object)message2));
            Assert.IsTrue(message2.Equals((object)message1));
        }

        [Test]
        [Description("Checks that a message is equal to itself.")]
        public void EqualsWithSameObject()
        {
            var message = new LogLevelChangeRequestMessage(LevelToLog.Info);
            Assert.IsTrue(message.Equals((object)message));
        }

        [Test]
        [Description("Checks that a message is not equal to a null reference.")]
        public void EqualsWithNullMessage()
        {
            var message = new LogLevelChangeRequestMessage(LevelToLog.Info);
            LogLevelChangeRequestMessage nullReference = null;

            Assert.IsFalse(message.Equals(nullReference));
        }

        [Test]
        [Description("Checks that a message is not equal to an message of a different type.")]
        public void EqualsWithDifferentMessageType()
        {
            var message1 = new LogLevelChangeRequestMessage(LevelToLog.Info);
            var message2 = new LogEntryRequestMessage(new LogMessage("bla", LevelToLog.Fatal, "Something bad happened"), LogType.Debug);

            Assert.IsFalse(message1.Equals(message2));
        }

        [Test]
        [Description("Checks that a message is not equal to a non-equal message of equal type.")]
        public void EqualsWithNonEqualMessages()
        {
            var message1 = new LogLevelChangeRequestMessage(LevelToLog.Info);
            var message2 = new LogLevelChangeRequestMessage(LevelToLog.Warn);

            Assert.IsFalse(message1.Equals(message2));
            Assert.IsFalse(message2.Equals(message1));
        }

        [Test]
        [Description("Checks that a message is equal to an equal message of equal type.")]
        public void EqualsWithEqualMessages()
        {
            var message1 = new LogLevelChangeRequestMessage(LevelToLog.Info);
            var message2 = (LogLevelChangeRequestMessage)message1.Copy();

            Assert.IsTrue(message1.Equals(message2));
            Assert.IsTrue(message2.Equals(message1));
        }

        [Test]
        [Description("Checks that a message is equal to itself.")]
        public void EqualsWithSameMessage()
        {
            var message = new LogLevelChangeRequestMessage(LevelToLog.Info);
            Assert.IsTrue(message.Equals(message));
        }
    }
}