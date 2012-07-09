//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles the showing of the dataset advanced view.
    /// </summary>
    public sealed class SwitchDatasetToEditModeCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the dataset can be switched to edit mode.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>
        /// <see langword="true" /> if dataset can be switched to edit mode; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanSwitchToEditMode(DatasetFacade dataset)
        {
            return !dataset.IsEditMode;
        }

        /// <summary>
        /// Switches the dataset to the edit mode.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        private static void OnSwitchToEditMode(DatasetFacade dataset)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (dataset == null)
            {
                return;
            }

            dataset.SwitchToEditMode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchDatasetToEditModeCommand"/> class.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        public SwitchDatasetToEditModeCommand(DatasetFacade dataset)
            : base(obj => OnSwitchToEditMode(dataset), obj => CanSwitchToEditMode(dataset))
        {
        }
    }
}
