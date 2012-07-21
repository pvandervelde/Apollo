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
    /// Stores information about a given schedule action.
    /// </summary>
    public sealed class ScheduleActionModel : Model
    {
        /// <summary>
        /// The object that provides information about the given schedule action.
        /// </summary>
        private readonly ScheduleActionFacade m_Action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleActionModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="action">The object that provides information about the given schedule action.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="action"/> is <see langword="null" />.
        /// </exception>
        public ScheduleActionModel(IContextAware context, ScheduleActionFacade action)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => action);
            }

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
