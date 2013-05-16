//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Scripting;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// Handles the closing of the script system.
    /// </summary>
    /// <remarks>
    /// This command is nearly the same as the <see cref="CancelScriptRunCommand"/> with the difference that
    /// the current command doesn't care if there is a script running or not. If not we don't have to do anything,
    /// if so then we just stop it.
    /// </remarks>
    public sealed class CloseScriptCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the script host can be closed.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the script host can be closed; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanCloseScript()
        {
            return true;
        }

        /// <summary>
        /// Closes the script host.
        /// </summary>
        /// <param name="scriptHost">The object that controls the script system.</param>
        /// <param name="info">The information describing the running script.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnCloseScript(IHostScripts scriptHost, ScriptRunInformation info, Func<string, IDisposable> timer)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (scriptHost == null)
            {
                return;
            }

            // All we do in here is to stop the script if it is running. There is nothing else to do
            if (info != null)
            {
                using (timer("Closing script"))
                {
                    if ((info.CancellationToken != null) && (info.ScriptRunningTask != null))
                    {
                        info.CancellationToken.Cancel();
                        info.ScriptRunningTask.Wait();
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseScriptCommand"/> class.
        /// </summary>
        /// <param name="scriptHost">The object that controls the script system.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public CloseScriptCommand(IHostScripts scriptHost, Func<string, IDisposable> timer)
            : base(obj => OnCloseScript(scriptHost, obj as ScriptRunInformation, timer), obj => CanCloseScript())
        { 
        }
    }
}
