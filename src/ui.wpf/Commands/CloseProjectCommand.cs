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
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnCloseProject(ILinkToProjects projectFacade, Func<string, IDisposable> timer)
        {
            // If there is no project facade, then we're in 
            // designer mode, or something else silly.
            if (projectFacade == null)
            {
                return;
            }

            using (var interval = timer("Unloading project"))
            {
                projectFacade.UnloadProject();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseProjectCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction
        /// with the project system.
        /// </param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public CloseProjectCommand(ILinkToProjects projectFacade, Func<string, IDisposable> timer)
            : base(obj => OnCloseProject(projectFacade, timer), obj => CanCloseProject(projectFacade))
        { 
        }
    }
}
