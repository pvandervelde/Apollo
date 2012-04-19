﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Utilities;
using MbUnit.Framework;

namespace Apollo.Core.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ReceivingEndpointTest
    {
        [Test]
        public void AcceptMessage()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);

            var endpoint = new ReceivingEndpoint(systemDiagnostics);
            endpoint.OnNewMessage += (s, e) => Assert.AreSame(msg, e.Message);

            endpoint.AcceptMessage(msg);
        }

        [Test]
        public void AcceptMessageThrowingException()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);

            var endpoint = new ReceivingEndpoint(systemDiagnostics);
            endpoint.OnNewMessage += 
                (s, e) => 
                { 
                    throw new Exception(); 
                };

            Assert.DoesNotThrow(() => endpoint.AcceptMessage(msg));
        }
    }
}
