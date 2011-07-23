//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using MbUnit.Framework;

namespace Apollo.Core.UserInterfaces.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CancelScriptTokenTest
    {
        [Test]
        public void CreateWithCancellationToken()
        {
            var source = new CancellationTokenSource();
            var token = new CancelScriptToken(source.Token);

            Assert.IsFalse(token.IsCancellationRequested);

            source.Cancel();
            Assert.IsTrue(token.IsCancellationRequested);
        }
    }
}
