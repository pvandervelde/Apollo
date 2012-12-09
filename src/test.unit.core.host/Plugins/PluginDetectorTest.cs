//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Host.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class PluginDetectorTest
    {
        private sealed class MockScanner : IAssemblyScanner
        {
            private IEnumerable<string> m_Files;

            public void Scan(IEnumerable<string> assemblyFilesToScan)
            {
                m_Files = assemblyFilesToScan;
            }

            public IEnumerable<string> FilesToScan
            {
                get
                {
                    return m_Files;
                }
            }
        }

        [Test]
        public void SearchWithNewFilesOnly()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginFiles())
                    .Returns(Enumerable.Empty<PluginFileInfo>());
            }

            var files = new List<string> 
                        { 
                            @"c:\temp\foobar.dll",
                            @"c:\temp\foobar2.dll"
                        };
            var fileSystem = new Mock<IVirtualizeFileSystems>();
            {
                fileSystem.Setup(f => f.DoesFileExist(It.IsAny<string>()))
                    .Returns(true);
                fileSystem.Setup(f => f.FileLastWriteTimeUtc(It.IsAny<string>()))
                    .Returns(DateTimeOffset.Now.AddHours(-1));
                fileSystem.Setup(f => f.GetFilesInDirectory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Returns(files);
            }

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var detector = new PluginDetector(
                repository.Object,
                scannerBuilder,
                fileSystem.Object,
                new SystemDiagnostics((p, s) => { }, null));

            detector.SearchDirectory(@"c:\temp");
            Assert.AreElementsEqualIgnoringOrder(files, scanner.FilesToScan);
        }

        [Test]
        public void SearchWithKnownFilesOnly()
        {
            var files = new List<string> 
                        { 
                            @"c:\temp\foobar.dll",
                            @"c:\temp\foobar2.dll"
                        };

            var pluginFiles = new List<PluginFileInfo> 
                {
                    new PluginFileInfo(files[0], DateTimeOffset.Now.AddHours(-2)),
                    new PluginFileInfo(files[1], DateTimeOffset.Now.AddHours(-2)),
                };
            
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginFiles())
                    .Returns(pluginFiles);
            }

            var fileSystem = new Mock<IVirtualizeFileSystems>();
            {
                fileSystem.Setup(f => f.DoesFileExist(It.IsAny<string>()))
                    .Returns(true);
                fileSystem.Setup(f => f.FileLastWriteTimeUtc(It.IsAny<string>()))
                    .Returns(DateTimeOffset.Now.AddHours(-1));
                fileSystem.Setup(f => f.GetFilesInDirectory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Returns(files);
            }

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var detector = new PluginDetector(
                repository.Object,
                scannerBuilder,
                fileSystem.Object,
                new SystemDiagnostics((p, s) => { }, null));

            detector.SearchDirectory(@"c:\temp");
            Assert.AreElementsEqualIgnoringOrder(files, scanner.FilesToScan);
        }

        [Test]
        public void SearchWithUpdatedFilesOnly()
        {
            var files = new List<string> 
                        { 
                            @"c:\temp\foobar.dll",
                            @"c:\temp\foobar2.dll"
                        };

            var pluginFiles = new List<PluginFileInfo> 
                {
                    new PluginFileInfo(files[0], DateTimeOffset.Now),
                    new PluginFileInfo(files[1], DateTimeOffset.Now.AddHours(-2)),
                };

            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginFiles())
                    .Returns(pluginFiles);
            }

            var fileSystem = new Mock<IVirtualizeFileSystems>();
            {
                fileSystem.Setup(f => f.DoesFileExist(It.IsAny<string>()))
                    .Returns(true);
                fileSystem.Setup(f => f.FileLastWriteTimeUtc(It.IsAny<string>()))
                    .Returns(DateTimeOffset.Now.AddHours(-1));
                fileSystem.Setup(f => f.GetFilesInDirectory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Returns(files);
            }

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var detector = new PluginDetector(
                repository.Object,
                scannerBuilder,
                fileSystem.Object,
                new SystemDiagnostics((p, s) => { }, null));

            detector.SearchDirectory(@"c:\temp");
            Assert.AreElementsEqualIgnoringOrder(new List<string> { files[1] }, scanner.FilesToScan);
        }

        [Test]
        public void SearchWithDeletedFilesOnly()
        {
            var pluginFiles = new List<PluginFileInfo> 
                {
                    new PluginFileInfo(@"c:\temp\foobar.dll", DateTimeOffset.Now),
                    new PluginFileInfo(@"c:\temp\foobar2.dll", DateTimeOffset.Now.AddHours(-2)),
                };

            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginFiles())
                    .Returns(pluginFiles);
            }

            var fileSystem = new Mock<IVirtualizeFileSystems>();
            {
                fileSystem.Setup(f => f.DoesFileExist(It.IsAny<string>()))
                    .Returns(false);
                fileSystem.Setup(f => f.FileLastWriteTimeUtc(It.IsAny<string>()))
                    .Returns(DateTimeOffset.Now.AddHours(-1));
                fileSystem.Setup(f => f.GetFilesInDirectory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Returns(new List<string>());
            }

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var detector = new PluginDetector(
                repository.Object,
                scannerBuilder,
                fileSystem.Object,
                new SystemDiagnostics((p, s) => { }, null));

            detector.SearchDirectory(@"c:\temp");
            Assert.IsNull(scanner.FilesToScan);
        }
    }
}
