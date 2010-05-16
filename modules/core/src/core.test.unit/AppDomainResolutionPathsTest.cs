//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the ApplicationShutdownCapabilityResponseMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class AppDomainResolutionPathsTest
    {
        [Test]
        [Description("Checks that an object cannot be created with a null base path string.")]
        public void WithFilesWithNullBasePath()
        {
            Assert.Throws<ArgumentNullException>(() => AppDomainResolutionPaths.WithFiles(null, new List<string>()));
        }

        [Test]
        [Description("Checks that an object cannot be created with a empty base path string.")]
        public void WithFilesWithEmptyBasePath()
        {
            Assert.Throws<ArgumentException>(() => AppDomainResolutionPaths.WithFiles(string.Empty, new List<string>()));
        }

        [Test]
        [Description("Checks that an object can be created.")]
        public void WithFiles()
        {
            var collection = new List<string> { "files" };
            var basePath = "basePath";

            var paths = AppDomainResolutionPaths.WithFiles(basePath, collection);
            Assert.AreEqual(basePath, paths.BasePath);
            Assert.AreElementsEqual(collection, paths.Files);
            Assert.IsEmpty(paths.Directories);
        }

        [Test]
        [Description("Checks that an object cannot be created with a null base path string.")]
        public void WithFilesAndDirectoriesWithNullBasePath()
        {
            Assert.Throws<ArgumentNullException>(() => AppDomainResolutionPaths.WithFilesAndDirectories(null, new List<string>(), new List<string>()));
        }

        [Test]
        [Description("Checks that an object cannot be created with a empty base path string.")]
        public void WithFilesAndDirectoriesWithEmptyBasePath()
        {
            Assert.Throws<ArgumentException>(() => AppDomainResolutionPaths.WithFilesAndDirectories(string.Empty, new List<string>(), new List<string>()));
        }

        [Test]
        [Description("Checks that an object can be created.")]
        public void WithFilesAndDirectories()
        {
            var fileCollection = new List<string> { "files" };
            var directoryCollection = new List<string> { "directories" };
            var basePath = "basePath";

            var paths = AppDomainResolutionPaths.WithFilesAndDirectories(basePath, fileCollection, directoryCollection);
            Assert.AreEqual(basePath, paths.BasePath);
            Assert.AreElementsEqual(fileCollection, paths.Files);
            Assert.AreElementsEqual(directoryCollection, paths.Directories);
        }
    }
}
