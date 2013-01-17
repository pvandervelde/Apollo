//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Scripting;
using Apollo.UI.Wpf.Views.Scripting;
using Apollo.Utilities;
using Microsoft.Practices.Prism.Commands;
using Utilities.Diagnostics.Profiling;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// Handles the creation of new scripts.
    /// </summary>
    public sealed class NewScriptCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if a new script can be created.
        /// </summary>
        /// <param name="scriptHost">The object handles the script running.</param>
        /// <returns>
        ///     <see langword="true"/> if a new script can be created; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanCreateNewScript(IHostScripts scriptHost)
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
        /// Called when the creation of a new script is required.
        /// </summary>
        /// <param name="scriptHost">The object handles the script running.</param>
        /// <param name="selectScriptLanguage">The function that provides the selected script language.</param>
        /// <param name="storeScriptInformation">The function that stores the information about the new script.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnCreateNewScript(
            IHostScripts scriptHost,
            Func<Tuple<bool, ScriptDescriptionModel>> selectScriptLanguage,
            Action<ScriptDescriptionModel, ISyntaxVerifier> storeScriptInformation,
            Func<string, IDisposable> timer)
        {
            // If there is no project facade, then we're in 
            // designer mode, or something else silly.
            if (scriptHost == null)
            {
                throw new CreationOfNewScriptCanceledException();
            }

            using (var interval = timer("Creating new script"))
            {
                var tuple = selectScriptLanguage();
                if (!tuple.Item1)
                {
                    // The user didn't select anything so just bail.
                    throw new CreationOfNewScriptCanceledException();
                }

                var verifier = scriptHost.VerifySyntax(tuple.Item2.Language);
                storeScriptInformation(tuple.Item2, verifier);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewScriptCommand"/> class.
        /// </summary>
        /// <param name="scriptHost">The object handles the script running.</param>
        /// <param name="selectScriptLanguage">The function that provides the selected script language.</param>
        /// <param name="storeScriptInformation">The function that stores the information about the new script.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return multiple values and a Tuple works just fine for that.")]
        public NewScriptCommand(
            IHostScripts scriptHost,
            Func<Tuple<bool, ScriptDescriptionModel>> selectScriptLanguage,
            Action<ScriptDescriptionModel, ISyntaxVerifier> storeScriptInformation,
            Func<string, IDisposable> timer)
            : base(
                obj => OnCreateNewScript(scriptHost, selectScriptLanguage, storeScriptInformation, timer),
                obj => CanCreateNewScript(scriptHost))
        {
        }
    }
}
