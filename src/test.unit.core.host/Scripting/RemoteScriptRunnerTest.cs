//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Scripting.Projects;
using IronPython.Hosting;
using Moq;
using NUnit.Framework;

namespace Apollo.Core.Host.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteScriptRunnerTest
    {
        [Test]
        public void ExecuteWithoutOutput()
        {
            var projects = new Mock<ILinkScriptsToProjects>();
            {
                projects.Setup(p => p.NewProject())
                    .Verifiable();
            }

            var writer = new ScriptOutputPipe();

            bool hasOutput = false;
            writer.OnScriptOutput += (s, e) => hasOutput = true;

            var engine = Python.CreateEngine();
            var runner = new RemoteScriptRunner(projects.Object, writer, engine);

            var script = "projects.NewProject()";
            runner.Execute(script, new CancelScriptToken());

            Assert.IsFalse(hasOutput);
            projects.Verify(p => p.NewProject(), Times.Once());
        }

        [Test]
        public void ExecuteWithOutput()
        {
            var projects = new Mock<ILinkScriptsToProjects>();
            var writer = new ScriptOutputPipe();

            var output = string.Empty;
            writer.OnScriptOutput += (s, e) => output += e.Text;

            var engine = Python.CreateEngine();
            var runner = new RemoteScriptRunner(projects.Object, writer, engine);

            var script = "print \"Hello\"";
            runner.Execute(script, new CancelScriptToken());

            Assert.AreEqual("Hello" + Environment.NewLine, output);
        }

        [Test]
        public void ExecuteWithExecutionFailure()
        {
            var projects = new Mock<ILinkScriptsToProjects>();
            var writer = new ScriptOutputPipe();

            bool hasOutput = false;
            writer.OnScriptOutput += (s, e) => hasOutput = true;

            var engine = Python.CreateEngine();
            var runner = new RemoteScriptRunner(projects.Object, writer, engine);

            var script = "foobar()";
            Assert.Throws<ScriptExecutionFailureException>(() => runner.Execute(script, new CancelScriptToken()));
            Assert.IsFalse(hasOutput);
        }

        [Test]
        public void VerifySyntaxWithoutSyntaxErrors()
        {
            var projects = new Mock<ILinkScriptsToProjects>();
            var writer = new ScriptOutputPipe();

            writer.OnScriptOutput += (s, e) => { };

            var engine = Python.CreateEngine();
            var runner = new RemoteScriptRunner(projects.Object, writer, engine);

            var script = "print \"Hello\"";
            var errors = runner.VerifySyntax(script);
            Assert.AreEqual(0, errors.Count());
        }

        [Test]
        public void VerifySyntaxWithSingleSyntaxError()
        {
            var projects = new Mock<ILinkScriptsToProjects>();
            var writer = new ScriptOutputPipe();

            writer.OnScriptOutput += (s, e) => { };

            var engine = Python.CreateEngine();
            var runner = new RemoteScriptRunner(projects.Object, writer, engine);

            var script = "prin 10";
            var errors = runner.VerifySyntax(script);
            Assert.AreEqual(1, errors.Count());

            var error = errors.First();
            Assert.AreEqual(1, error.Line);
            Assert.AreEqual(6, error.Column);
            Assert.AreEqual(SyntaxVerificationSeverity.Error, error.Severity);
        }

        [Test]
        public void VerifySyntaxWithMultipleSyntaxErrors()
        {
            var projects = new Mock<ILinkScriptsToProjects>();
            var writer = new ScriptOutputPipe();

            writer.OnScriptOutput += (s, e) => { };

            var engine = Python.CreateEngine();
            var runner = new RemoteScriptRunner(projects.Object, writer, engine);

            var script = "prin 10" + Environment.NewLine + "prin 20";
            var errors = runner.VerifySyntax(script);
            Assert.AreEqual(2, errors.Count());

            var error = errors.First();
            Assert.AreEqual(1, error.Line);
            Assert.AreEqual(6, error.Column);
            Assert.AreEqual(SyntaxVerificationSeverity.Error, error.Severity);

            error = errors.Last();
            Assert.AreEqual(2, error.Line);
            Assert.AreEqual(6, error.Column);
            Assert.AreEqual(SyntaxVerificationSeverity.Error, error.Severity);
        }
    }
}
