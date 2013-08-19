//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;
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

            var fileConstants = new FileConstants(new ApplicationConstants());

            var collector = new FeedbackReportCollector(fileSystem.Object, fileConstants);
            var reports = collector.LocateFeedbackReports();

            Assert.AreElementsEqualIgnoringOrder(files, reports.Select(r => r.FullName));
        }
    }
}
