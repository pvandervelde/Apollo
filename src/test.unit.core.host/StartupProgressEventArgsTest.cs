//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using MbUnit.Framework;

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
            var text = "a";

            var args = new ProgressEventArgs(progress, text);
            Assert.AreEqual(progress, args.Progress);
            Assert.AreEqual(text, args.Description);
        }
    }
}
