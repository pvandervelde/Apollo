//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Common.Properties;
using Apollo.Utilities;
using Microsoft.Practices.Prism.Commands;
using NManto;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles adding a new child dataset.
    /// </summary>
    public sealed class AddChildDatasetCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if a new child dataset can be added.
        /// </summary>
        /// <param name="datasetFacade">The object that contains the methods that allow interaction with a dataset.</param>
        /// <returns>
        ///     <see langword="true"/> if a new child dataset can be added; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanAddNewChild(DatasetFacade datasetFacade)
        {
            // If there is no dataset facade, then we're in 
            // designer mode, or something else silly.
            if (datasetFacade == null)
            {
                return false;
            }

            return datasetFacade.IsValid ? datasetFacade.CanBecomeParent : false;
        }
        
        /// <summary>
        /// Called when a new child dataset should be added.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="datasetFacade">The object that contains the methods that allow interaction with a dataset.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnAddNewChild(ILinkToProjects projectFacade, DatasetFacade datasetFacade, Func<string, IDisposable> timer)
        {
            // If there is no dataset facade, then we're in 
            // designer mode, or something else silly.
            if (datasetFacade == null)
            {
                return;
            }

            using (var interval = timer("Add dataset to graph"))
            {
                datasetFacade.AddChild();
                projectFacade.ActiveProject().History.Mark();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddChildDatasetCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="datasetFacade">The object that contains the methods that allow interaction with a dataset.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public AddChildDatasetCommand(ILinkToProjects projectFacade, DatasetFacade datasetFacade, Func<string, IDisposable> timer)
            : base(obj => OnAddNewChild(projectFacade, datasetFacade, timer), obj => CanAddNewChild(datasetFacade))
        { 
        }
    }
}
