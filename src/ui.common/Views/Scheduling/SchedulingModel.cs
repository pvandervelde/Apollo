//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Common.Utilities;
using ICommand = System.Windows.Input.ICommand;

namespace Apollo.UI.Common.Views.Scheduling
{
    /// <summary>
    /// Stores information about the available schedules.
    /// </summary>
    public sealed class SchedulingModel : Model
    {
        /// <summary>
        /// The object that provides information about the available schedules.
        /// </summary>
        private readonly SchedulingFacade m_Schedules;

        /// <summary>
        /// The collection that stores all the action models.
        /// </summary>
        private readonly ObservableCollection<ScheduleActionModel> m_ActionModels;

        /// <summary>
        /// The collection that stores all the condition models.
        /// </summary>
        private readonly ObservableCollection<ScheduleConditionModel> m_ConditionModels;

        /// <summary>
        /// The collection that stores all the schedule models.
        /// </summary>
        private readonly ObservableCollection<ScheduleModel> m_ScheduleModels;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="schedules">The object that provides information about the available schedules.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedules"/> is <see langword="null" />.
        /// </exception>
        public SchedulingModel(IContextAware context, SchedulingFacade schedules)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => schedules);
            }

            m_Schedules = schedules;
            {
                m_Schedules.OnScheduleAdded += HandleOnScheduleAdded;
                m_Schedules.OnScheduleUpdated += HandleOnScheduleUpdated;

                m_Schedules.OnScheduleActionAdded += HandleOnScheduleActionAdded;
                m_Schedules.OnScheduleActionUpdated += HandleOnScheduleActionUpdated;
                m_Schedules.OnScheduleActionRemoved += HandleOnScheduleActionRemoved;

                m_Schedules.OnScheduleConditionAdded += HandleOnScheduleConditionAdded;
                m_Schedules.OnScheduleConditionUpdated += HandleOnScheduleConditionUpdated;
                m_Schedules.OnScheduleConditionRemoved += HandleOnScheduleConditionRemoved;
            }
            
            m_ActionModels = new ObservableCollection<ScheduleActionModel>(
                m_Schedules.Actions.Select(f => new ScheduleActionModel(context, f)));
            m_ConditionModels = new ObservableCollection<ScheduleConditionModel>(
                m_Schedules.Conditions.Select(f => new ScheduleConditionModel(context, f)));
            m_ScheduleModels = new ObservableCollection<ScheduleModel>(
                m_Schedules.Schedules.Select(f => new ScheduleModel(context, f)));
        }

        private void HandleOnScheduleAdded(object sender, ScheduleEventArgs e)
        {
            m_ScheduleModels.Add(new ScheduleModel(InternalContext, m_Schedules.Schedule(e.Schedule)));
        }

        private void HandleOnScheduleUpdated(object sender, ScheduleEventArgs e)
        {
            int index = m_ScheduleModels.FindIndex<ScheduleModel>(m => m.Id.Equals(e.Schedule));
            if (index > -1)
            {
                m_ScheduleModels[index] = new ScheduleModel(InternalContext, m_Schedules.Schedule(e.Schedule));
            }
        }

        private void HandleOnScheduleActionAdded(object sender, ScheduleElementEventArgs e)
        {
            m_ActionModels.Add(new ScheduleActionModel(InternalContext, m_Schedules.Action(e.ScheduleElement)));
        }

        private void HandleOnScheduleActionUpdated(object sender, ScheduleElementEventArgs e)
        {
            int index = m_ActionModels.FindIndex<ScheduleActionModel>(m => m.Id.Equals(e.ScheduleElement));
            if (index > -1)
            {
                m_ActionModels[index] = new ScheduleActionModel(InternalContext, m_Schedules.Action(e.ScheduleElement));
            }
        }

        private void HandleOnScheduleActionRemoved(object sender, ScheduleElementEventArgs e)
        {
            int index = m_ActionModels.FindIndex<ScheduleActionModel>(m => m.Id.Equals(e.ScheduleElement));
            if (index > -1)
            {
                m_ActionModels.RemoveAt(index);
            }
        }

        private void HandleOnScheduleConditionAdded(object sender, ScheduleElementEventArgs e)
        {
            m_ConditionModels.Add(new ScheduleConditionModel(InternalContext, m_Schedules.Condition(e.ScheduleElement)));
        }

        private void HandleOnScheduleConditionUpdated(object sender, ScheduleElementEventArgs e)
        {
            int index = m_ConditionModels.FindIndex<ScheduleConditionModel>(m => m.Id.Equals(e.ScheduleElement));
            if (index > -1)
            {
                m_ConditionModels[index] = new ScheduleConditionModel(InternalContext, m_Schedules.Condition(e.ScheduleElement));
            }
        }

        private void HandleOnScheduleConditionRemoved(object sender, ScheduleElementEventArgs e)
        {
            int index = m_ConditionModels.FindIndex<ScheduleConditionModel>(m => m.Id.Equals(e.ScheduleElement));
            if (index > -1)
            {
                m_ConditionModels.RemoveAt(index);
            }
        }

        /// <summary>
        /// Gets the collection that stores all the available schedule actions.
        /// </summary>
        public ObservableCollection<ScheduleActionModel> Actions
        {
            get
            {
                return m_ActionModels;
            }
        }

        /// <summary>
        /// Gets the collection that stores all the available schedule conditions.
        /// </summary>
        public ObservableCollection<ScheduleConditionModel> Conditions
        {
            get
            {
                return m_ConditionModels;
            }
        }

        /// <summary>
        /// Gets the collection that contains all the schedules.
        /// </summary>
        public ObservableCollection<ScheduleModel> Schedules
        {
            get
            {
                return m_ScheduleModels;
            }
        }

        /// <summary>
        /// Gets or sets the command that is used to create a new schedule.
        /// </summary>
        public ICommand AddScheduleCommand
        {
            get;
            set;
        }
    }
}
