//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using IronPython.Hosting;

namespace Apollo.UI.Common.Scripting
{
    /// <summary>
    /// Defines methods for launching an <see cref="IExecuteScripts"/> object in a remote <see cref="AppDomain"/>.
    /// </summary>
    internal sealed class ScriptDomainLauncher : MarshalByRefObject
    {
        /// <summary>
        /// Creates a new <see cref="IExecuteScripts"/> objects and returns the proxy to the object.
        /// </summary>
        /// <param name="language">The language that the <c>IExecuteScripts</c> object should be able to process.</param>
        /// <param name="projects">The object that handles all project activities.</param>
        /// <returns>
        ///     The object that can execute scripts.
        /// </returns>
        public IExecuteScripts Launch(ScriptLanguage language, ILinkScriptsToProjects projects)
        {
            switch (language)
            {
                case ScriptLanguage.None:
                    throw new InvalidScriptLanguageException(language);
                case ScriptLanguage.IronPython:
                    return new RemoteScriptRunner(projects, Python.CreateEngine());
                case ScriptLanguage.IronRuby:
                    throw new NotImplementedException();
                case ScriptLanguage.PowerShell:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
