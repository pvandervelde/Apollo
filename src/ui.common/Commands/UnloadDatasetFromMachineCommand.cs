//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Microsoft.Practices.Prism.Commands;
using NManto;

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
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnUnload(ILinkToProjects projectFacade, DatasetFacade dataset, Func<string, IDisposable> timer)
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

            using (var interval = timer("Unloading dataset"))
            {
                dataset.UnloadFromMachine();
                projectFacade.ActiveProject().History.Mark();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadDatasetFromMachineCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public UnloadDatasetFromMachineCommand(ILinkToProjects projectFacade, DatasetFacade dataset, Func<string, IDisposable> timer)
            : base(obj => OnUnload(projectFacade, dataset, timer), obj => CanUnload(dataset))
        {
        }
    }
}
