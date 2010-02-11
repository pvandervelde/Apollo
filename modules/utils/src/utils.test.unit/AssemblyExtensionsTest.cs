//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MbUnit.Framework;

namespace Apollo.Utils
{
    [TestFixture]
    [Description("Tests the AssemblyExtensions class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class AssemblyExtensionsTest
    {
        private string GetAssemblyPath(Assembly assembly)
        {
            var codebase = assembly.CodeBase;
            var uri = new Uri(codebase);
            return uri.LocalPath;
        }

        [Test]
        [Description("Checks that AssemblyExtensions.LocalFilePath correctly determines the local file path of an assembly.")]
        public void LocalFilePath()
        {
            // Note that this test isn't complete by a long shot. We should really test
            // networked paths too
            // and failures (i.e. dynamically generated code)
            // and ...???
            Assert.AreEqual(GetAssemblyPath(typeof(string).Assembly), typeof(string).Assembly.LocalFilePath());
            Assert.AreEqual(GetAssemblyPath(typeof(FixtureSetUpAttribute).Assembly), typeof(FixtureSetUpAttribute).Assembly.LocalFilePath());
            Assert.AreEqual(GetAssemblyPath(Assembly.GetExecutingAssembly()), Assembly.GetExecutingAssembly().LocalFilePath());
        }
    }
}
