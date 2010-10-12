//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Messaging;
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

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<MessageBody>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection<MessageBody> 
                { 
                    new LogLevelChangeRequestMessage(LevelToLog.Trace),
                    new LogLevelChangeRequestMessage(LevelToLog.Debug),
                    new LogLevelChangeRequestMessage(LevelToLog.Info),
                    new LogLevelChangeRequestMessage(LevelToLog.Warn),
                    new LogLevelChangeRequestMessage(LevelToLog.Error),
                    new LogLevelChangeRequestMessage(LevelToLog.Fatal),
                },
        };

        [Test]
        [Description("Checks that the new log level is properly stored.")]
        public void Level()
        {
            var message = new LogLevelChangeRequestMessage(LevelToLog.Info);
            Assert.AreEqual(LevelToLog.Info, message.Level);
        }

        [Test]
        [Description("Checks that the message serialises and deserialises correctly.")]
        public void RoundTripSerialise()
        {
            var msg = new LogLevelChangeRequestMessage(LevelToLog.Info);
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            AssertEx.That(
               () => msg.IsResponseRequired == otherMsg.IsResponseRequired
                  && msg.Level == otherMsg.Level);
        }
    }
}