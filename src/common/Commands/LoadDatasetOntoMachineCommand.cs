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
using Apollo.Core.UserInterfaces.Projects;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Common.Commands
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
        /// <param name="dataset">The dataset.</param>
        /// <param name="selector">
        ///     The function that is used to select the most suitable machine to load the dataset onto.
        /// </param>
        private static void OnLoad(
            DatasetFacade dataset,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector)
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

            // the only catch is that the Shutdown method will return before
            // we know if the shutdown will be canceled?
            var source = new CancellationTokenSource();
            dataset.LoadOntoMachine(
                LoadingLocations.All,
                selector,
                source.Token);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadDatasetOntoMachineCommand"/> class.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="selector">
        ///     The function that is used to select the most suitable machine to load the dataset onto.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "To select an appropriate machine we need a function which requires nested generics.")]
        public LoadDatasetOntoMachineCommand(
            DatasetFacade dataset,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector)
            : base(obj => OnLoad(dataset, selector), obj => CanLoad(dataset))
        {
        }
    }
}
