//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Communication.Messages.Processors;
using Apollo.Utilities;
using MbUnit.Framework;

namespace Apollo.Base.Communication.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class UnknownMessageTypeProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { };
            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new UnknownMessageTypeProcessAction(endpoint, sendAction, logger);
            Assert.AreEqual(typeof(ICommunicationMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            var endpoint = new EndpointId("id");

            EndpointId storedEndpoint = null;
            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
            {
                storedEndpoint = e;
                storedMsg = m;
            };

            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new UnknownMessageTypeProcessAction(endpoint, sendAction, logger);

            var otherEndpoint = new EndpointId("otherId");
            action.Invoke(new CommandInformationRequestMessage(otherEndpoint));

            Assert.AreSame(otherEndpoint, storedEndpoint);
            Assert.IsInstanceOfType(typeof(UnknownMessageTypeMessage), storedMsg);
        }

        [Test]
        public void InvokeWithFailingResponse()
        {
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

            int loggerCount = 0;
            Action<LogSeverityProxy, string> logger = (p, t) => { loggerCount++; };

            var action = new UnknownMessageTypeProcessAction(endpoint, sendAction, logger);
            action.Invoke(new CommandInformationRequestMessage(new EndpointId("otherId")));

            Assert.AreEqual(2, count);
            Assert.AreEqual(1, loggerCount);
            Assert.IsInstanceOfType(typeof(FailureMessage), storedMsg);
        }

        [Test]
        public void InvokeWithFailedChannel()
        {
            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { throw new Exception(); };

            int count = 0;
            Action<LogSeverityProxy, string> logger = (p, t) => { count++; };

            var action = new UnknownMessageTypeProcessAction(endpoint, sendAction, logger);
            action.Invoke(new CommandInformationRequestMessage(new EndpointId("otherId")));

            Assert.AreEqual(2, count);
        }
    }
}
