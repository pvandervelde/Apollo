﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Apollo.Internals;
using NUnit.Framework;

namespace Apollo.Utilities
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ApplicationConstantsTest
    {
        private static Assembly GetAssembly()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                // Either we're being called from unmanaged code
                // or we're in a different appdomain than the actual executable
                assembly = typeof(ApplicationConstants).Assembly;
            }

            return assembly;
        }

        [Test]
        public void CompanyName()
        {
            Assert.AreEqual(CompanyInformation.CompanyName, ApplicationConstants.CompanyName);
        }

        [Test]
        public void ApplicationName()
        {
            Assert.AreEqual(ProductInformation.ProductName, ApplicationConstants.ApplicationName);
        }

        [Test]
        public void ApplicationVersion()
        {
            var assembly = GetAssembly();
            var version = assembly.GetName().Version;

            Assert.AreEqual(version, ApplicationConstants.ApplicationVersion);
        }

        [Test]
        public void ApplicationCompatibilityVersion()
        {
            var assembly = GetAssembly();
            var version = assembly.GetName().Version;

            Assert.AreEqual(new Version(version.Major, version.Minor), ApplicationConstants.ApplicationCompatibilityVersion);
        }
    }
}
