//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Wpf.Properties;
using Apollo.Utilities;
using Microsoft.Practices.Prism.Commands;
using Nuclei.Diagnostics.Profiling;

namespace Apollo.UI.Wpf.Commands
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
        ///     <see langword="true"/> if the dataset can be deleted; otherwise, <see langword="false"/>.
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
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="datasetFacade">The object that contains the methods that allow interaction with a dataset.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnDeleteDataset(ILinkToProjects projectFacade, DatasetFacade datasetFacade, Func<string, IDisposable> timer)
        {
            // If there is no dataset facade, then we're in 
            // designer mode, or something else silly.
            if (datasetFacade == null)
            {
                return;
            }

            using (var interval = timer("Deleting dataset"))
            {
                datasetFacade.Delete();
                projectFacade.ActiveProject().History.Mark();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteDatasetCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="datasetFacade">The object that contains the methods that allow interaction with a dataset.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public DeleteDatasetCommand(ILinkToProjects projectFacade, DatasetFacade datasetFacade, Func<string, IDisposable> timer)
            : base(obj => OnDeleteDataset(projectFacade, datasetFacade, timer), obj => CanDeleteDataset(datasetFacade))
        { 
        }
    }
}
