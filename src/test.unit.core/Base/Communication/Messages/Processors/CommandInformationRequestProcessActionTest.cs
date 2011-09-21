//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Communication.Messages.Processors;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class CommandInformationRequestProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { };
            var commands = new Mock<ICommandCollection>();
            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new CommandInformationRequestProcessAction(endpoint, sendAction, commands.Object, logger);
            Assert.AreEqual(typeof(CommandInformationRequestMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(IHostCommands), new Mock<IHostCommands>().Object)
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

            var action = new CommandInformationRequestProcessAction(endpoint, sendAction, commands.Object, logger);

            var otherEndpoint = new EndpointId("otherId");
            action.Invoke(new CommandInformationRequestMessage(otherEndpoint));

            Assert.AreSame(otherEndpoint, storedEndpoint);
            Assert.IsInstanceOfType(typeof(EndpointProxyTypesResponseMessage), storedMsg);

            var responseMsg = storedMsg as EndpointProxyTypesResponseMessage;
            Assert.AreElementsEqual(
                new List<ISerializedType> { ProxyExtensions.FromType(typeof(IHostCommands)) }, 
                responseMsg.ProxyTypes, 
                (x, y) => x.Equals(y));
        }

        [Test]
        public void InvokeWithFailingResponse()
        {
            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(IHostCommands), new Mock<IHostCommands>().Object)
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

            var action = new CommandInformationRequestProcessAction(endpoint, sendAction, commands.Object, logger);
            action.Invoke(new CommandInformationRequestMessage(new EndpointId("otherId")));

            Assert.AreEqual(2, count);
            Assert.IsInstanceOfType(typeof(FailureMessage), storedMsg);
        }

        [Test]
        public void InvokeWithFailedChannel()
        {
            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(IHostCommands), new Mock<IHostCommands>().Object)
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

            var action = new CommandInformationRequestProcessAction(endpoint, sendAction, commands.Object, logger);
            action.Invoke(new CommandInformationRequestMessage(new EndpointId("otherId")));

            Assert.AreEqual(2, count);
        }
    }
}
