//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Host.UserInterfaces.Scripting
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that stores the text output of a script run.
    /// </summary>
    public sealed class ScriptOutputEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptOutputEventArgs"/> class.
        /// </summary>
        /// <param name="text">The output text.</param>
        public ScriptOutputEventArgs(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Gets the output text.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }
    }
}
