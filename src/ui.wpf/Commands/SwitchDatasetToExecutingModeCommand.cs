//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// Handles switching the dataset to executing mode.
    /// </summary>
    public sealed class SwitchDatasetToExecutingModeCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the dataset can be switched to executing mode.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>
        /// <see langword="true" /> if the dataset can be switched to executing mode; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanSwitchToExecutingMode(DatasetFacade dataset)
        {
            return dataset.IsEditMode;
        }

        /// <summary>
        /// Switches the dataset to the executing mode.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        private static void OnSwitchToExecutingMode(DatasetFacade dataset)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (dataset == null)
            {
                return;
            }

            dataset.SwitchToExecutingMode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchDatasetToExecutingModeCommand"/> class.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        public SwitchDatasetToExecutingModeCommand(DatasetFacade dataset)
            : base(obj => OnSwitchToExecutingMode(dataset), obj => CanSwitchToExecutingMode(dataset))
        {
        }
    }
}
