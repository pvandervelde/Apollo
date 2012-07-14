//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Input;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.UserInterfaces.Projects;
using ICommand = System.Windows.Input.ICommand;

namespace Apollo.UI.Common.Views.Scheduling
{
    /// <summary>
    /// Stores information about a schedule.
    /// </summary>
    public sealed class ScheduleModel : Model
    {
        /// <summary>
        /// The facade that provides information about the selected schedule.
        /// </summary>
        private readonly ScheduleFacade m_Schedule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="schedule">The object that provides information about the selected schedule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        public ScheduleModel(IContextAware context, ScheduleFacade schedule)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => schedule);
            }

            m_Schedule = schedule;
        }

        /// <summary>
        /// Gets the ID of the schedule.
        /// </summary>
        public ScheduleId Id
        {
            get
            {
                return m_Schedule.Id;
            }
        }

        /// <summary>
        /// Gets or sets the name of the schedule.
        /// </summary>
        public string Name
        {
            get 
            {
                return m_Schedule.Name;
            }

            set
            {
                m_Schedule.Name = value;
                Notify(() => Name);
            }
        }

        /// <summary>
        /// Gets or sets the summary of the schedule.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Schedule.Summary;
            }

            set
            {
                m_Schedule.Summary = value;
                Notify(() => Summary);
            }
        }

        /// <summary>
        /// Gets or sets the command that is used to display the detail view.
        /// </summary>
        public ICommand ShowDetailViewCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command that copies the current schedule.
        /// </summary>
        public ICommand CopyCommand
        {
            get;
            set;
        }
    }
}
