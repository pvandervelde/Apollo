//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;

namespace Apollo.Utilities
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class FileConstantsTest
    {
        [Test]
        public void ProductSettingsPath()
        {
            var applicationConstants = new ApplicationConstants();
            var constants = new FileConstants(applicationConstants);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationConstants.CompanyName);
            path = Path.Combine(path, ApplicationConstants.ApplicationName);
            path = Path.Combine(path, ApplicationConstants.ApplicationCompatibilityVersion.ToString(2));

            Assert.AreEqual(path, FileConstants.ProductSettingsUserPath());
        }

        [Test]
        public void LogPath()
        {
            var applicationConstants = new ApplicationConstants();
            var constants = new FileConstants(applicationConstants);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationConstants.CompanyName);
            path = Path.Combine(path, ApplicationConstants.ApplicationName);
            path = Path.Combine(path, ApplicationConstants.ApplicationCompatibilityVersion.ToString(2));
            path = Path.Combine(path, "logs");

            Assert.AreEqual(path, FileConstants.LogPath());
        }
    }
}
