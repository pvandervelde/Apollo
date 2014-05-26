//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using NUnit.Framework;
using Test.Mocks;

namespace Apollo.UI.Wpf.Feedback
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class FeedbackReportCollectorTest
    {
        [Test]
        public void LocateFeedbackReports()
        {
            var files = new List<string>
                {
                    @"c:\a\def.b",
                };
            var mockDirectory = new MockDirectory(files);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(mockDirectory);
            }

            var collector = new FeedbackReportCollector(fileSystem.Object);
            var reports = collector.LocateFeedbackReports();

            Assert.That(
                reports.Select(r => r.FullName),
                Is.EquivalentTo(files));
        }
    }
}
