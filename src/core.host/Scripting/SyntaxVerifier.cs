//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.Host.Scripting
{
    /// <summary>
    /// Provides repeatable verification of script syntax.
    /// </summary>
    internal sealed class SyntaxVerifier : ISyntaxVerifier
    {
        /// <summary>
        /// The <c>AppDomain</c> in which the <c>executor</c> lives.
        /// </summary>
        private readonly AppDomain m_Domain;

        /// <summary>
        /// The object that performs the actual verification of the
        /// script syntax.
        /// </summary>
        private readonly IExecuteScripts m_Executor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxVerifier"/> class.
        /// </summary>
        /// <param name="executorDomain">The <see cref="AppDomain"/> in which the <paramref name="executor"/> lives.</param>
        /// <param name="executor">The object that performs the actual verification of the script syntax.</param>
        public SyntaxVerifier(AppDomain executorDomain, IExecuteScripts executor)
        {
            m_Domain = executorDomain;
            m_Executor = executor;
        }

        /// <summary>
        /// Verifies that the script has no syntax errors.
        /// </summary>
        /// <param name="scriptCode">The script code.</param>
        /// <returns>
        ///     A collection containing information about any present syntax problems,
        ///     given as a line number and a syntax error message.
        /// </returns>
        public IEnumerable<ScriptErrorInformation> VerifySyntax(string scriptCode)
        {
            return m_Executor.VerifySyntax(scriptCode);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            AppDomain.Unload(m_Domain);
        }
    }
}
