//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.UI.Common.Scripting
{
    /// <summary>
    /// Indicates the severity of an syntax error in the script.
    /// </summary>
    public enum SyntaxVerificationSeverity
    {
        /// <summary>
        /// There is no syntax error.
        /// </summary>
        None,

        /// <summary>
        /// The severity indicates the the script will run but it is
        /// highly advisable to make changes that will remove the syntax
        /// warnings.
        /// </summary>
        Warning,

        /// <summary>
        /// The severity indicates that the script will not run due to 
        /// syntax errors.
        /// </summary>
        Error,

        /// <summary>
        /// The severity is unknown.
        /// </summary>
        Unknown,
    }
}
