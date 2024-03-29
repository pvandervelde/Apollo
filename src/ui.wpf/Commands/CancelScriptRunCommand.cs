﻿//-----------------------------------------------------------------------
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
    /// Handles the cancelling of a running script.
    /// </summary>
    /// <remarks>
    /// This command is nearly the same as the <see cref="CloseScriptCommand"/> with the difference that
    /// the current command can only be executed if there is a script running.
    /// </remarks>
    public sealed class CancelScriptRunCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the script run can be cancelled.
        /// </summary>
        /// <param name="scriptHost">The object that controls the script system.</param>
        /// <returns>
        /// <see langword="true" /> if the script run can be cancelled; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanCancelScriptRun(IHostScripts scriptHost)
        {
            return (scriptHost != null) && scriptHost.IsExecutingScript;
        }

        /// <summary>
        /// Cancels the running of the script.
        /// </summary>
        /// <param name="scriptHost">The object that controls the script system.</param>
        /// <param name="info">The information describing the running script.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnCancelScriptRun(IHostScripts scriptHost, ScriptRunInformation info, Func<string, IDisposable> timer)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (scriptHost == null)
            {
                return;
            }

            if (info != null)
            {
                using (timer("Cancelling script run"))
                {
                    // Note that the cancellation may take some time ...?
                    info.CancellationToken.Cancel();
                    info.ScriptRunningTask.Wait();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelScriptRunCommand"/> class.
        /// </summary>
        /// <param name="scriptHost">The object that controls the script system.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public CancelScriptRunCommand(IHostScripts scriptHost, Func<string, IDisposable> timer)
            : base(obj => OnCancelScriptRun(scriptHost, obj as ScriptRunInformation, timer), obj => CanCancelScriptRun(scriptHost))
        { 
        }
    }
}
