//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Base.Communication;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Loaders;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetOnlineInformationTest
    {
        [Test]
        public void Create()
        {
            var hub = new Mock<ISendCommandsToRemoteEndpoints>();
            var id = new DatasetId();
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var networkId = NetworkIdentifier.ForLocalMachine();

            var info = new DatasetOnlineInformation(id, endpoint, networkId, hub.Object);
            Assert.AreSame(id, info.Id);
            Assert.AreSame(endpoint, info.Endpoint);
            Assert.AreSame(networkId, info.RunsOn);
        }

        [Test]
        public void AvailableCommands()
        {
            var id = new DatasetId();
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var networkId = NetworkIdentifier.ForLocalMachine();

            var commandList = new Dictionary<Type, ICommandSet> 
                {
                    { 
                        typeof(CommandProxyBuilderTest.IMockCommandSetWithTaskReturn),
                        new Mock<CommandProxyBuilderTest.IMockCommandSetWithTaskReturn>().Object
                    },
                    {
                        typeof(CommandProxyBuilderTest.IMockCommandSetWithTypedTaskReturn),
                        new Mock<CommandProxyBuilderTest.IMockCommandSetWithTaskReturn>().Object
                    },
                    {
                        typeof(CommandProxyBuilderTest.IMockCommandSetForInternalUse),
                        new Mock<CommandProxyBuilderTest.IMockCommandSetWithTaskReturn>().Object
                    },
                };
            var hub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                hub.Setup(h => h.AvailableCommandsFor(It.IsAny<EndpointId>()))
                    .Returns(commandList.Keys);
                hub.Setup(h => h.CommandsFor(It.IsAny<EndpointId>(), It.IsAny<Type>()))
                    .Returns<EndpointId, Type>((e, t) => commandList[t]);
            }

            var info = new DatasetOnlineInformation(id, endpoint, networkId, hub.Object);
            var commands = info.AvailableCommands();

            Assert.AreEqual(2, commands.Count());
        }

        [Test]
        public void Command()
        {
            var id = new DatasetId();
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var networkId = NetworkIdentifier.ForLocalMachine();

            var commandList = new SortedList<Type, ICommandSet> 
                {
                    { 
                        typeof(CommandProxyBuilderTest.IMockCommandSetWithTaskReturn),
                        new Mock<CommandProxyBuilderTest.IMockCommandSetWithTaskReturn>().Object
                    },
                };
            var hub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                hub.Setup(h => h.CommandsFor<CommandProxyBuilderTest.IMockCommandSetWithTaskReturn>(It.IsAny<EndpointId>()))
                    .Returns((CommandProxyBuilderTest.IMockCommandSetWithTaskReturn)commandList.Values[0]);
            }

            var info = new DatasetOnlineInformation(id, endpoint, networkId, hub.Object);
            var commands = info.Command<CommandProxyBuilderTest.IMockCommandSetWithTaskReturn>();
            Assert.AreSame(commandList.Values[0], commands);
        }

        [Test]
        public void Close()
        {
            var id = new DatasetId();
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var networkId = NetworkIdentifier.ForLocalMachine();

            var datasetCommands = new Mock<IDatasetApplicationCommands>();
            {
                datasetCommands.Setup(d => d.Close())
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationToken(),
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var hub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                hub.Setup(h => h.HasCommandFor(It.IsAny<EndpointId>(), It.IsAny<Type>()))
                    .Returns(true);
                hub.Setup(h => h.CommandsFor<IDatasetApplicationCommands>(It.IsAny<EndpointId>()))
                    .Returns(datasetCommands.Object);
            }

            var info = new DatasetOnlineInformation(id, endpoint, networkId, hub.Object);
            info.Close();

            datasetCommands.Verify(d => d.Close(), Times.Once());
        }
    }
}
