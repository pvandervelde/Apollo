//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MbUnit.Framework;
using Moq;
using Utilities;
using Utilities.Communication;
using Utilities.Configuration;
using Utilities.FileSystem;

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
            var fileSystem = new Mock<IVirtualizeFileSystems>();
            var uploads = new Mock<IStoreUploads>();
            var configuration = new Mock<IConfiguration>();

            var commands = new HostApplicationCommands(fileSystem.Object, uploads.Object, configuration.Object);
            var task = commands.PreparePluginContainerForTransfer(typeof(string).Assembly.GetName());

            Assert.Throws<AggregateException>(() => task.Wait());
        }

        [Test]
        public void PreparePluginContainerForTransferWithUknownName()
        {
            var fileSystem = new Mock<IVirtualizeFileSystems>();
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
            var fileSystem = new Mock<IVirtualizeFileSystems>();
            {
                fileSystem.Setup(f => f.GetFilesInDirectory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Returns(() => files);
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
            var index = -1;
            var files = new List<List<string>>
                {
                    new List<string> { typeof(string).Assembly.LocalFilePath() },
                    new List<string> { typeof(HostApplicationCommands).Assembly.LocalFilePath() },
                    new List<string> { Assembly.GetExecutingAssembly().LocalFilePath() }
                };
            var fileSystem = new Mock<IVirtualizeFileSystems>();
            {
                fileSystem.Setup(f => f.GetFilesInDirectory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Returns<string, string, bool>(
                        (path, filter, flag) => 
                        {
                            index++;
                            return files[index];
                        });
            }

            var uploads = new Mock<IStoreUploads>();
            {
                uploads.Setup(u => u.Register(It.IsAny<string>()))
                    .Callback<string>(
                        f =>
                        {
                            Assert.AreEqual(files[1][0], f);
                        })
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
