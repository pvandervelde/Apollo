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
    /// Defines the facade that is used for interaction with a schedule condition.
    /// </summary>
    public sealed class ScheduleConditionFacade
    {
        /// <summary>
        /// The scene that owns the current schedule.
        /// </summary>
        private readonly SceneFacade m_Owner;

        /// <summary>
        /// The object that provides the information about the condition.
        /// </summary>
        private readonly IHoldSceneScheduleConditionData m_Condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleConditionFacade"/> class.
        /// </summary>
        /// <param name="owner">The scene that owns the current schedule.</param>
        /// <param name="condition">The object that provides the information about the condition.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="condition"/> is <see langword="null" />.
        /// </exception>
        internal ScheduleConditionFacade(SceneFacade owner, IHoldSceneScheduleConditionData condition)
        {
            {
                Lokad.Enforce.Argument(() => owner);
                Lokad.Enforce.Argument(() => condition);
            }

            m_Owner = owner;
            m_Condition = condition;
        }

        /// <summary>
        /// Gets the ID of the condition.
        /// </summary>
        public ScheduleElementId Id
        {
            get
            {
                return m_Condition.Id;
            }
        }
    }
}
