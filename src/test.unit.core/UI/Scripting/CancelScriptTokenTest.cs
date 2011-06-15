//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Apollo.UI.Common.Scripting;
using MbUnit.Framework;

namespace Apollo.UI.Scripting
{
    [TestFixture]
    [Description("Tests the CancelScriptToken class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CancelScriptTokenTest
    {
        [Test]
        [Description("Checks that the token state is set to cancelled if the attached CancellationToken is set to cancelled.")]
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
