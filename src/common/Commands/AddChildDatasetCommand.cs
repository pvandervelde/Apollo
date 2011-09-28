﻿//-----------------------------------------------------------------------
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
    /// Handles adding a new child dataset.
    /// </summary>
    public sealed class AddChildDatasetCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if a new child dataset can be added.
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
        /// Called when the existing project should be closed.
        /// </summary>
        /// <param name="datasetFacade">
        /// The object that contains the methods that allow interaction with
        /// a dataset.
        /// </param>
        private static void OnAddNewChild(DatasetFacade datasetFacade)
        {
            // If there is no dataset facade, then we're in 
            // designer mode, or something else silly.
            if (datasetFacade == null)
            {
                return;
            }

            datasetFacade.AddChild();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddChildDatasetCommand"/> class.
        /// </summary>
        /// <param name="datasetFacade">
        /// The object that contains the methods that allow interaction with
        /// a dataset.
        /// </param>
        public AddChildDatasetCommand(DatasetFacade datasetFacade)
            : base(obj => OnAddNewChild(datasetFacade), obj => CanAddNewChild(datasetFacade))
        { 
        }
    }
}