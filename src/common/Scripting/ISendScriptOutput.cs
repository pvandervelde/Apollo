//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Common.Scripting
{
    /// <summary>
    /// Defines the interface for objects that send script output text to whomever wants to hear 
    /// about it.
    /// </summary>
    public interface ISendScriptOutput
    {
        /// <summary>
        /// An event raised if new output has been received from a script.
        /// </summary>
        event EventHandler<ScriptOutputEventArgs> OnScriptOutput;
    }
}
