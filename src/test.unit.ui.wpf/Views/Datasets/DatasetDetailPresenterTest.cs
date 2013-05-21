//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Wpf.Commands;
using Apollo.Utilities;
using MbUnit.Framework;
using Microsoft.Practices.Prism.Events;
using Moq;
using Nuclei.Progress;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetDetailPresenterTest
    {
        [Test]
        public void Initialize()
        {
            var context = new Mock<IContextAware>();
            var project = new Mock<ILinkToProjects>();
            var progress = new Mock<ITrackSteppingProgress>();
            var aggregator = new Mock<IEventAggregator>();

            var view = new Mock<IDatasetDetailView>();

            var proxy = new Mock<IProxyDataset>();
            var dataset = new DatasetFacade(proxy.Object);
            var parameter = new DatasetDetailParameter(context.Object, dataset);

            var container = new Mock<IDependencyInjectionProxy>();
            {
                container.Setup(c => c.Resolve<IContextAware>())
                    .Returns(context.Object);
                container.Setup(c => c.Resolve<ILinkToProjects>())
                    .Returns(project.Object);
                container.Setup(c => c.Resolve<ITrackSteppingProgress>())
                    .Returns(progress.Object);
                container.Setup(c => c.Resolve<CloseViewCommand>(It.IsAny<Autofac.Core.Parameter[]>()))
                    .Returns(new CloseViewCommand(aggregator.Object, "a", parameter));
            }

            var presenter = new DatasetDetailPresenter(container.Object);
            ((IPresenter)presenter).Initialize(view.Object, parameter);

            Assert.AreSame(view.Object, presenter.View);
            Assert.AreSame(parameter, presenter.Parameter);
        }
    }
}
