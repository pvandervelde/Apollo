//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.Utilities;
using MbUnit.Framework;

namespace Apollo.Core.Utilities
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

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), applicationConstants.CompanyName);
            path = Path.Combine(path, applicationConstants.ApplicationName);
            path = Path.Combine(path, applicationConstants.ApplicationCompatibilityVersion.ToString(2));

            Assert.AreEqual(path, constants.ProductSettingsPath());
        }

        [Test]
        public void LogPath()
        {
            var applicationConstants = new ApplicationConstants();
            var constants = new FileConstants(applicationConstants);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), applicationConstants.CompanyName);
            path = Path.Combine(path, applicationConstants.ApplicationName);
            path = Path.Combine(path, applicationConstants.ApplicationCompatibilityVersion.ToString(2));
            path = Path.Combine(path, "logs");

            Assert.AreEqual(path, constants.LogPath());
        }
    }
}
