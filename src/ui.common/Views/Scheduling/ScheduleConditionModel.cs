//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.UserInterfaces.Projects;

namespace Apollo.UI.Common.Views.Scheduling
{
    /// <summary>
    /// Stores information about a given schedule condition.
    /// </summary>
    public sealed class ScheduleConditionModel : Model
    {
        /// <summary>
        /// The object that provides information about the given schedule condition.
        /// </summary>
        private readonly ScheduleConditionFacade m_Condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleConditionModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="condition">The object that provides information about the given schedule condition.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="condition"/> is <see langword="null" />.
        /// </exception>
        public ScheduleConditionModel(IContextAware context, ScheduleConditionFacade condition)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => condition);
            }

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
