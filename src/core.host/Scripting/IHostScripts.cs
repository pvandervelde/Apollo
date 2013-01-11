//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Apollo.Core.Host.Scripting
{
    /// <summary>
    /// Defines the interface for the script hosting engine.
    /// </summary>
    public interface IHostScripts : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether a script is currently running.
        /// </summary>
        bool IsExecutingScript
        {
            get;
        }
        
        /// <summary>
        /// Executes the given script.
        /// </summary>
        /// <param name="language">The language for the script.</param>
        /// <param name="scriptCode">The script code.</param>
        /// <param name="outputChannel">The object that forwards messages from the script.</param>
        /// <returns>
        /// A tuple that contains the task which is running the script and the
        /// <see cref="CancellationTokenSource"/> object that can be used to cancel the 
        /// running task.
        /// </returns>
        Tuple<Task, CancellationTokenSource> Execute(ScriptLanguage language, string scriptCode, TextWriter outputChannel);

        /// <summary>
        /// Returns an object that can be used to verify the syntax of a script.
        /// </summary>
        /// <param name="language">The language for the script.</param>
        /// <returns>
        ///     The object that verifies script syntax.
        /// </returns>
        ISyntaxVerifier VerifySyntax(ScriptLanguage language);
    }
}
