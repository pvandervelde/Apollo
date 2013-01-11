//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.Core.Host.Scripting;
using Apollo.UI.Wpf.Views.Scripting;
using Apollo.Utilities;
using Microsoft.Practices.Prism.Commands;
using NManto;

namespace Apollo.UI.Wpf.Commands
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
        ///     <see langword="true"/> if the script can be opened from disk; otherwise, <see langword="false"/>.
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
        /// Called when a script should be read from disk.
        /// </summary>
        /// <param name="scriptHost">The object handles the script running.</param>
        /// <param name="selectScriptLanguage">The function that provides the selected script language.</param>
        /// <param name="storeScriptInformation">The function that stores the information about the new script.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnLoadScript(
            IHostScripts scriptHost,
            Func<Tuple<FileInfo, ScriptDescriptionModel>> selectScriptLanguage,
            Action<ScriptDescriptionModel, FileInfo, ISyntaxVerifier> storeScriptInformation,
            Func<string, IDisposable> timer)
        {
            // If there is no project facade, then we're in 
            // designer mode, or something else silly.
            if (scriptHost == null)
            {
                throw new LoadingOfScriptCanceledException();
            }

            using (var interval = timer("Loading script"))
            {
                var tuple = selectScriptLanguage();
                if (tuple.Item1 == null)
                {
                    // The user didn't select anything so just bail.
                    throw new LoadingOfScriptCanceledException();
                }

                var verifier = scriptHost.VerifySyntax(tuple.Item2.Language);
                storeScriptInformation(tuple.Item2, tuple.Item1, verifier);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenScriptCommand"/> class.
        /// </summary>
        /// <param name="scriptHost">The object handles the script running.</param>
        /// <param name="selectScriptLanguage">The function that provides the selected script language.</param>
        /// <param name="storeScriptInformation">The function that stores the information about the new script.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return multiple values and a Tuple works just fine for that.")]
        public OpenScriptCommand(
            IHostScripts scriptHost,
            Func<Tuple<FileInfo, ScriptDescriptionModel>> selectScriptLanguage,
            Action<ScriptDescriptionModel, FileInfo, ISyntaxVerifier> storeScriptInformation,
            Func<string, IDisposable> timer)
            : base(
                obj => OnLoadScript(scriptHost, selectScriptLanguage, storeScriptInformation, timer), 
                obj => CanLoadScript(scriptHost))
        { 
        }
    }
}
