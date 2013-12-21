//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Wpf
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

        /// <summary>
        /// The automation ID for the project view name text control.
        /// </summary>
        public const string ProjectName = "Wpf.ProjectView.Name";

        /// <summary>
        /// The automation ID for the project view summary text control.
        /// </summary>
        public const string ProjectSummary = "Wpf.ProjectView.Summary";

        /// <summary>
        /// The automation ID for the project view dataset count control.
        /// </summary>
        public const string DatasetCount = "Wpf.ProjectView.DatasetCount";
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

    /// <summary>
    /// Defines the automation IDs for the dataset view.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Don't want to create too many files for the automation IDs.")]
    internal static class DatasetViewAutomationIds
    {
        /// <summary>
        /// The automation ID for the dataset graph control.
        /// </summary>
        public const string GraphVertex = "Wpf.DatasetView.GraphVertex";

        /// <summary>
        /// The automation ID for the dataset view name text control.
        /// </summary>
        public const string DatasetName = "Wpf.DatasetView.Name";

        /// <summary>
        /// The automation ID for the dataset view summary text control.
        /// </summary>
        public const string DatasetSummary = "Wpf.DatasetView.Summary";

        /// <summary>
        /// The automation ID for the dataset delete control.
        /// </summary>
        public const string DatasetDelete = "Wpf.DatasetView.Delete";

        /// <summary>
        /// The automation ID for the dataset create child control.
        /// </summary>
        public const string DatasetCreateChild = "Wpf.DatasetView.CreateChild";

        /// <summary>
        /// The automation ID for the dataset activation / deactivation control.
        /// </summary>
        public const string DatasetActivateDeactivate = "Wpf.DatasetView.ActivateDeactivate";

        /// <summary>
        /// The automation ID for the control that indicates where the dataset is activated to.
        /// </summary>
        public const string DatasetRunningOn = "Wpf.DatasetView.DatasetRunningOn";
    }

    /// <summary>
    /// Defines the automation IDs for the machine selection view.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Don't want to create too many files for the automation IDs.")]
    internal static class MachineSelectorViewAutomationIds
    {
        /// <summary>
        /// The automation ID for the control that displays the list of available machines.
        /// </summary>
        public const string AvailableMachines = "Wpf.MachineSelectorView.AvailableMachines";

        /// <summary>
        /// The automation ID for the control that confirms the current machine selection.
        /// </summary>
        public const string ConfirmSelection = "Wpf.MachineSelectorView.ConfirmSelection";

        /// <summary>
        /// The automation ID for the control that cancels the current machine selection.
        /// </summary>
        public const string CancelSelection = "Wpf.MachineSelectorView.CancelSelection";
    }
}
