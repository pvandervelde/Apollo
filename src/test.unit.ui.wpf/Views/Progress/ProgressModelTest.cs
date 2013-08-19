//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Views.Progress
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ProgressModelTest
    {
        [Test]
        public void OnStartProgress()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var collector = new Mock<ICollectProgressReports>();
            var model = new ProgressModel(context.Object, collector.Object);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            collector.Raise(c => c.OnStartProgress += null, EventArgs.Empty);

            Assert.AreEqual(0.0, model.Progress);
            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreElementsEqual(
                new List<string>
                    {
                        "Progress",
                    },
                properties);
        }

        [Test]
        public void OnProgress()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var collector = new Mock<ICollectProgressReports>();
            var model = new ProgressModel(context.Object, collector.Object);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            var progress = 50;
            collector.Raise(c => c.OnProgress += null, new ProgressEventArgs(progress, "a"));

            Assert.AreEqual(progress / 100.0, model.Progress);
            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreElementsEqual(
                new List<string>
                    {
                        "Progress",
                    },
                properties);
        }

        [Test]
        public void OnStopProgress()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var collector = new Mock<ICollectProgressReports>();
            var model = new ProgressModel(context.Object, collector.Object);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            collector.Raise(c => c.OnStopProgress += null, EventArgs.Empty);

            Assert.AreEqual(0.0, model.Progress);
            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreElementsEqual(
                new List<string>
                    {
                        "Progress",
                    },
                properties);
        }
    }
}
