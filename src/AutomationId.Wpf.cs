//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Explorer
{
    /// <summary>
    /// Defines the automation IDs for the project view.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Don't want to create too many files for the automation IDs.")]
    internal static class ProjectViewAutomationIds
    {
        /// <summary>
        /// The automation ID for the header of the control.
        /// </summary>
        public const string Header = "Wpf.ProjectView.Header";

        /// <summary>
        /// The automation ID for the project view tab close button.
        /// </summary>
        public const string CloseTabButton = "Wpf.ProjectView.CloseTabButton";
    }

    /// <summary>
    /// Defines the automation IDs for the script view.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Don't want to create too many files for the automation IDs.")]
    internal static class ScriptViewAutomationIds
    {
        /// <summary>
        /// The automation ID for the header of the control.
        /// </summary>
        public const string Header = "Wpf.ScriptView.Header";

        /// <summary>
        /// The automation ID for the script view tab close button.
        /// </summary>
        public const string CloseTabButton = "Wpf.ScriptView.CloseTabButton";
    }
}
