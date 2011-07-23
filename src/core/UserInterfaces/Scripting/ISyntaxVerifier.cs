//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.UserInterfaces.Scripting
{
    /// <summary>
    /// Defines the interface for objects that perform verification
    /// of the syntax of a script.
    /// </summary>
    public interface ISyntaxVerifier : IDisposable
    {
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
