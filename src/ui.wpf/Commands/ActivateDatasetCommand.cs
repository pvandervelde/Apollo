//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Apollo.Core.Base.Activation;
using Apollo.Core.Host.UserInterfaces.Projects;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// Handles the activation of a dataset.
    /// </summary>
    public sealed class ActivateDatasetCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines whether the dataset can be activated.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>
        ///     <see langword="true"/> if the dataset can be activated; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanActivate(DatasetFacade dataset)
        {
            // If there is no application facade, then we're in 
            // designer mode, or something else silly.
            if (dataset == null)
            {
                return false;
            }

            return !dataset.IsActivated && dataset.CanActivate;
        }

        /// <summary>
        /// Called when the dataset should be activated.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="selector">
        ///     The function that is used to select the most suitable machine to distribute the dataset to.
        /// </param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnActivate(
            ILinkToProjects projectFacade,
            DatasetFacade dataset,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector,
            Func<string, IDisposable> timer)
        {
            // If there is no application facade, then we're in 
            // designer mode, or something else silly.
            if (dataset == null)
            {
                return;
            }

            if (dataset.IsActivated || !dataset.CanActivate)
            {
                return;
            }

            using (timer("Loading dataset onto machine"))
            {
                var source = new CancellationTokenSource();
                dataset.Activate(
                    DistributionLocations.All,
                    selector,
                    source.Token);
                projectFacade.ActiveProject().History.Mark();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivateDatasetCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="selector">
        ///     The function that is used to select the most suitable machine to distribute the dataset onto.
        /// </param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "To select an appropriate machine we need a function which requires nested generics.")]
        public ActivateDatasetCommand(
            ILinkToProjects projectFacade, 
            DatasetFacade dataset,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector,
            Func<string, IDisposable> timer)
            : base(obj => OnActivate(projectFacade, dataset, selector, timer), obj => CanActivate(dataset))
        {
        }
    }
}
