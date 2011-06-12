//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.UI.Common.Scripting;
using Apollo.UI.Common.Views.Scripting;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles the opening of a script from disk.
    /// </summary>
    public sealed class OpenScriptCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if a new project can be created.
        /// </summary>
        /// <param name="scriptHost">The object handles the script running.</param>
        /// <returns>
        ///     <see langword="true"/> if a new project can be created; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanLoadScript(IHostScripts scriptHost)
        {
            // If there is no project facade, then we're in 
            // designer mode, or something else silly.
            if (scriptHost == null)
            {
                return false;
            }

            return !scriptHost.IsExecutingScript;
        }
        
        /// <summary>
        /// Called when the creation of a new project is required.
        /// </summary>
        /// <param name="scriptHost">The object handles the script running.</param>
        /// <param name="selectScriptLanguage">The function that provides the selected script language.</param>
        /// <param name="storeScriptInformation">The function that stores the information about the new script.</param>
        private static void OnLoadScript(
            IHostScripts scriptHost,
            Func<Tuple<FileInfo, ScriptDescriptionModel>> selectScriptLanguage,
            Action<ScriptDescriptionModel, FileInfo, ISyntaxVerifier> storeScriptInformation)
        {
            // If there is no project facade, then we're in 
            // designer mode, or something else silly.
            if (scriptHost == null)
            {
                throw new LoadingOfScriptCancelledException();
            }

            var tuple = selectScriptLanguage();
            if (tuple.Item1 == null)
            {
                // The user didn't select anything so just bail.
                throw new LoadingOfScriptCancelledException();
            }
            
            var verifier = scriptHost.VerifySyntax(tuple.Item2.Language);
            storeScriptInformation(tuple.Item2, tuple.Item1, verifier);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenScriptCommand"/> class.
        /// </summary>
        /// <param name="scriptHost">The object handles the script running.</param>
        /// <param name="selectScriptLanguage">The function that provides the selected script language.</param>
        /// <param name="storeScriptInformation">The function that stores the information about the new script.</param>
        public OpenScriptCommand(
            IHostScripts scriptHost,
            Func<Tuple<FileInfo, ScriptDescriptionModel>> selectScriptLanguage,
            Action<ScriptDescriptionModel, FileInfo, ISyntaxVerifier> storeScriptInformation)
            : base(obj => OnLoadScript(scriptHost, selectScriptLanguage, storeScriptInformation), obj => CanLoadScript(scriptHost))
        { 
        }
    }
}
