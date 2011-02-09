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
    /// Handles the closing of the existing project.
    /// </summary>
    public sealed class CloseProjectCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the existing project can be closed.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction with
        /// the project system.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the existing project can be closed; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanCloseProject(ILinkToProjects projectFacade)
        {
            // If there is no project facade, then we're in 
            // designer mode, or something else silly.
            if (projectFacade == null)
            {
                return false;
            }

            return projectFacade.CanUnloadProject();
        }
        
        /// <summary>
        /// Called when the existing project should be closed.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction with
        /// the project system.
        /// </param>
        private static void OnCloseProject(ILinkToProjects projectFacade)
        {
            // If there is no project facade, then we're in 
            // designer mode, or something else silly.
            if (projectFacade == null)
            {
                return;
            }

            projectFacade.UnloadProject();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseProjectCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction
        /// with the project system.
        /// </param>
        public CloseProjectCommand(ILinkToProjects projectFacade)
            : base(obj => OnCloseProject(projectFacade), obj => CanCloseProject(projectFacade))
        { 
        }
    }
}
