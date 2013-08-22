//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Profiling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class TimingReportCollectionTest
    {
        [Test]
        public void Add()
        {
            var report = new Mock<IProfilingTimeReport>();

            var collection = new TimingReportCollection();
            collection.CollectionChanged += 
                (s, e) =>
                {
                    Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
                    Assert.AreEqual(1, e.NewItems.Count);
                    Assert.IsNull(e.OldItems);
                    Assert.AreSame(report.Object, e.NewItems[0]);
                };

            collection.Add(report.Object);
            Assert.That(
                collection,
                Is.EquivalentTo(
                    new List<IProfilingTimeReport>
                    {
                        report.Object,
                    }));
        }
    }
}
