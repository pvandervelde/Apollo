//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// Handles the deactivation of a dataset.
    /// </summary>
    public sealed class DeactivateDatasetCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines whether the dataset can be deactivated.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>
        ///     <see langword="true"/> if the dataset can be deactivated; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanDeactivate(DatasetFacade dataset)
        {
            // If there is no application facade, then we're in 
            // designer mode, or something else silly.
            if (dataset == null)
            {
                return false;
            }

            return dataset.IsActivated;
        }

        /// <summary>
        /// Called when the dataset should be deactivated.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnDeactivate(ILinkToProjects projectFacade, DatasetFacade dataset, Func<string, IDisposable> timer)
        {
            // If there is no application facade, then we're in 
            // designer mode, or something else silly.
            if (dataset == null)
            {
                return;
            }

            if (!dataset.IsActivated)
            {
                return;
            }

            using (timer("Unloading dataset"))
            {
                dataset.Deactivate();
                projectFacade.ActiveProject().History.Mark();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeactivateDatasetCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public DeactivateDatasetCommand(ILinkToProjects projectFacade, DatasetFacade dataset, Func<string, IDisposable> timer)
            : base(obj => OnDeactivate(projectFacade, dataset, timer), obj => CanDeactivate(dataset))
        {
        }
    }
}
