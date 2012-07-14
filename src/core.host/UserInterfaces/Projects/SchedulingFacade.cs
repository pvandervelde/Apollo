//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Host.UserInterfaces.Projects
{
    /// <summary>
    /// Defines the facade that is used for interaction with the scheduling classes.
    /// </summary>
    public sealed class SchedulingFacade
    {
        /// <summary>
        /// Adds a new schedule to the storage and provides an information object
        /// that describes the new schedule.
        /// </summary>
        /// <param name="schedule">The new schedule.</param>
        /// <param name="name">The name of the schedule.</param>
        /// <param name="summary">The summary of the schedule.</param>
        /// <param name="description">The description of the schedule.</param>
        /// <param name="produces">The variables that are affected when the schedule is executed.</param>
        /// <param name="dependencies">The dependencies that are required for the execution of the schedule.</param>
        /// <returns>The ID of the schedule.</returns>
        public ScheduleId AddNewSchedule(
            IEditableSchedule schedule,
            string name,
            string summary,
            string description,
            IEnumerable<IScheduleVariable> produces,
            IEnumerable<IScheduleDependency> dependencies)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces the schedule current stored against the given ID with a new one.
        /// </summary>
        /// <param name="scheduleToUpdate">The ID of the schedule that should be replaced.</param>
        /// <param name="newSchedule">The new schedule.</param>
        public void Update(ScheduleId scheduleToUpdate, IEditableSchedule newSchedule)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts the execution of the given schedule.
        /// </summary>
        /// <param name="scheduleToExecute">The ID of the schedule that should be executed.</param>
        public void Start(ScheduleId scheduleToExecute)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pauses the execution of the given schedule.
        /// </summary>
        /// <param name="scheduleToPause">The ID of the schedule that should be executed.</param>
        public void Pause(ScheduleId scheduleToPause)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the execution of the given schedule.
        /// </summary>
        /// <param name="scheduleToStop">The ID of the schedule that should be executed.</param>
        public void Stop(ScheduleId scheduleToStop)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the facade for the schedule with the given ID.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <returns>The facade for the schedule with the given ID.</returns>
        public ScheduleFacade Schedule(ScheduleId id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the collection of currently available schedules.
        /// </summary>
        public IEnumerable<ScheduleFacade> Schedules
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// An event raised if a schedule is added to the collection.
        /// </summary>
        public event EventHandler<ScheduleEventArgs> OnScheduleAdded;

        private void RaiseOnScheduleAdded(ScheduleId id)
        {
            var local = OnScheduleAdded;
            if (local != null)
            {
                local(this, new ScheduleEventArgs(id));
            }
        }

        /// <summary>
        /// An event raised if a schedule is updated.
        /// </summary>
        public event EventHandler<ScheduleEventArgs> OnScheduleUpdated;

        private void RaiseOnScheduleUpdated(ScheduleId id)
        {
            var local = OnScheduleUpdated;
            if (local != null)
            {
                local(this, new ScheduleEventArgs(id));
            }
        }

        /// <summary>
        /// Returns the facade for the action with the given ID.
        /// </summary>
        /// <param name="id">The ID of the schedule action.</param>
        /// <returns>The facade for the action with the given ID.</returns>
        public ScheduleActionFacade Action(ScheduleElementId id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the collection of currently available schedule actions.
        /// </summary>
        public IEnumerable<ScheduleActionFacade> Actions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// An event raised if a new schedule action is added.
        /// </summary>
        public event EventHandler<ScheduleElementEventArgs> OnScheduleActionAdded;

        private void RaiseOnScheduleActionAdded(ScheduleElementId id)
        {
            var local = OnScheduleActionAdded;
            if (local != null)
            {
                local(this, new ScheduleElementEventArgs(id));
            }
        }

        /// <summary>
        /// An event raised if a schedule action is updated.
        /// </summary>
        public event EventHandler<ScheduleElementEventArgs> OnScheduleActionUpdated;

        private void RaiseOnScheduleActionUpdated(ScheduleElementId id)
        {
            var local = OnScheduleActionUpdated;
            if (local != null)
            {
                local(this, new ScheduleElementEventArgs(id));
            }
        }

        /// <summary>
        /// An event raised if a schedule action is deleted.
        /// </summary>
        public event EventHandler<ScheduleElementEventArgs> OnScheduleActionRemoved;

        private void RaiseOnScheduleActionRemoved(ScheduleElementId id)
        {
            var local = OnScheduleActionRemoved;
            if (local != null)
            {
                local(this, new ScheduleElementEventArgs(id));
            }
        }

        /// <summary>
        /// Returns the facade for the condition with the given ID.
        /// </summary>
        /// <param name="id">The ID of the schedule condition.</param>
        /// <returns>The facade for the condition with the given ID.</returns>
        public ScheduleConditionFacade Condition(ScheduleElementId id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the collection of currently available schedule conditions.
        /// </summary>
        public IEnumerable<ScheduleConditionFacade> Conditions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// An event raised if a new schedule condition is added.
        /// </summary>
        public event EventHandler<ScheduleElementEventArgs> OnScheduleConditionAdded;

        private void RaiseOnScheduleConditionAdded(ScheduleElementId id)
        {
            var local = OnScheduleConditionAdded;
            if (local != null)
            {
                local(this, new ScheduleElementEventArgs(id));
            }
        }

        /// <summary>
        /// An event raised if a schedule condition is updated.
        /// </summary>
        public event EventHandler<ScheduleElementEventArgs> OnScheduleConditionUpdated;

        private void RaiseOnScheduleConditionUpdated(ScheduleElementId id)
        {
            var local = OnScheduleConditionUpdated;
            if (local != null)
            {
                local(this, new ScheduleElementEventArgs(id));
            }
        }

        /// <summary>
        /// An event raised if a schedule condition is deleted.
        /// </summary>
        public event EventHandler<ScheduleElementEventArgs> OnScheduleConditionRemoved;

        private void RaiseOnScheduleConditionRemoved(ScheduleElementId id)
        {
            var local = OnScheduleConditionRemoved;
            if (local != null)
            {
                local(this, new ScheduleElementEventArgs(id));
            }
        }
    }
}
