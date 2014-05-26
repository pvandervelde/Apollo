//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Host.Scripting;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class RunScriptCommandTest
    {
        [Test]
        public void CanCancelScriptRunWithNullHost()
        {
            var command = new RunScriptCommand(null);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanCancelScriptRunWithNonExecutingHost()
        {
            var scriptHost = new Mock<IHostScripts>();
            {
                scriptHost.Setup(s => s.IsExecutingScript)
                    .Returns(true);
            }

            var command = new RunScriptCommand(scriptHost.Object);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CancelScriptRun()
        {
            using (var task = new Task(() => { }))
            {
                using (var source = new CancellationTokenSource())
                {
                    var tuple = new Tuple<Task, CancellationTokenSource>(task, source);
                    var scriptHost = new Mock<IHostScripts>();
                    {
                        scriptHost.Setup(s => s.Execute(It.IsAny<ScriptLanguage>(), It.IsAny<string>(), It.IsAny<TextWriter>()))
                            .Returns(tuple);
                    }

                    var command = new RunScriptCommand(scriptHost.Object);
                    Assert.IsTrue(command.CanExecute(null));

                    var info = new ScriptRunInformation
                    {
                        Language = ScriptLanguage.IronPython,
                        Script = "a",
                        ScriptOutput = new ScriptOutputPipe(),
                    };
                    command.Execute(info);

                    Assert.AreSame(tuple.Item1, info.ScriptRunningTask);
                    Assert.AreSame(tuple.Item2, info.CancellationToken);
                }
            }
        }
    }
}
