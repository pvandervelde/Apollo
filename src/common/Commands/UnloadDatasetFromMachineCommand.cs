//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.UserInterfaces.Projects;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles the unloading of a dataset from a machine.
    /// </summary>
    public sealed class UnloadDatasetFromMachineCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines whether the dataset can be unloaded from a machine..
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>
        ///     <see langword="true"/> if the dataset can be unloaded from a machine; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanUnload(DatasetFacade dataset)
        {
            // If there is no application facade, then we're in 
            // designer mode, or something else silly.
            if (dataset == null)
            {
                return false;
            }

            return dataset.IsLoaded;
        }

        /// <summary>
        /// Called when the dataset should be unloaded.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        private static void OnUnload(DatasetFacade dataset)
        {
            // If there is no application facade, then we're in 
            // designer mode, or something else silly.
            if (dataset == null)
            {
                return;
            }

            if (!dataset.IsLoaded)
            {
                return;
            }

            dataset.UnloadFromMachine();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadDatasetFromMachineCommand"/> class.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        public UnloadDatasetFromMachineCommand(DatasetFacade dataset)
            : base(obj => OnUnload(dataset), obj => CanUnload(dataset))
        {
        }
    }
}
