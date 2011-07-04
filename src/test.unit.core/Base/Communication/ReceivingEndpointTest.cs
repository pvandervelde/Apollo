//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Utilities;
using MbUnit.Framework;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the ReceivingEndpoint class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ReceivingEndpointTest
    {
        [Test]
        [Description("Checks that a message can be accepted.")]
        public void AcceptMessage()
        {
            Action<LogSeverityProxy, string> logger = (level, m) => { };

            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);

            var endpoint = new ReceivingEndpoint(logger);
            endpoint.OnNewMessage += (s, e) => Assert.AreSame(msg, e.Message);

            endpoint.AcceptMessage(msg);
        }

        [Test]
        [Description("Checks that a message which raises an exception doesn't bring down the system.")]
        public void AcceptMessageThrowingException()
        {
            Action<LogSeverityProxy, string> logger = (level, m) => { };

            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);

            var endpoint = new ReceivingEndpoint(logger);
            endpoint.OnNewMessage += 
                (s, e) => 
                { 
                    throw new Exception(); 
                };

            Assert.DoesNotThrow(() => endpoint.AcceptMessage(msg));
        }
    }
}
