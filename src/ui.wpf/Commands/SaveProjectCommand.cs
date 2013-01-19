//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Wpf.Properties;
using Apollo.Utilities;
using Microsoft.Practices.Prism.Commands;
using Utilities.Diagnostics.Profiling;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// Handles the saving of the current project.
    /// </summary>
    public sealed class SaveProjectCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the existing project should be saved.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction
        /// with the project system.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the existing project should be saved; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool ShouldSaveProject(ILinkToProjects projectFacade)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (projectFacade == null)
            {
                return false;
            }

            if (!projectFacade.HasActiveProject())
            {
                return false;
            }

            var project = projectFacade.ActiveProject();
            return project.ShouldSaveProject();
        }

        /// <summary>
        /// Saves the existing project.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="persistenceInformation">The object that describes how the project should be persisted.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnSaveProject(
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

            if (!projectFacade.HasActiveProject())
            {
                return;
            }

            using (var interval = timer("Saving project"))
            {
                var project = projectFacade.ActiveProject();
                project.SaveProject(persistenceInformation);

                projectFacade.ActiveProject().History.Mark(Resources.SaveProjectCommand_HistoryMark);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveProjectCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public SaveProjectCommand(ILinkToProjects projectFacade, Func<string, IDisposable> timer)
            : base(obj => OnSaveProject(projectFacade, obj as IPersistenceInformation, timer), obj => ShouldSaveProject(projectFacade))
        { 
        }
    }
}
