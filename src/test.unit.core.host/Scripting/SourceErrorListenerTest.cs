//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Scripting;
using NUnit.Framework;

namespace Apollo.Core.Host.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SourceErrorListenerTest
    {
        [Test]
        public void ErrorReported()
        {
            var listener = new SourceErrorListener();

            var message = "message";
            var span = new SourceSpan(
                new SourceLocation(1, 1, 1),
                new SourceLocation(2, 2, 2));
            var errorCode = 10;
            var severity = Severity.FatalError;
            listener.ErrorReported(null, message, span, errorCode, severity);

            var list = new List<ScriptErrorInformation> 
                {
                    new ScriptErrorInformation
                        {
                            Column = span.Start.Column,
                            Line = span.Start.Line,
                            Message = message,
                            Severity = SyntaxVerificationSeverity.Error,
                        },
                };
            Assert.That(listener.Errors(), Is.EquivalentTo(list));
        }
    }
}
