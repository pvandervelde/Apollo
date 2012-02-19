//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Host.UserInterfaces.Projects;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles the undoing of the last action.
    /// </summary>
    public sealed class UndoCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if it is possible to undo the last action.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction with
        /// the project system.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the last action can be undone; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanUndo(ILinkToProjects projectFacade)
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
            if (project.History.CanRollBack)
            {
                var markers = project.History.MarkersInThePast();

                var markerToRollBackTo = markers.FirstOrDefault(m => !m.Equals(project.History.Current));
                return markerToRollBackTo != null;
            }

            return false;
        }
        
        /// <summary>
        /// Called when the last action should be undone.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction with
        /// the project system.
        /// </param>
        private static void OnUndo(ILinkToProjects projectFacade)
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

            var project = projectFacade.ActiveProject();
            if (project.History.CanRollBack)
            {
                var markers = project.History.MarkersInThePast();

                var markerToRollBackTo = markers.FirstOrDefault(m => !m.Equals(project.History.Current));
                if (markerToRollBackTo != null)
                {
                    project.History.RollBackTo(markerToRollBackTo);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction
        /// with the project system.
        /// </param>
        public UndoCommand(ILinkToProjects projectFacade)
            : base(obj => OnUndo(projectFacade), obj => CanUndo(projectFacade))
        { 
        }
    }
}
