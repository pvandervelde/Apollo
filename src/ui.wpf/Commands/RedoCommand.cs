//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Microsoft.Practices.Prism.Commands;
using Nuclei.Diagnostics.Profiling;

namespace Apollo.UI.Wpf.Commands
{
    /// <summary>
    /// Handles the redoing of the last undo action.
    /// </summary>
    public sealed class RedoCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if it is possible to redo the last undo action.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction with
        /// the project system.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the last undo action can be redone; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanRedo(ILinkToProjects projectFacade)
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
            if (project.History.CanRollForward)
            {
                var markers = project.History.MarkersInTheFuture();

                var markerToRollBackTo = markers.FirstOrDefault(m => !m.Equals(project.History.Current));
                return markerToRollBackTo != null;
            }

            return false;
        }
        
        /// <summary>
        /// Called when the last undo action should be redone.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction with
        /// the project system.
        /// </param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        private static void OnRedo(ILinkToProjects projectFacade, Func<string, IDisposable> timer)
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

            using (var interval = timer("Redoing change"))
            {
                var project = projectFacade.ActiveProject();
                if (project.History.CanRollForward)
                {
                    var markers = project.History.MarkersInTheFuture();

                    var markerToRollBackTo = markers.FirstOrDefault(m => !m.Equals(project.History.Current));
                    if (markerToRollBackTo != null)
                    {
                        project.History.RollForwardTo(markerToRollBackTo);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedoCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction
        /// with the project system.
        /// </param>
        /// <param name="timer">The function that creates and stores timing intervals.</param>
        public RedoCommand(ILinkToProjects projectFacade, Func<string, IDisposable> timer)
            : base(obj => OnRedo(projectFacade, timer), obj => CanRedo(projectFacade))
        { 
        }
    }
}
