//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Plugins;
using MbUnit.Framework;

namespace Apollo.Core.Base.Communication.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointProxyTypesResponseMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("endpoint");
            var response = new MessageId();
            var commands = new List<Type> 
                { 
                    typeof(ICompositionCommands),
                    typeof(IDatasetApplicationCommands)
                };

            var msg = new EndpointProxyTypesResponseMessage(id, response, commands.ToArray());

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(response, msg.InResponseTo);
            
            Assert.AreEqual(commands[0].AssemblyQualifiedName, msg.ProxyTypes[0].AssemblyQualifiedTypeName);
            Assert.AreEqual(commands[1].AssemblyQualifiedName, msg.ProxyTypes[1].AssemblyQualifiedTypeName);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("endpoint");
            var response = new MessageId();
            var commands = new List<Type> 
                { 
                    typeof(ICompositionCommands),
                    typeof(IDatasetApplicationCommands)
                };
            var msg = new EndpointProxyTypesResponseMessage(id, response, commands.ToArray());
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(response, otherMsg.InResponseTo);
            Assert.AreEqual(msg.Id, otherMsg.Id);

            Assert.AreElementsEqual(msg.ProxyTypes, otherMsg.ProxyTypes, (x, y) => x.Equals(y));
        }
    }
}
