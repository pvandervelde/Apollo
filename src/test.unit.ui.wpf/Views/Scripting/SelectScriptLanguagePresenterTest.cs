//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SelectScriptLanguagePresenterTest
    {
        [Test]
        public void Initialize()
        {
            var context = new Mock<IContextAware>();

            var view = new Mock<ISelectScriptLanguageView>();
            {
                view.SetupSet(v => v.Model = It.IsAny<SelectScriptLanguageModel>())
                    .Verifiable();
            }

            var parameter = new SelectScriptLanguageParameter(context.Object);
            var container = new Mock<IDependencyInjectionProxy>();
            {
                container.Setup(c => c.Resolve<IContextAware>())
                    .Returns(context.Object);
            }

            var presenter = new SelectScriptLanguagePresenter(container.Object);
            ((IPresenter)presenter).Initialize(view.Object, parameter);

            Assert.AreSame(view.Object, presenter.View);
            Assert.AreSame(parameter, presenter.Parameter);
            view.VerifySet(v => v.Model = It.IsAny<SelectScriptLanguageModel>(), Times.Once());
        }
    }
}
