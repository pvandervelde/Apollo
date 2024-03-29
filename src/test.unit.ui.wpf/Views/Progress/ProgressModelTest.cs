﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using Moq;
using NUnit.Framework;

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
            Assert.AreEqual(3, propertyChangedWasRaised);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "Progress",
                        "Description",
                        "HasErrors"
                    }));
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
            collector.Raise(c => c.OnProgress += null, new ProgressEventArgs(progress, "a", false));

            Assert.AreEqual(progress / 100.0, model.Progress);
            Assert.AreEqual(3, propertyChangedWasRaised);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "Progress",
                        "Description",
                        "HasErrors"
                    }));
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
            Assert.AreEqual(3, propertyChangedWasRaised);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "Progress",
                        "Description",
                        "HasErrors"
                    }));
        }
    }
}
