//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Scripting;
using Apollo.UI.Wpf.Views.Scripting;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class NewScriptCommandTest
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
        public void CanCreateNewScriptWithNullHost()
        {
            Func<Tuple<bool, ScriptDescriptionModel>> selectScript = () => new Tuple<bool, ScriptDescriptionModel>(false, null);
            Action<ScriptDescriptionModel, ISyntaxVerifier> storeScript = (s, v) => { };
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new NewScriptCommand(null, selectScript, storeScript, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanCreateNewScriptWhileHostIsExecuting()
        {
            var scriptHost = new Mock<IHostScripts>();
            {
                scriptHost.Setup(s => s.IsExecutingScript)
                    .Returns(true);
            }

            Func<Tuple<bool, ScriptDescriptionModel>> selectScript = () => new Tuple<bool, ScriptDescriptionModel>(false, null);
            Action<ScriptDescriptionModel, ISyntaxVerifier> storeScript = (s, v) => { };
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new NewScriptCommand(scriptHost.Object, selectScript, storeScript, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CreateNewScriptWithoutSelectingScriptLanguage()
        {
            var scriptHost = new Mock<IHostScripts>();
            {
                scriptHost.Setup(s => s.IsExecutingScript)
                    .Returns(true);
            }

            Func<Tuple<bool, ScriptDescriptionModel>> selectScript = () => new Tuple<bool, ScriptDescriptionModel>(false, null);
            Action<ScriptDescriptionModel, ISyntaxVerifier> storeScript = (s, v) => { };
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new NewScriptCommand(scriptHost.Object, selectScript, storeScript, timerFunc);
            Assert.Throws<CreationOfNewScriptCanceledException>(() => command.Execute(null));
        }

        [Test]
        public void CreateNewScript()
        {
            var verifier = new Mock<ISyntaxVerifier>();

            var scriptHost = new Mock<IHostScripts>();
            {
                scriptHost.Setup(s => s.IsExecutingScript)
                    .Returns(true);
                scriptHost.Setup(s => s.VerifySyntax(It.IsAny<ScriptLanguage>()))
                    .Returns(verifier.Object)
                    .Verifiable();
            }

            var context = new Mock<IContextAware>();
            var model = new ScriptDescriptionModel(context.Object, ScriptLanguage.IronPython);

            Func<Tuple<bool, ScriptDescriptionModel>> selectScript = () => new Tuple<bool, ScriptDescriptionModel>(true, model);
            Action<ScriptDescriptionModel, ISyntaxVerifier> storeScript = 
                (s, v) =>
                {
                    Assert.AreSame(model, s);
                    Assert.AreSame(verifier.Object, v);
                };
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new NewScriptCommand(scriptHost.Object, selectScript, storeScript, timerFunc);
            command.Execute(null);
        }
    }
}
