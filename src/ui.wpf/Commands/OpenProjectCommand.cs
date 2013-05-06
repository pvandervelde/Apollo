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
    /// Handles the loading of projects.
    /// </summary>
    public sealed class OpenProjectCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if a project can be loaded.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction
        /// with the project system.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if a project can be loaded; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanLoadProject(ILinkToProjects projectFacade)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (projectFacade == null)
            {
                return false;
            }

            return projectFacade.CanLoadProject();
        }

        /// <summary>
        /// Loads a new project.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="persistenceInformation">The object that describes how the project was persisted.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnLoadProject(
            ILinkToProjects projectFacade, 
            IPersistenceInformation persistenceInformation, 
            Func<string, IDisposable> timer)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (projectFacade == null)
            {
                return;
            }

            using (var interval = timer("Loading project"))
            {
                projectFacade.LoadProject(persistenceInformation);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenProjectCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public OpenProjectCommand(ILinkToProjects projectFacade, Func<string, IDisposable> timer)
            : base(obj => OnLoadProject(projectFacade, obj as IPersistenceInformation, timer), obj => CanLoadProject(projectFacade))
        { 
        }
    }
}
