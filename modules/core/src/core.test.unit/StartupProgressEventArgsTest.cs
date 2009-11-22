//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Test.Unit
{
    [TestFixture]
    [Description("Tests the StartupProgressEventArgs class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class StartupProgressEventArgsTest
    {
        [Test]
        [Description("Checks that it is not possible to create progress event arguments.")]
        public void Create()
        {
            int progress = 10;
            var progressMock = new Mock<IProgressMark>();

            var args = new StartupProgressEventArgs(progress, progressMock.Object);
            Assert.AreEqual<int>(progress, args.Progress);
            Assert.AreEqual<IProgressMark>(progressMock.Object, args.CurrentlyProcessing);
        }

        [Test]
        [Description("Checks that it is not possible to create progress event arguments without a mark.")]
        public void CreateWithNullMarker()
        {
            Assert.Throws<ArgumentNullException>(() => new StartupProgressEventArgs(10, null));
        }

        [Test]
        [Description("Checks that it is not possible to create progress event arguments with negative progress.")]
        public void CreateWithTooLowNumber()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new StartupProgressEventArgs(-10, new Mock<IProgressMark>().Object));
        }

        [Test]
        [Description("Checks that it is not possible to create progress event arguments with more than 100% progress.")]
        public void CreateWithTooHighNumber()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new StartupProgressEventArgs(110, new Mock<IProgressMark>().Object));
        }
    }
}
