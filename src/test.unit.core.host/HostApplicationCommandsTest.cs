//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Reflection;
using MbUnit.Framework;
using Moq;
using Nuclei;
using Nuclei.Communication;
using Nuclei.Configuration;
using Test.Mocks;

namespace Apollo.Core.Host
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class HostApplicationCommandsTest
    {
        [Test]
        public void PreparePluginContainerForTransferWithoutConfiguration()
        {
            var fileSystem = new Mock<IFileSystem>();
            var uploads = new Mock<IStoreUploads>();
            var configuration = new Mock<IConfiguration>();

            var commands = new HostApplicationCommands(fileSystem.Object, uploads.Object, configuration.Object);
            var task = commands.PreparePluginContainerForTransfer(typeof(string).Assembly.GetName());

            Assert.Throws<AggregateException>(() => task.Wait());
        }

        [Test]
        public void PreparePluginContainerForTransferWithUknownName()
        {
            var fileSystem = new Mock<IFileSystem>();
            var uploads = new Mock<IStoreUploads>();
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<List<string>>(It.IsAny<ConfigurationKey>()))
                    .Returns(new List<string> { "bla" });
            }

            var commands = new HostApplicationCommands(fileSystem.Object, uploads.Object, configuration.Object);
            var task = commands.PreparePluginContainerForTransfer(typeof(string).Assembly.GetName());

            Assert.Throws<AggregateException>(() => task.Wait());
        }

        [Test]
        public void PreparePluginContainerForTransfer()
        {
            var files = new List<string>
                {
                    typeof(string).Assembly.LocalFilePath(),
                    typeof(HostApplicationCommands).Assembly.LocalFilePath(),
                    Assembly.GetExecutingAssembly().LocalFilePath()
                };
            var mockDirectory = new MockDirectory(files);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(() => mockDirectory);
            }

            var uploads = new Mock<IStoreUploads>();
            {
                uploads.Setup(u => u.Register(It.IsAny<string>()))
                    .Callback<string>(
                        f =>
                        {
                            Assert.AreEqual(files[1], f);
                        })
                    .Returns(new UploadToken())
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<List<string>>(It.IsAny<ConfigurationKey>()))
                    .Returns(new List<string> { "bla" });
            }

            var commands = new HostApplicationCommands(fileSystem.Object, uploads.Object, configuration.Object);
            var task = commands.PreparePluginContainerForTransfer(typeof(HostApplicationCommands).Assembly.GetName());
            task.Wait();

            uploads.Verify(u => u.Register(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void PreparePluginContainerForTransferWithMultipleDirectories()
        {
            var files = new List<string>
                {
                    typeof(string).Assembly.LocalFilePath(),
                    typeof(HostApplicationCommands).Assembly.LocalFilePath(),
                    Assembly.GetExecutingAssembly().LocalFilePath()
                };
            var mockDirectory = new MockDirectory(files);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(() => mockDirectory);
            }

            var uploads = new Mock<IStoreUploads>();
            {
                uploads.Setup(u => u.Register(It.IsAny<string>()))
                    .Callback<string>(f => Assert.AreEqual(files[1], f))
                    .Returns(new UploadToken())
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<List<string>>(It.IsAny<ConfigurationKey>()))
                    .Returns(new List<string> { "bla1", "bla2", "bla3" });
            }

            var commands = new HostApplicationCommands(fileSystem.Object, uploads.Object, configuration.Object);
            var task = commands.PreparePluginContainerForTransfer(typeof(HostApplicationCommands).Assembly.GetName());
            task.Wait();

            uploads.Verify(u => u.Register(It.IsAny<string>()), Times.Once());
        }
    }
}
