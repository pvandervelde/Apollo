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
using MbUnit.Framework;

namespace Apollo.Base.Communication.Messages
{
    [TestFixture]
    [Description("Tests the EndpointInformationResponseMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointInformationResponseMessageTest
    {
        [Test]
        [Description("Checks that a message object can be created.")]
        public void Create()
        {
            var id = new EndpointId("endpoint");
            var response = new MessageId();
            var commands = new List<Type> 
                { 
                    typeof(IHostCommands),
                    typeof(IDatasetApplicationCommands)
                };

            var msg = new EndpointInformationResponseMessage(id, response, commands.ToArray());

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(response, msg.InResponseTo);
            
            Assert.AreEqual(commands[0].AssemblyQualifiedName, msg.Commands[0].AssemblyQualifiedTypeName);
            Assert.AreEqual(commands[1].AssemblyQualifiedName, msg.Commands[1].AssemblyQualifiedTypeName);
        }

        [Test]
        [Description("Checks that the message serialises and deserialises correctly.")]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("endpoint");
            var response = new MessageId();
            var commands = new List<Type> 
                { 
                    typeof(IHostCommands),
                    typeof(IDatasetApplicationCommands)
                };
            var msg = new EndpointInformationResponseMessage(id, response, commands.ToArray());
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(response, otherMsg.InResponseTo);
            Assert.AreEqual(msg.Id, otherMsg.Id);

            Assert.AreElementsEqual(msg.Commands, otherMsg.Commands, (x, y) => x.Equals(y));
        }
    }
}
