//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.UserInterfaces.Scripting;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles the closing of the script system.
    /// </summary>
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
        private static void OnCloseScript(IHostScripts scriptHost, ScriptRunInformation info)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (scriptHost == null)
            {
                return;
            }

            if (info != null)
            {
                if ((info.CancellationToken != null) && (info.ScriptRunningTask != null))
                {
                    info.CancellationToken.Cancel();
                    info.ScriptRunningTask.Wait();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseScriptCommand"/> class.
        /// </summary>
        /// <param name="scriptHost">The object that controls the script system.</param>
        public CloseScriptCommand(IHostScripts scriptHost)
            : base(obj => OnCloseScript(scriptHost, obj as ScriptRunInformation), obj => CanCloseScript())
        { 
        }
    }
}
