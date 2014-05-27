//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Explorer.Nuclei.AppDomains;
using Apollo.Utilities;
using Moq;
using Nuclei;
using NUnit.Framework;

namespace Apollo.Core.Host.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptHostTest
    {
        [Test]
        public void Execute()
        {
            var filePaths = new List<string>();
            var directoryPaths = new List<string>
                {
                    Assembly.GetExecutingAssembly().LocalDirectoryPath()
                };

            var resolutionPaths = AppDomainResolutionPaths.WithFilesAndDirectories(
                Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath),
                filePaths,
                directoryPaths);
            Func<string, AppDomainPaths, AppDomain> builder = (name, paths) => AppDomainBuilder.Assemble(name, resolutionPaths);

            var projects = new Mock<ILinkToProjects>();
            using (var host = new ScriptHost(projects.Object, builder))
            {
                var output = string.Empty;
                using (var writer = new ScriptOutputPipe())
                {
                    writer.OnScriptOutput += (s, e) => output += e.Text;
                    var tuple = host.Execute(ScriptLanguage.IronPython, "print \"hello\"", writer);
            
                    Assert.IsTrue(host.IsExecutingScript);
            
                    tuple.Item1.Wait();
                    Assert.IsFalse(host.IsExecutingScript);
                    Assert.AreEqual("hello" + Environment.NewLine, output);
                }
            }
        }
    }
}
