﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetViewVertexTest
    {
        [Test]
        public void OnActivated()
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
                proxy.Setup(p => p.IsActivated)
                    .Returns(true);
            }

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);
            var vertex = new DatasetViewVertex(context.Object, model);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            vertex.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };
            
            proxy.Raise(p => p.OnActivated += null, EventArgs.Empty);
            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "IsDatasetActivated",
                    }));
            Assert.IsTrue(vertex.IsDatasetActivated);
        }

        [Test]
        public void OnDeactivated()
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
                proxy.Setup(p => p.IsActivated)
                    .Returns(false);
            }

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);
            var vertex = new DatasetViewVertex(context.Object, model);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            vertex.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            proxy.Raise(p => p.OnDeactivated += null, EventArgs.Empty);
            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "IsDatasetActivated",
                    }));
            Assert.IsFalse(vertex.IsDatasetActivated);
        }
    }
}
