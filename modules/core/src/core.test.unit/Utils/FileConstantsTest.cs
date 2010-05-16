//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using MbUnit.Framework;

namespace Apollo.Core.Utils
{
    [TestFixture]
    [Description("Tests the FusionHelper class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class FileConstantsTest
    {
        [Test]
        [Description("Checks that an object cannot be created without an ApplicationConstants object.")]
        public void CreateWithNullApplicationConstants()
        {
            Assert.Throws<ArgumentNullException>(() => new FileConstants(null));
        }

        [Test]
        [Description("Checks that the ProductSettingsPath method returns the correct path.")]
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
        [Description("Checks that the LogPath method returns the correct path.")]
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
