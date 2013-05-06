//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using Moq;
using Nuclei.Progress;

namespace Apollo.Core.Host
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class StartupProgressEventArgsTest
    {
        [Test]
        public void Create()
        {
            int progress = 10;
            var progressMock = new Mock<IProgressMark>();

            var args = new ProgressEventArgs(progress, progressMock.Object);
            Assert.AreEqual<int>(progress, args.Progress);
            Assert.AreEqual<IProgressMark>(progressMock.Object, args.CurrentlyProcessing);
        }
    }
}
