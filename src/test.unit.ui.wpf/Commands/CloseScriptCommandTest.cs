//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Host.Scripting;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CloseScriptCommandTest
    {
        private sealed class MockDisposable : IDisposable
        {
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // Do nothing.
            }
        }

        [Test]
        public void CloseScript()
        {
            var scriptHost = new Mock<IHostScripts>();
            {
                scriptHost.Setup(s => s.IsExecutingScript)
                    .Returns(true);
            }

            Func<string, IDisposable> func = s => new MockDisposable();

            var command = new CloseScriptCommand(scriptHost.Object, func);
            Assert.IsTrue(command.CanExecute(null));

            var info = new ScriptRunInformation
            {
                ScriptRunningTask = Task.Factory.StartNew(() => { }),
                CancellationToken = new CancellationTokenSource(),
            };
            command.Execute(info);

            Assert.IsTrue(info.CancellationToken.IsCancellationRequested);
        }
    }
}
