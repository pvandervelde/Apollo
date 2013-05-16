//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.UI.Wpf.Views.Feedback;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class FeedbackReportToTextConverterTest
    {
        [Test]
        public void ConvertWithFeedbackFileModel()
        {
            var context = new Mock<IContextAware>();

            var applicationName = "MyApplication";
            var path = string.Format(
                CultureInfo.InvariantCulture,
                @"c:\temp\{0}_12345566.nsdump",
                applicationName);
            var date = DateTimeOffset.Now;
            var model = new FeedbackFileModel(context.Object, path, date);

            var expected = string.Format(
                CultureInfo.CurrentCulture,
                "Error in: {0} - Occurred on: {1}",
                applicationName,
                date);
            
            var converter = new FeedbackReportToTextConverter();
            var convertedValue = converter.Convert(model, null, null, null);
            Assert.AreEqual(expected, convertedValue);
        }

        [Test]
        public void ConvertWithNullReference()
        {
            var converter = new FeedbackReportToTextConverter();
            Assert.Throws<ArgumentException>(() => converter.Convert(null, null, null, null));
        }

        [Test]
        public void ConvertWithNonFeedbackFileModelObject()
        {
            var converter = new FeedbackReportToTextConverter();
            Assert.Throws<ArgumentException>(() => converter.Convert(new object(), null, null, null));
        }
    }
}
