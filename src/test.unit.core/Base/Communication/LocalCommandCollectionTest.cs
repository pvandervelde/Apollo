//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the ManualDiscoverySource class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class LocalCommandCollectionTest
    {
        public interface IMockCommandSetWithTaskReturn : ICommandSet
        {
            Task MyMethod(int input);
        }

        [Test]
        [Description("Checks that a command set can be registered.")]
        public void Register()
        {
            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("mine"));
                layer.Setup(l => l.IsSignedIn)
                    .Returns(true);
                layer.Setup(l => l.LocalConnectionPoints())
                    .Returns(
                        new List<ChannelConnectionInformation> 
                            { 
                                new ChannelConnectionInformation(
                                    new EndpointId("other"), 
                                    typeof(NamedPipeChannelType), 
                                    new Uri("net.pipe://localhost/apollo_test")) 
                            });
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
            }

            var collection = new LocalCommandCollection(layer.Object);

            var commands = new Mock<IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object);

            Assert.IsTrue(collection.Exists(pair => pair.Key.Equals(typeof(IMockCommandSetWithTaskReturn))));
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }

        [Test]
        [Description("Checks that a command set can only be registered once.")]
        public void RegisterWithExistingType()
        {
            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("mine"));
                layer.Setup(l => l.IsSignedIn)
                    .Returns(true);
                layer.Setup(l => l.LocalConnectionPoints())
                    .Returns(
                        new List<ChannelConnectionInformation> 
                            { 
                                new ChannelConnectionInformation(
                                    new EndpointId("other"), 
                                    typeof(NamedPipeChannelType), 
                                    new Uri("net.pipe://localhost/apollo_test")) 
                            });
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
            }

            var collection = new LocalCommandCollection(layer.Object);
            
            var commands = new Mock<IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object);
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());

            Assert.Throws<CommandAlreadyRegisteredException>(() => collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object));
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }

        [Test]
        [Description("Checks that asking for an unknown command set returns a null reference.")]
        public void CommandsForWithUnknownType()
        {
            var layer = new Mock<ICommunicationLayer>();
            var collection = new LocalCommandCollection(layer.Object);
            Assert.IsNull(collection.CommandsFor(typeof(IMockCommandSetWithTaskReturn)));
        }

        [Test]
        [Description("Checks that asking for a command set the command set object.")]
        public void CommandsFor()
        {
            var layer = new Mock<ICommunicationLayer>();
            var collection = new LocalCommandCollection(layer.Object);

            var commands = new Mock<IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object);

            var commandSet = collection.CommandsFor(typeof(IMockCommandSetWithTaskReturn));
            Assert.AreSame(commands.Object, commandSet);
        }
    }
}
