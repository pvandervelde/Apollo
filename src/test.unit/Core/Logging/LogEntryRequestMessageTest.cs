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
using Moq;

namespace Apollo.Core.Logging
{
    [TestFixture]
    [Description("Tests the LogEntryRequestMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LogEntryRequestMessageTest
    {
        private static ILogMessage GenerateMockMessage(string owner, LevelToLog level, string text)
        {
            var msg = new Mock<ILogMessage>();
            {
                msg.Setup(m => m.Origin)
                    .Returns(owner);
                msg.Setup(m => m.Level)
                    .Returns(level);
                msg.Setup(m => m.Text())
                    .Returns(text);
            }

            return msg.Object;
        }

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
                            GenerateMockMessage("a", LevelToLog.Info, "b"),
                            GenerateMockMessage("c", LevelToLog.Info, "d"),
                            GenerateMockMessage("a", LevelToLog.Warn, "b"),
                            GenerateMockMessage("a", LevelToLog.Error, "b"),
                            GenerateMockMessage("a", LevelToLog.Fatal, "b"),
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
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new LogEntryRequestMessage(GenerateMockMessage("a", LevelToLog.Info, "b"), LogType.Debug),
                    new LogEntryRequestMessage(GenerateMockMessage("c", LevelToLog.Warn, "d"), LogType.Debug),
                    new LogEntryRequestMessage(GenerateMockMessage("a", LevelToLog.Info, "b"), LogType.Command),
                },
        };

        [Test]
        [Description("Checks that the log type is properly stored.")]
        public void CheckLogType()
        {
            var message = new Mock<ILogMessage>();
            {
                message.Setup(m => m.Level)
                    .Returns(LevelToLog.Info);
                message.Setup(m => m.Origin)
                    .Returns("fromhere");
                message.Setup(m => m.Text())
                    .Returns("This is an interesting message.");
            }

            var msg = new LogEntryRequestMessage(message.Object, LogType.Command);
            Assert.AreEqual(LogType.Command, msg.LogType);
        }

        [Test]
        [Description("Checks that the message is properly stored.")]
        public void Message()
        {
            var message = new Mock<ILogMessage>();
            {
                message.Setup(m => m.Level)
                    .Returns(LevelToLog.None);
                message.Setup(m => m.Origin)
                    .Returns("fromhere");
                message.Setup(m => m.Text())
                    .Returns("This is an interesting message.");
            }

            var msg = new LogEntryRequestMessage(message.Object, LogType.Command);
            Assert.AreSame(message.Object, msg.Message);
        }
    }
}