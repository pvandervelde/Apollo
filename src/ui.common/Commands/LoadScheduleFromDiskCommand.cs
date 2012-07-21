//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles creating a new schedule.
    /// </summary>
    public sealed class LoadScheduleFromDiskCommand : DelegateCommand<object>
    {
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanCreateNewSchedule(SchedulingFacade schedulingFacade)
        {
            // If there is no dataset facade, then we're in 
            // designer mode, or something else silly.
            if (schedulingFacade == null)
            {
                return false;
            }

            // Check dataset can be edited?
            return true;
        }
        
        private static void OnCreateNewSchedule(SchedulingFacade schedulingFacade)
        {
            // If there is no dataset facade, then we're in 
            // designer mode, or something else silly.
            if (schedulingFacade == null)
            {
                return;
            }

            // Show dialog to edit schedule
            // Add at the end ..
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadScheduleFromDiskCommand"/> class.
        /// </summary>
        /// <param name="schedulingFacade">The object that contains the methods that allow interaction with the scheduling system.</param>
        public LoadScheduleFromDiskCommand(SchedulingFacade schedulingFacade)
            : base(obj => OnCreateNewSchedule(schedulingFacade), obj => CanCreateNewSchedule(schedulingFacade))
        { 
        }
    }
}
