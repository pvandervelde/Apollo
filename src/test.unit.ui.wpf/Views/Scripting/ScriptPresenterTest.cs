//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.Core.Host.Scripting;
using Apollo.UI.Wpf.Commands;
using Apollo.Utilities;
using MbUnit.Framework;
using Microsoft.Practices.Prism.Events;
using Moq;

namespace Apollo.UI.Wpf.Views.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptPresenterTest
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
        public void Initialize()
        {
            var context = new Mock<IContextAware>();
            Func<string, IDisposable> disposeFunc = s => new MockDisposable();

            var view = new Mock<IScriptView>();
            {
                view.SetupProperty(v => v.Model);
            }

            var syntaxVerifier = new Mock<ISyntaxVerifier>();
            var scriptHost = new Mock<IHostScripts>();
            {
                scriptHost.Setup(s => s.VerifySyntax(It.IsAny<ScriptLanguage>()))
                    .Returns(syntaxVerifier.Object);
            }

            var parameter = new ScriptParameter(context.Object);
            var container = new Mock<IDependencyInjectionProxy>();
            {
                container.Setup(c => c.Resolve<IContextAware>())
                    .Returns(context.Object);
                container.Setup(c => c.Resolve<IHostScripts>())
                    .Returns(scriptHost.Object);
                container.Setup(c => c.Resolve<RunScriptCommand>())
                    .Returns(new RunScriptCommand(scriptHost.Object));
                container.Setup(c => c.Resolve<CancelScriptRunCommand>())
                    .Returns(new CancelScriptRunCommand(scriptHost.Object, disposeFunc));
                container.Setup(c => c.Resolve<CloseScriptCommand>())
                    .Returns(new CloseScriptCommand(scriptHost.Object, disposeFunc));
                container.Setup(c => c.Resolve<CloseViewCommand>(It.IsAny<Autofac.Core.Parameter[]>()))
                    .Returns(new CloseViewCommand(new Mock<IEventAggregator>().Object, "a", new ScriptParameter(context.Object)));
                container.Setup(c => c.Resolve<NewScriptCommand>(It.IsAny<Autofac.Core.Parameter[]>()))
                    .Returns(
                        new NewScriptCommand(
                            scriptHost.Object,
                            () => new Tuple<bool, ScriptDescriptionModel>(false, null),
                            (m, s) => { },
                            disposeFunc));
                container.Setup(c => c.Resolve<ISyntaxVerifier>())
                    .Returns(syntaxVerifier.Object);
                container.Setup(c => c.Resolve<OpenScriptCommand>(It.IsAny<Autofac.Core.Parameter[]>()))
                    .Returns(
                        new OpenScriptCommand(
                            scriptHost.Object,
                            () => new Tuple<FileInfo, ScriptDescriptionModel>(null, null),
                            (s, f, v) => { },
                            disposeFunc));
            }

            var presenter = new ScriptPresenter(container.Object);
            ((IPresenter)presenter).Initialize(view.Object, parameter);

            Assert.AreSame(view.Object, presenter.View);
            Assert.AreSame(parameter, presenter.Parameter);
            view.VerifySet(v => v.Model = It.IsAny<ScriptModel>(), Times.Once());
        }
    }
}
