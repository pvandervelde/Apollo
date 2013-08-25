//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Moq;
using Nuclei.Communication;
using NUnit.Framework;
using Test.Mocks;

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

            DownloadDataFromRemoteEndpoints layer = (id, token, file) =>
                {
                    Assert.Fail();
                    return Task<FileInfo>.Factory.StartNew(() => new FileInfo(file));
                };

            var fileSystem = new Mock<IFileSystem>();

            var endpoint = new EndpointId("a");
            var resolver = new PluginLoadingAssemblyResolver(
                commandContainer.Object,
                layer,
                fileSystem.Object,
                endpoint);
            Assert.IsNull(resolver.LocatePluginAssembly(new object(), new ResolveEventArgs("a")));
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

            DownloadDataFromRemoteEndpoints layer = (id, u, file) => Task<FileInfo>.Factory.StartNew(
                () =>
                {
                    Assert.AreSame(token, u);
                    throw new FileNotFoundException();
                });

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(new Dictionary<string, string>()));
            }

            var endpoint = new EndpointId("a");
            var resolver = new PluginLoadingAssemblyResolver(
                commandContainer.Object,
                layer,
                fileSystem.Object,
                endpoint);
            Assert.IsNull(resolver.LocatePluginAssembly(new object(), new ResolveEventArgs("a")));
        }

        [Test]
        public void LocatePluginAssembly()
        {
            var token = new UploadToken();
            var commands = new Mock<IHostApplicationCommands>();
            {
                commands.Setup(c => c.PreparePluginContainerForTransfer(It.IsAny<AssemblyName>()))
                    .Returns(Task<UploadToken>.Factory.StartNew(() => token));
            }

            var commandContainer = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                commandContainer.Setup(c => c.CommandsFor<IHostApplicationCommands>(It.IsAny<EndpointId>()))
                    .Returns(commands.Object);
            }

            DownloadDataFromRemoteEndpoints layer = (id, u, file) =>
            {
                Assert.AreSame(token, u);
                return Task<FileInfo>.Factory.StartNew(() => new FileInfo(file));
            };

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(new Dictionary<string, string>()));
            }

            var endpoint = new EndpointId("a");
            var resolver = new PluginLoadingAssemblyResolver(
                commandContainer.Object,
                layer,
                fileSystem.Object,
                endpoint);
            Assert.AreEqual(
                Assembly.GetExecutingAssembly(),
                resolver.LocatePluginAssembly(new object(), new ResolveEventArgs(Assembly.GetExecutingAssembly().GetName().FullName)));
        }
    }
}
