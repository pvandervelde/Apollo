//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.UI.Wpf.Profiling;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Profiling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ProfileModelTest
    {
        [Test]
        public void Create()
        {
            var context = new Mock<IContextAware>();
            var collection = new TimingReportCollection();

            var report = new Mock<IProfilingTimeReport>();
            collection.Add(report.Object);

            var model = new ProfileModel(context.Object, collection);
            Assert.That(
                model.Results.Cast<IProfilingTimeReport>(),
                Is.EquivalentTo(collection));
        }
    }
}
