//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Apollo.Core.Base;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using MbUnit.Framework;
using Microsoft.Practices.Prism.Events;
using Moq;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetGraphPresenterTest
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

        private static Mock<IProxyDataset> CreateDataset(int index, IEnumerable<Mock<IProxyDataset>> children)
        {
            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.Name)
                    .Returns(index.ToString(CultureInfo.InvariantCulture));
                proxy.Setup(p => p.Children())
                    .Returns(children.Select(c => c.Object));
                proxy.Setup(p => p.Id)
                    .Returns(new DatasetId(index));
                proxy.Setup(d => d.Equals(It.IsAny<IProxyDataset>()))
                    .Returns<IProxyDataset>(other => ReferenceEquals(other, proxy.Object));
                proxy.Setup(d => d.Equals(It.IsAny<object>()))
                    .Returns<object>(other => ReferenceEquals(other, proxy.Object));
                proxy.Setup(d => d.GetHashCode())
                    .Returns(proxy.GetHashCode());
            }

            return proxy;
        }

        [Test]
        public void Initialize()
        {
            var context = new Mock<IContextAware>();
            var progress = new Mock<ITrackSteppingProgress>();
            var aggregator = new Mock<IEventAggregator>();
            Func<string, IDisposable> func = s => new MockDisposable();

            var proxyDataset = CreateDataset(0, new List<Mock<IProxyDataset>>());
            var project = new Mock<IProject>();
            {
                project.Setup(p => p.BaseDataset())
                    .Returns(proxyDataset.Object);
            }

            var projectFacade = new ProjectFacade(project.Object);
            var projectLink = new Mock<ILinkToProjects>();
            {
                projectLink.Setup(p => p.ActiveProject())
                    .Returns(projectFacade);
            }

            var view = new Mock<IDatasetGraphView>();
            {
                view.SetupSet(v => v.Model = It.IsAny<DatasetGraphModel>())
                    .Verifiable();
            }

            var parameter = new DatasetGraphParameter(context.Object);

            var container = new Mock<IDependencyInjectionProxy>();
            {
                container.Setup(c => c.Resolve<IContextAware>())
                    .Returns(context.Object);
                container.Setup(c => c.Resolve<ILinkToProjects>())
                    .Returns(projectLink.Object);
                container.Setup(c => c.Resolve<ITrackSteppingProgress>())
                    .Returns(progress.Object);
                container.Setup(c => c.Resolve<IEventAggregator>())
                    .Returns(aggregator.Object);
                container.Setup(c => c.Resolve<Func<string, IDisposable>>())
                    .Returns(func);
            }

            var presenter = new DatasetGraphPresenter(container.Object);
            ((IPresenter)presenter).Initialize(view.Object, parameter);

            Assert.AreSame(view.Object, presenter.View);
            Assert.AreSame(parameter, presenter.Parameter);
            view.VerifySet(v => v.Model = It.IsAny<DatasetGraphModel>(), Times.Once());
        }
    }
}
