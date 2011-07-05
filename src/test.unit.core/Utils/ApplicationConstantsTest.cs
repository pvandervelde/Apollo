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
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ApplicationConstantsTest
    {
        [Test]
        public void CompanyName()
        {
            var assembly = Assembly.GetEntryAssembly();
            var attribute = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0] as AssemblyCompanyAttribute;

            var constants = new ApplicationConstants();
            Assert.AreEqual(attribute.Company, constants.CompanyName);
        }

        [Test]
        public void ApplicationName()
        {
            var assembly = Assembly.GetEntryAssembly();
            var attribute = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0] as AssemblyProductAttribute;

            var constants = new ApplicationConstants();
            Assert.AreEqual(attribute.Product, constants.ApplicationName);
        }

        [Test]
        public void ApplicationVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            var version = assembly.GetName().Version;

            var constants = new ApplicationConstants();
            Assert.AreEqual(version, constants.ApplicationVersion);
        }

        [Test]
        public void ApplicationCompatibilityVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            var version = assembly.GetName().Version;

            var constants = new ApplicationConstants();
            Assert.AreEqual(new Version(version.Major, version.Minor), constants.ApplicationCompatibilityVersion);
        }
    }
}
