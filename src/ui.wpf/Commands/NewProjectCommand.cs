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
using Nuclei.Diagnostics.Profiling;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// Handles the creation of new projects.
    /// </summary>
    public sealed class NewProjectCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if a new project can be created.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction with
        /// the project system.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if a new project can be created; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanCreateNewProject(ILinkToProjects projectFacade)
        {
            // If there is no project facade, then we're in 
            // designer mode, or something else silly.
            if (projectFacade == null)
            {
                return false;
            }

            return projectFacade.CanCreateNewProject();
        }
        
        /// <summary>
        /// Called when the creation of a new project is required.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction with
        /// the project system.
        /// </param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnCreateNewProject(ILinkToProjects projectFacade, Func<string, IDisposable> timer)
        {
            // If there is no project facade, then we're in 
            // designer mode, or something else silly.
            if (projectFacade == null)
            {
                return;
            }

            using (var interval = timer("Creating new project"))
            {
                projectFacade.NewProject();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction
        /// with the project system.
        /// </param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public NewProjectCommand(ILinkToProjects projectFacade, Func<string, IDisposable> timer)
            : base(obj => OnCreateNewProject(projectFacade, timer), obj => CanCreateNewProject(projectFacade))
        { 
        }
    }
}
