//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetViewVertexToDatasetModelConverterTest
    {
        [Test]
        public void ConvertWithNullValue()
        {
            var converter = new DatasetViewVertexToDatasetModelConverter();
            Assert.Throws<InvalidOperationException>(() => converter.Convert(null, null, null, null));
        }

        [Test]
        public void ConvertWithIncorrectTypeOfValue()
        {
            var converter = new DatasetViewVertexToDatasetModelConverter();
            Assert.Throws<InvalidOperationException>(() => converter.Convert(new object(), null, null, null));
        }

        [Test]
        public void Convert()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var progressTracker = new Mock<ITrackSteppingProgress>();
            var projectLink = new Mock<ILinkToProjects>();
            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsLoaded)
                    .Returns(true);
            }

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);
            var vertex = new DatasetViewVertex(context.Object, model);
            var converter = new DatasetViewVertexToDatasetModelConverter();
            var obj = converter.Convert(vertex, null, null, null) as DatasetModel;

            Assert.AreSame(model, obj);
        }
    }
}
