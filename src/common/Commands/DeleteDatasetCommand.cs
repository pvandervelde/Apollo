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
    /// Handles deletion of a dataset.
    /// </summary>
    public sealed class DeleteDatasetCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if a dataset can be deleted.
        /// </summary>
        /// <param name="datasetFacade">
        /// The object that contains the methods that allow interaction with
        /// a dataset.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the existing project can be closed; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanDeleteDataset(DatasetFacade datasetFacade)
        {
            // If there is no dataset facade, then we're in 
            // designer mode, or something else silly.
            if (datasetFacade == null)
            {
                return false;
            }

            return datasetFacade.IsValid ? datasetFacade.CanBeDeleted : false;
        }
        
        /// <summary>
        /// Called when the dataset should be deleted.
        /// </summary>
        /// <param name="datasetFacade">
        /// The object that contains the methods that allow interaction with
        /// a dataset.
        /// </param>
        private static void OnDeleteDataset(DatasetFacade datasetFacade)
        {
            // If there is no dataset facade, then we're in 
            // designer mode, or something else silly.
            if (datasetFacade == null)
            {
                return;
            }

            // Store the current state of the system, just after load.
            // projectFacade.ActiveProject().History.RemoveFromTimeline(datasetFacade.HistoryId);
            // projectFacade.ActiveProject().History.Mark();
            datasetFacade.Delete();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteDatasetCommand"/> class.
        /// </summary>
        /// <param name="datasetFacade">
        /// The object that contains the methods that allow interaction with
        /// a dataset.
        /// </param>
        public DeleteDatasetCommand(DatasetFacade datasetFacade)
            : base(obj => OnDeleteDataset(datasetFacade), obj => CanDeleteDataset(datasetFacade))
        { 
        }
    }
}
