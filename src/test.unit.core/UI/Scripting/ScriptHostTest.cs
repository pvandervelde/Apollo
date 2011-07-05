﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.UserInterfaces.Projects;
using Apollo.UI.Common.Scripting;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptHostTest
    {
        [Test]
        public void Execute()
        {
            var projects = new Mock<ILinkToProjects>();
            Func<string, AppDomainPaths, AppDomain> builder = (s, p) => AppDomain.CurrentDomain;

            var host = new ScriptHost(projects.Object, builder);

            var output = string.Empty;
            var writer = new ScriptOutputPipe();
            writer.OnScriptOutput += (s, e) => output += e.Text;
            var tuple = host.Execute(ScriptLanguage.IronPython, "print \"hello\"", writer);
            
            Assert.IsTrue(host.IsExecutingScript);
            
            tuple.Item1.Wait();
            Assert.IsFalse(host.IsExecutingScript);
            Assert.AreEqual("hello" + Environment.NewLine, output);
        }
    }
}
