//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.UI.Common.Scripting
{
    /// <summary>
    /// Stores information about a (syntax) error in a given script.
    /// </summary>
    public sealed class ScriptErrorInformation
    {
        /// <summary>
        /// Gets or sets the line on which the error occurred.
        /// </summary>
        public int Line
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the column on which the error occurred.
        /// </summary>
        public int Column
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error severity.
        /// </summary>
        public SyntaxVerificationSeverity Severity
        {
            get;
            set;
        }
    }
}
