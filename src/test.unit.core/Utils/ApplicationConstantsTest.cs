//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Apollo.Utilities;
using MbUnit.Framework;

namespace Apollo.Core.Utilities
{
    [TestFixture]
    [Description("Tests the FusionHelper class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ApplicationConstantsTest
    {
        [Test]
        [Description("Checks that the CompanyName property returns the correct value.")]
        public void CompanyName()
        {
            var assembly = Assembly.GetEntryAssembly();
            var attribute = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0] as AssemblyCompanyAttribute;

            var constants = new ApplicationConstants();
            Assert.AreEqual(attribute.Company, constants.CompanyName);
        }

        [Test]
        [Description("Checks that the ApplicationName property returns the correct value.")]
        public void ApplicationName()
        {
            var assembly = Assembly.GetEntryAssembly();
            var attribute = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0] as AssemblyProductAttribute;

            var constants = new ApplicationConstants();
            Assert.AreEqual(attribute.Product, constants.ApplicationName);
        }

        [Test]
        [Description("Checks that the ApplicationVersion property returns the correct value.")]
        public void ApplicationVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            var version = assembly.GetName().Version;

            var constants = new ApplicationConstants();
            Assert.AreEqual(version, constants.ApplicationVersion);
        }

        [Test]
        [Description("Checks that the ApplicationCompatibilityVersion property returns the correct value.")]
        public void ApplicationCompatibilityVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            var version = assembly.GetName().Version;

            var constants = new ApplicationConstants();
            Assert.AreEqual(new Version(version.Major, version.Minor), constants.ApplicationCompatibilityVersion);
        }
    }
}
