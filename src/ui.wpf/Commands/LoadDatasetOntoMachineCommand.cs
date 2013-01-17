//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Microsoft.Practices.Prism.Commands;
using Utilities.Diagnostics.Profiling;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// Handles the loading of a dataset onto a machine.
    /// </summary>
    public sealed class LoadDatasetOntoMachineCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines whether the dataset can be loaded onto a machine..
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>
        ///     <see langword="true"/> if the dataset can be loaded onto a machine; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanLoad(DatasetFacade dataset)
        {
            // If there is no application facade, then we're in 
            // designer mode, or something else silly.
            if (dataset == null)
            {
                return false;
            }

            return !dataset.IsLoaded && dataset.CanLoad;
        }

        /// <summary>
        /// Called when the dataset should be loaded.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="selector">
        ///     The function that is used to select the most suitable machine to load the dataset onto.
        /// </param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnLoad(
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

            if (dataset.IsLoaded || !dataset.CanLoad)
            {
                return;
            }

            using (var interval = timer("Loading dataset onto machine"))
            {
                var source = new CancellationTokenSource();
                dataset.LoadOntoMachine(
                    LoadingLocations.All,
                    selector,
                    source.Token);
                projectFacade.ActiveProject().History.Mark();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadDatasetOntoMachineCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="selector">
        ///     The function that is used to select the most suitable machine to load the dataset onto.
        /// </param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "To select an appropriate machine we need a function which requires nested generics.")]
        public LoadDatasetOntoMachineCommand(
            ILinkToProjects projectFacade, 
            DatasetFacade dataset,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector,
            Func<string, IDisposable> timer)
            : base(obj => OnLoad(projectFacade, dataset, selector, timer), obj => CanLoad(dataset))
        {
        }
    }
}
