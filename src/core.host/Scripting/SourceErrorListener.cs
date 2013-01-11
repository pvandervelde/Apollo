//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace Apollo.Core.Host.Scripting
{
    /// <summary>
    /// An <see cref="ErrorListener"/> that is used to report errors while verifying script
    /// syntax or running a script.
    /// </summary>
    internal sealed class SourceErrorListener : ErrorListener
    {
        private static SyntaxVerificationSeverity Translate(Severity severity)
        {
            switch (severity)
            {
                case Severity.Error:
                    return SyntaxVerificationSeverity.Error;
                case Severity.FatalError:
                    return SyntaxVerificationSeverity.Error;
                case Severity.Ignore:
                    return SyntaxVerificationSeverity.None;
                case Severity.Warning:
                    return SyntaxVerificationSeverity.Warning;
                default:
                    return SyntaxVerificationSeverity.Unknown;
            }
        }

        /// <summary>
        /// The collection that stores all the error messages.
        /// </summary>
        private readonly List<ScriptErrorInformation> m_Errors
            = new List<ScriptErrorInformation>();

        /// <summary>
        /// Stores the reported error.
        /// </summary>
        /// <param name="source">The source in which the error occurred.</param>
        /// <param name="message">The error message.</param>
        /// <param name="span">The location of the error in the script file.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="severity">The severity of the error.</param>
        public override void ErrorReported(ScriptSource source, string message, SourceSpan span, int errorCode, Severity severity)
        {
            var error = new ScriptErrorInformation 
                { 
                    Line = span.Start.Line,
                    Column = span.Start.Column,
                    Message = message,
                    Severity = Translate(severity),
                };
            m_Errors.Add(error);
        }

        /// <summary>
        /// Returns the collection containing the syntax errors.
        /// </summary>
        /// <returns>
        /// The collection containing the syntax errors.
        /// </returns>
        public IEnumerable<ScriptErrorInformation> Errors()
        {
            return m_Errors;
        }
    }
}
