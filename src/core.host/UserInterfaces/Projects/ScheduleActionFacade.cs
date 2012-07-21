//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.Projects;

namespace Apollo.Core.Host.UserInterfaces.Projects
{
    /// <summary>
    /// Defines the facade that is used for interaction with a schedule action.
    /// </summary>
    public sealed class ScheduleActionFacade
    {
        /// <summary>
        /// The scene that owns the current schedule.
        /// </summary>
        private readonly SceneFacade m_Owner;

        /// <summary>
        /// The object that provides the information about the schedule action.
        /// </summary>
        private readonly IHoldSceneScheduleActionData m_Action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleActionFacade"/> class.
        /// </summary>
        /// <param name="owner">The scene that owns the current schedule.</param>
        /// <param name="action">The object that provides the information about the schedule action.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="action"/> is <see langword="null" />.
        /// </exception>
        internal ScheduleActionFacade(SceneFacade owner, IHoldSceneScheduleActionData action)
        {
            {
                Lokad.Enforce.Argument(() => owner);
                Lokad.Enforce.Argument(() => action);
            }

            m_Owner = owner;
            m_Action = action;
        }

        /// <summary>
        /// Gets the ID of the action.
        /// </summary>
        public ScheduleElementId Id
        {
            get
            {
                return m_Action.Id;
            }
        }
    }
}
