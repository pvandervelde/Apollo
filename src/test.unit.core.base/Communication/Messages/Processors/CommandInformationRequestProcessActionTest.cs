//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Plugins;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Base.Communication.Messages.Processors
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
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new CommandInformationRequestProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            Assert.AreEqual(typeof(CommandInformationRequestMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(IGroupCompositionCommands), new Mock<IGroupCompositionCommands>().Object)
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

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new CommandInformationRequestProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);

            var otherEndpoint = new EndpointId("otherId");
            action.Invoke(new CommandInformationRequestMessage(otherEndpoint));

            Assert.AreSame(otherEndpoint, storedEndpoint);
            Assert.IsInstanceOfType(typeof(EndpointProxyTypesResponseMessage), storedMsg);

            var responseMsg = storedMsg as EndpointProxyTypesResponseMessage;
            Assert.AreElementsEqual(
                new List<ISerializedType> { ProxyExtensions.FromType(typeof(IGroupCompositionCommands)) }, 
                responseMsg.ProxyTypes, 
                (x, y) => x.Equals(y));
        }

        [Test]
        public void InvokeWithFailingResponse()
        {
            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(IGroupCompositionCommands), new Mock<IGroupCompositionCommands>().Object)
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

            int loggerCount = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { loggerCount++; }, null);

            var action = new CommandInformationRequestProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            action.Invoke(new CommandInformationRequestMessage(new EndpointId("otherId")));

            Assert.AreEqual(2, count);
            Assert.AreEqual(1, loggerCount);
            Assert.IsInstanceOfType(typeof(FailureMessage), storedMsg);
        }

        [Test]
        public void InvokeWithFailedChannel()
        {
            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(IGroupCompositionCommands), new Mock<IGroupCompositionCommands>().Object)
                };

            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { throw new Exception(); };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            int count = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { count++; }, null);

            var action = new CommandInformationRequestProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            action.Invoke(new CommandInformationRequestMessage(new EndpointId("otherId")));

            Assert.AreEqual(2, count);
        }
    }
}
