//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;

namespace Apollo.UI.Common.Scripting
{
    /// <summary>
    /// Defines the interface for objects that execute scripts.
    /// </summary>
    public interface IExecuteScripts
    {
        /// <summary>
        /// Executes the given script.
        /// </summary>
        /// <param name="scriptCode">The script code.</param>
        /// <param name="token">The cancellation token used to cancel the running of the script.</param>
        void Execute(string scriptCode, CancellationToken token);

        /// <summary>
        /// Verifies that the script has no syntax errors.
        /// </summary>
        /// <param name="scriptCode">The script code.</param>
        /// <returns>
        ///     A collection containing information about any present syntax problems,
        ///     given as a line number and a syntax error message.
        /// </returns>
        IEnumerable<ScriptErrorInformation> VerifySyntax(string scriptCode);
    }
}
