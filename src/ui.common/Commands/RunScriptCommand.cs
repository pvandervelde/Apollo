//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.Core.Host.UserInterfaces.Scripting;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// Handles the running of a script.
    /// </summary>
    public sealed class RunScriptCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if a script can be run.
        /// </summary>
        /// <param name="scriptHost">The object that controls the script system.</param>
        /// <returns>
        /// <see langword="true" /> if the script can be run; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanRunScript(IHostScripts scriptHost)
        {
            return (scriptHost != null) ? !scriptHost.IsExecutingScript : false;
        }

        /// <summary>
        /// Runs a given script.
        /// </summary>
        /// <param name="scriptHost">The object that controls the script system.</param>
        /// <param name="info">The information describing the running script.</param>
        private static void OnRunScript(IHostScripts scriptHost, ScriptRunInformation info)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if ((scriptHost == null) || (info == null))
            {
                return;
            }

            var result = scriptHost.Execute(info.Language, info.Script, info.ScriptOutput as TextWriter);
            info.ScriptRunningTask = result.Item1;
            info.CancellationToken = result.Item2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunScriptCommand"/> class.
        /// </summary>
        /// <param name="scriptHost">The object that controls the script system.</param>
        public RunScriptCommand(IHostScripts scriptHost)
            : base(obj => OnRunScript(scriptHost, obj as ScriptRunInformation), obj => CanRunScript(scriptHost))
        { 
        }
    }
}
