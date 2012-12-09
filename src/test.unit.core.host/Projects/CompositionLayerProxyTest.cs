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
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Host.Plugins;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Host.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CompositionLayerProxyTest
    {
        [Test]
        public void Add()
        {
            GroupCompositionId storedId = null;
            GroupDefinition storedDefinition = null;
            var commands = new Mock<ICompositionCommands>();
            {
                commands.Setup(c => c.Add(It.IsAny<GroupCompositionId>(), It.IsAny<GroupDefinition>()))
                    .Callback<GroupCompositionId, GroupDefinition>(
                        (id, def) =>
                        {
                            storedId = id;
                            storedDefinition = def;
                        })
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var connector = new Mock<IConnectGroups>();
            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var definition = new GroupDefinition("Group1");
            var task = layer.Add(definition);
            task.Wait();

            Assert.AreSame(storedId, task.Result);
            Assert.AreSame(definition, storedDefinition);
            Assert.IsTrue(layer.Contains(task.Result));
        }

        [Test]
        public void Remove()
        {
            GroupCompositionId storedId = null;
            GroupDefinition storedDefinition = null;
            var commands = new Mock<ICompositionCommands>();
            {
                commands.Setup(c => c.Add(It.IsAny<GroupCompositionId>(), It.IsAny<GroupDefinition>()))
                    .Callback<GroupCompositionId, GroupDefinition>(
                        (id, def) =>
                        {
                            storedId = id;
                            storedDefinition = def;
                        })
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));

                commands.Setup(c => c.Remove(It.IsAny<GroupCompositionId>()))
                    .Callback<GroupCompositionId>(id => storedId = id)
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var connector = new Mock<IConnectGroups>();
            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var definition = new GroupDefinition("Group1");
            var task = layer.Add(definition);
            task.Wait();

            Assert.AreSame(storedId, task.Result);
            Assert.AreSame(definition, storedDefinition);
            Assert.IsTrue(layer.Contains(task.Result));

            var otherTask = layer.Remove(task.Result);
            otherTask.Wait();

            Assert.AreSame(task.Result, storedId);
            Assert.IsFalse(layer.Contains(task.Result));
        }

        [Test]
        public void RemoveWhileConnected()
        {
            var commands = new Mock<ICompositionCommands>();
            {
                commands.Setup(c => c.Add(It.IsAny<GroupCompositionId>(), It.IsAny<GroupDefinition>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));

                commands.Setup(c => c.Remove(It.IsAny<GroupCompositionId>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));

                commands.Setup(c => c.Connect(It.IsAny<GroupConnection>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var connector = new Mock<IConnectGroups>();
            {
                connector.Setup(
                    c => c.GenerateConnectionFor(
                        It.IsAny<GroupDefinition>(),
                        It.IsAny<GroupImportDefinition>(),
                        It.IsAny<GroupDefinition>()))
                    .Callback<GroupDefinition, GroupImportDefinition, GroupDefinition>(
                        (importingGroup, importDef, exportingGroup) => { })
                    .Returns(Enumerable.Empty<PartImportToPartExportMap>());
            }

            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var exportingDefinition = new GroupDefinition("Group1");
            var addTask = layer.Add(exportingDefinition);
            var exportingId = addTask.Result;

            var importingDefinition = new GroupDefinition("Group2");
            addTask = layer.Add(importingDefinition);
            var importingId = addTask.Result;

            var importDefinition = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var connectTask = layer.Connect(importingId, importDefinition, exportingId);
            connectTask.Wait();

            Assert.IsTrue(layer.IsConnected(importingId, importDefinition));
            Assert.IsTrue(layer.IsConnected(importingId, importDefinition, exportingId));
            Assert.AreEqual(exportingId, layer.ConnectedTo(importingId, importDefinition));

            var removeTask = layer.Remove(exportingId);
            removeTask.Wait();

            Assert.IsTrue(layer.Contains(importingId));
            Assert.IsFalse(layer.Contains(exportingId));
            Assert.IsFalse(layer.IsConnected(importingId, importDefinition));
            Assert.IsFalse(layer.IsConnected(importingId, importDefinition, exportingId));
            Assert.IsNull(layer.ConnectedTo(importingId, importDefinition));
        }

        [Test]
        public void ConnectWithUnknownImportingGroup()
        {
            var commands = new Mock<ICompositionCommands>();
            {
                commands.Setup(c => c.Add(It.IsAny<GroupCompositionId>(), It.IsAny<GroupDefinition>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var connector = new Mock<IConnectGroups>();
            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var exportingGroup = new GroupDefinition("Group1");
            var task = layer.Add(exportingGroup);
            task.Wait();

            var importDefinition = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            Assert.Throws<UnknownPartGroupException>(() => layer.Connect(new GroupCompositionId(), importDefinition, task.Result));
        }

        [Test]
        public void ConnectWithUnknownExportingGroup()
        {
            var commands = new Mock<ICompositionCommands>();
            {
                commands.Setup(c => c.Add(It.IsAny<GroupCompositionId>(), It.IsAny<GroupDefinition>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var connector = new Mock<IConnectGroups>();
            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var exportingGroup = new GroupDefinition("Group1");
            var task = layer.Add(exportingGroup);
            task.Wait();

            var importDefinition = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            Assert.Throws<UnknownPartGroupException>(() => layer.Connect(task.Result, importDefinition, new GroupCompositionId()));
        }

        [Test]
        public void Connect()
        {
            var commands = new Mock<ICompositionCommands>();
            {
                commands.Setup(c => c.Add(It.IsAny<GroupCompositionId>(), It.IsAny<GroupDefinition>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));

                commands.Setup(c => c.Connect(It.IsAny<GroupConnection>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var connector = new Mock<IConnectGroups>();
            {
                connector.Setup(
                    c => c.GenerateConnectionFor(
                        It.IsAny<GroupDefinition>(),
                        It.IsAny<GroupImportDefinition>(),
                        It.IsAny<GroupDefinition>()))
                    .Callback<GroupDefinition, GroupImportDefinition, GroupDefinition>(
                        (importingGroup, importDef, exportingGroup) => { })
                    .Returns(Enumerable.Empty<PartImportToPartExportMap>());
            }

            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var exportingDefinition = new GroupDefinition("Group1");
            var addTask = layer.Add(exportingDefinition);
            var exportingId = addTask.Result;

            var importingDefinition = new GroupDefinition("Group2");
            addTask = layer.Add(importingDefinition);
            var importingId = addTask.Result;

            var importDefinition = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var connectTask = layer.Connect(importingId, importDefinition, exportingId);
            connectTask.Wait();

            Assert.IsTrue(layer.IsConnected(importingId, importDefinition));
            Assert.IsTrue(layer.IsConnected(importingId, importDefinition, exportingId));
            Assert.AreEqual(exportingId, layer.ConnectedTo(importingId, importDefinition));
        }

        [Test]
        public void DisconnectImportFromExport()
        {
            var commands = new Mock<ICompositionCommands>();
            {
                commands.Setup(c => c.Add(It.IsAny<GroupCompositionId>(), It.IsAny<GroupDefinition>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));

                commands.Setup(c => c.Connect(It.IsAny<GroupConnection>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
                
                commands.Setup(c => c.Disconnect(It.IsAny<GroupCompositionId>(), It.IsAny<GroupCompositionId>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var connector = new Mock<IConnectGroups>();
            {
                connector.Setup(
                    c => c.GenerateConnectionFor(
                        It.IsAny<GroupDefinition>(),
                        It.IsAny<GroupImportDefinition>(),
                        It.IsAny<GroupDefinition>()))
                    .Callback<GroupDefinition, GroupImportDefinition, GroupDefinition>(
                        (importingGroup, importDef, exportingGroup) => { })
                    .Returns(Enumerable.Empty<PartImportToPartExportMap>());
            }

            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var exportingDefinition = new GroupDefinition("Group1");
            var addTask = layer.Add(exportingDefinition);
            var exportingId = addTask.Result;

            var importingDefinition = new GroupDefinition("Group2");
            addTask = layer.Add(importingDefinition);
            var importingId = addTask.Result;

            var importDefinition = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var connectTask = layer.Connect(importingId, importDefinition, exportingId);
            connectTask.Wait();

            Assert.IsTrue(layer.IsConnected(importingId, importDefinition));
            Assert.IsTrue(layer.IsConnected(importingId, importDefinition, exportingId));
            Assert.AreEqual(exportingId, layer.ConnectedTo(importingId, importDefinition));

            var disconnectTask = layer.Disconnect(importingId, exportingId);
            disconnectTask.Wait();

            Assert.IsTrue(layer.Contains(importingId));
            Assert.IsTrue(layer.Contains(exportingId));
            Assert.IsFalse(layer.IsConnected(importingId, importDefinition));
            Assert.IsFalse(layer.IsConnected(importingId, importDefinition, exportingId));
            Assert.IsNull(layer.ConnectedTo(importingId, importDefinition));
        }

        [Test]
        public void DisconnectFromAll()
        {
            var commands = new Mock<ICompositionCommands>();
            {
                commands.Setup(c => c.Add(It.IsAny<GroupCompositionId>(), It.IsAny<GroupDefinition>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));

                commands.Setup(c => c.Connect(It.IsAny<GroupConnection>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
                
                commands.Setup(c => c.Disconnect(It.IsAny<GroupCompositionId>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var connector = new Mock<IConnectGroups>();
            {
                connector.Setup(
                    c => c.GenerateConnectionFor(
                        It.IsAny<GroupDefinition>(),
                        It.IsAny<GroupImportDefinition>(),
                        It.IsAny<GroupDefinition>()))
                    .Callback<GroupDefinition, GroupImportDefinition, GroupDefinition>(
                        (importingGroup, importDef, exportingGroup) => { })
                    .Returns(Enumerable.Empty<PartImportToPartExportMap>());
            }

            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var exportingDefinition = new GroupDefinition("Group1");
            var addTask = layer.Add(exportingDefinition);
            var exportingId = addTask.Result;

            var importingDefinition = new GroupDefinition("Group2");
            addTask = layer.Add(importingDefinition);
            var importingId = addTask.Result;

            var importDefinition = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var connectTask = layer.Connect(importingId, importDefinition, exportingId);
            connectTask.Wait();

            Assert.IsTrue(layer.IsConnected(importingId, importDefinition));
            Assert.IsTrue(layer.IsConnected(importingId, importDefinition, exportingId));
            Assert.AreEqual(exportingId, layer.ConnectedTo(importingId, importDefinition));

            var disconnectTask = layer.Disconnect(importingId);
            disconnectTask.Wait();

            Assert.IsTrue(layer.Contains(importingId));
            Assert.IsTrue(layer.Contains(exportingId));
            Assert.IsFalse(layer.IsConnected(importingId, importDefinition));
            Assert.IsFalse(layer.IsConnected(importingId, importDefinition, exportingId));
            Assert.IsNull(layer.ConnectedTo(importingId, importDefinition));
        }

        [Test]
        public void IsConnectedWithUnknowns()
        {
            var commands = new Mock<ICompositionCommands>();
            var connector = new Mock<IConnectGroups>();
            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var importDefinition = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            Assert.IsFalse(layer.IsConnected(null, importDefinition));
            Assert.IsFalse(layer.IsConnected(new GroupCompositionId(), null));
            Assert.IsFalse(layer.IsConnected(new GroupCompositionId(), importDefinition));
        }

        [Test]
        public void IsConnectedWithExportWithUnknowns()
        {
            var commands = new Mock<ICompositionCommands>();
            var connector = new Mock<IConnectGroups>();
            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var importDefinition = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            Assert.IsFalse(layer.IsConnected(null, importDefinition, new GroupCompositionId()));
            Assert.IsFalse(layer.IsConnected(new GroupCompositionId(), null, new GroupCompositionId()));
            Assert.IsFalse(layer.IsConnected(new GroupCompositionId(), importDefinition, null));
            Assert.IsFalse(layer.IsConnected(new GroupCompositionId(), importDefinition, new GroupCompositionId()));
        }

        [Test]
        public void ConnectedToWithUnknownImportingGroup()
        {
            var commands = new Mock<ICompositionCommands>();
            var connector = new Mock<IConnectGroups>();
            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);

            var importDefinition = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            Assert.IsNull(layer.ConnectedTo(null, importDefinition));
            Assert.IsNull(layer.ConnectedTo(new GroupCompositionId(), null));
            Assert.IsNull(layer.ConnectedTo(new GroupCompositionId(), importDefinition));
        }

        [Test]
        public void ReloadFromDataset()
        {
            var importingId = new GroupCompositionId();
            var exportingId = new GroupCompositionId();

            var importingGroup = new GroupDefinition("Group1");
            var exportingGroup = new GroupDefinition("Group2");

            var importDefinition = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            var state = new GroupCompositionState(
                new List<Tuple<GroupCompositionId, GroupDefinition>> 
                    {
                        new Tuple<GroupCompositionId, GroupDefinition>(importingId, importingGroup),
                        new Tuple<GroupCompositionId, GroupDefinition>(exportingId, exportingGroup),
                    },
                new List<Tuple<GroupCompositionId, GroupImportDefinition, GroupCompositionId>> 
                    { 
                        new Tuple<GroupCompositionId, GroupImportDefinition, GroupCompositionId>(importingId, importDefinition, exportingId)
                    });

            var commands = new Mock<ICompositionCommands>();
            {
                commands.Setup(c => c.CurrentState())
                    .Returns(
                        Task<GroupCompositionState>.Factory.StartNew(
                            () => state,
                            new CancellationTokenSource().Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var connector = new Mock<IConnectGroups>();
            var layer = new ProxyCompositionLayer(commands.Object, connector.Object);
            var task = layer.ReloadFromDataset();
            task.Wait();

            Assert.IsTrue(layer.Contains(importingId));
            Assert.AreEqual(importingGroup, layer.Group(importingId));

            Assert.IsTrue(layer.Contains(exportingId));
            Assert.AreEqual(exportingGroup, layer.Group(exportingId));

            Assert.IsTrue(layer.IsConnected(importingId, importDefinition));
            Assert.IsTrue(layer.IsConnected(importingId, importDefinition, exportingId));
            Assert.AreEqual(exportingId, layer.ConnectedTo(importingId, importDefinition));
        }
    }
}
