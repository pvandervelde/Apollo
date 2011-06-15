//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace Apollo.UI.Common.Scripting
{
    /// <summary>
    /// Stores information necessary to run a script.
    /// </summary>
    internal sealed class ScriptRunInformation
    {
        /// <summary>
        /// Gets or sets the language of the script.
        /// </summary>
        public ScriptLanguage Language 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the script text.
        /// </summary>
        public string Script 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the task that is responsible for running the script.
        /// </summary>
        public Task ScriptRunningTask
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cancellation token that is used to cancel the
        /// running of the script.
        /// </summary>
        public CancellationTokenSource CancellationToken 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the object that passes through the output from the script.
        /// </summary>
        public ISendScriptOutput ScriptOutput
        {
            get;
            set;
        }
    }
}
