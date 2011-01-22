//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.UserInterfaces.Projects;
using Apollo.Utils;
using Microsoft.Practices.Composite.Presentation.Commands;

namespace Apollo.UI.Common.Commands
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
        private static void OnLoadProject(ILinkToProjects projectFacade, IPersistenceInformation persistenceInformation)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (projectFacade == null)
            {
                return;
            }

            projectFacade.LoadProject(persistenceInformation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenProjectCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">The object that contains the methods that allow interaction with the project system.</param>
        public OpenProjectCommand(ILinkToProjects projectFacade)
            : base(obj => OnLoadProject(projectFacade, obj as IPersistenceInformation), obj => CanLoadProject(projectFacade))
        { 
        }
    }
}
