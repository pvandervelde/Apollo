//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Base;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;
using Utilities.Communication;
using Utilities.FileSystem;
using Utilities.Progress;

namespace Apollo.Core.Dataset
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class PluginLoadingAssemblyResolverTest
    {
        [Test]
        public void LocatePluginAssemblyWithUnknownAssemblyOnHost()
        {
            var commands = new Mock<IHostApplicationCommands>();
            {
                commands.Setup(c => c.PreparePluginContainerForTransfer(It.IsAny<AssemblyName>()))
                    .Returns(
                        Task<UploadToken>.Factory.StartNew(
                            () =>
                            {
                                throw new FileNotFoundException();
                            }));
            }

            var commandContainer = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                commandContainer.Setup(c => c.CommandsFor<IHostApplicationCommands>(It.IsAny<EndpointId>()))
                    .Returns(commands.Object);
            }

            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(
                        l => l.DownloadData(
                            It.IsAny<EndpointId>(),
                            It.IsAny<UploadToken>(),
                            It.IsAny<string>(),
                            It.IsAny<Action<IProgressMark, long>>(),
                            It.IsAny<CancellationToken>(),
                            It.IsAny<TaskScheduler>()))
                    .Verifiable();
            }

            var fileSystem = new Mock<IVirtualizeFileSystems>();

            var endpoint = new EndpointId("a");
            var resolver = new PluginLoadingAssemblyResolver(
                commandContainer.Object,
                layer.Object,
                fileSystem.Object,
                endpoint,
                new CurrentThreadTaskScheduler());
            Assert.IsNull(resolver.LocatePluginAssembly(new object(), new ResolveEventArgs("a")));
            layer.Verify(
                l => l.DownloadData(
                    It.IsAny<EndpointId>(),
                    It.IsAny<UploadToken>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<IProgressMark, long>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TaskScheduler>()),
                Times.Never());
        }

        [Test]
        public void LocatePluginAssemblyDownloadFail()
        {
            var token = new UploadToken();
            var commands = new Mock<IHostApplicationCommands>();
            {
                commands.Setup(c => c.PreparePluginContainerForTransfer(It.IsAny<AssemblyName>()))
                    .Returns(
                        Task<UploadToken>.Factory.StartNew(
                            () =>
                            {
                                return token;
                            }));
            }

            var commandContainer = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                commandContainer.Setup(c => c.CommandsFor<IHostApplicationCommands>(It.IsAny<EndpointId>()))
                    .Returns(commands.Object);
            }

            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(
                        l => l.DownloadData(
                            It.IsAny<EndpointId>(),
                            It.IsAny<UploadToken>(),
                            It.IsAny<string>(),
                            It.IsAny<Action<IProgressMark, long>>(),
                            It.IsAny<CancellationToken>(),
                            It.IsAny<TaskScheduler>()))
                    .Callback<EndpointId, UploadToken, string, Action<IProgressMark, long>, CancellationToken, TaskScheduler>(
                        (e, u, f, p, c, s) => 
                        {
                            Assert.AreSame(token, u);
                        })
                    .Returns(
                        Task<Stream>.Factory.StartNew(
                            () =>
                            {
                                throw new FileNotFoundException();
                            }))
                    .Verifiable();
            }

            var fileSystem = new Mock<IVirtualizeFileSystems>();

            var endpoint = new EndpointId("a");
            var resolver = new PluginLoadingAssemblyResolver(
                commandContainer.Object,
                layer.Object,
                fileSystem.Object,
                endpoint,
                new CurrentThreadTaskScheduler());
            Assert.IsNull(resolver.LocatePluginAssembly(new object(), new ResolveEventArgs("a")));
            layer.Verify(
                l => l.DownloadData(
                    It.IsAny<EndpointId>(),
                    It.IsAny<UploadToken>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<IProgressMark, long>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TaskScheduler>()),
                Times.Once());
        }

        [Test]
        public void LocatePluginAssembly()
        {
            var token = new UploadToken();
            var commands = new Mock<IHostApplicationCommands>();
            {
                commands.Setup(c => c.PreparePluginContainerForTransfer(It.IsAny<AssemblyName>()))
                    .Returns(
                        Task<UploadToken>.Factory.StartNew(
                            () =>
                            {
                                return token;
                            }));
            }

            var commandContainer = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                commandContainer.Setup(c => c.CommandsFor<IHostApplicationCommands>(It.IsAny<EndpointId>()))
                    .Returns(commands.Object);
            }

            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(
                        l => l.DownloadData(
                            It.IsAny<EndpointId>(),
                            It.IsAny<UploadToken>(),
                            It.IsAny<string>(),
                            It.IsAny<Action<IProgressMark, long>>(),
                            It.IsAny<CancellationToken>(),
                            It.IsAny<TaskScheduler>()))
                    .Callback<EndpointId, UploadToken, string, Action<IProgressMark, long>, CancellationToken, TaskScheduler>(
                        (e, u, f, p, c, s) =>
                        {
                            Assert.AreSame(token, u);
                        })
                    .Returns(
                        Task<Stream>.Factory.StartNew(
                            () =>
                            {
                                return new MemoryStream();
                            }))
                    .Verifiable();
            }

            var fileStream = new Mock<IVirtualFileStream>();
            {
                fileStream.Setup(f => f.CopyFrom(It.IsAny<Stream>()))
                    .Verifiable();
            }

            var fileSystem = new Mock<IVirtualizeFileSystems>();
            {
                fileSystem.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>()))
                    .Returns(fileStream.Object);
            }

            var endpoint = new EndpointId("a");
            var resolver = new PluginLoadingAssemblyResolver(
                commandContainer.Object,
                layer.Object,
                fileSystem.Object,
                endpoint,
                new CurrentThreadTaskScheduler());
            Assert.AreEqual(
                Assembly.GetExecutingAssembly(),
                resolver.LocatePluginAssembly(new object(), new ResolveEventArgs(Assembly.GetExecutingAssembly().GetName().FullName)));
            layer.Verify(
                l => l.DownloadData(
                    It.IsAny<EndpointId>(),
                    It.IsAny<UploadToken>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<IProgressMark, long>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TaskScheduler>()),
                Times.Once());
            fileStream.Verify(f => f.CopyFrom(It.IsAny<Stream>()), Times.Once());
        }
    }
}
