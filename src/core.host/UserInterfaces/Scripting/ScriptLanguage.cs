//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Host.UserInterfaces.Scripting
{
    /// <summary>
    /// Defines the language in which a script is written.
    /// </summary>
    public enum ScriptLanguage
    {
        /// <summary>
        /// Indicates that no script language has been chosen.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the script is written in the IronPython language.
        /// </summary>
        IronPython,

        /// <summary>
        /// Indicates that the script is written in the IronRuby language.
        /// </summary>
        IronRuby,

        /// <summary>
        /// Indicates that the script is written in the PowerShell language.
        /// </summary>
        PowerShell,
    }
}
