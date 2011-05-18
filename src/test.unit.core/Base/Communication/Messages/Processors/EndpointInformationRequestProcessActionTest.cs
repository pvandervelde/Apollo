//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Communication.Messages.Processors;
using Apollo.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication.Messages.Processors
{
    [TestFixture]
    [Description("Tests the EndpointInformationRequestProcessAction class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointInformationRequestProcessActionTest
    {
        [Test]
        [Description("Checks that the object processes the correct message type.")]
        public void MessageTypeToProcess()
        {
            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { };
            var commands = new Mock<ICommandCollection>();
            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new EndpointInformationRequestProcessAction(endpoint, sendAction, commands.Object, logger);
            Assert.AreEqual(typeof(EndpointInformationRequestMessage), action.MessageTypeToProcess);
        }

        [Test]
        [Description("Checks that the message is processed correctly.")]
        public void Invoke()
        {
            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(ICommunicationChannelCommands), new Mock<ICommunicationChannelCommands>().Object)
                };

            var endpoint = new EndpointId("id");

            EndpointId storedEndpoint = null;
            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
                {
                    storedEndpoint = e;
                    storedMsg = m;
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new EndpointInformationRequestProcessAction(endpoint, sendAction, commands.Object, logger);

            var otherEndpoint = new EndpointId("otherId");
            action.Invoke(new EndpointInformationRequestMessage(otherEndpoint));

            Assert.AreSame(otherEndpoint, storedEndpoint);
            Assert.IsInstanceOfType(typeof(EndpointInformationResponseMessage), storedMsg);

            var responseMsg = storedMsg as EndpointInformationResponseMessage;
            Assert.AreElementsEqual(
                new List<ISerializedType> { CommandSetProxyExtensions.FromType(typeof(ICommunicationChannelCommands)) }, 
                responseMsg.Commands, 
                (x, y) => x.Equals(y));
        }

        [Test]
        [Description("Checks that the message is processed correctly.")]
        public void InvokeWithFailingResponse()
        {
            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(ICommunicationChannelCommands), new Mock<ICommunicationChannelCommands>().Object)
                };

            var endpoint = new EndpointId("id");

            int count = 0;
            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => 
                {
                    count++;
                    if (count <= 1)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        storedMsg = m;
                    }
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new EndpointInformationRequestProcessAction(endpoint, sendAction, commands.Object, logger);
            action.Invoke(new EndpointInformationRequestMessage(new EndpointId("otherId")));

            Assert.AreEqual(2, count);
            Assert.IsInstanceOfType(typeof(FailureMessage), storedMsg);
        }

        [Test]
        [Description("Checks that the message is processed correctly.")]
        public void InvokeWithFailedChannel()
        {
            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(ICommunicationChannelCommands), new Mock<ICommunicationChannelCommands>().Object)
                };

            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { throw new Exception(); };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            int count = 0;
            Action<LogSeverityProxy, string> logger = (p, t) => { count++; };

            var action = new EndpointInformationRequestProcessAction(endpoint, sendAction, commands.Object, logger);
            action.Invoke(new EndpointInformationRequestMessage(new EndpointId("otherId")));

            Assert.AreEqual(2, count);
        }
    }
}
