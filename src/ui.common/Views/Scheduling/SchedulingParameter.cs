//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Common.Views.Scheduling
{
    /// <summary>
    /// A parameter for the scheduling view.
    /// </summary>
    public sealed class SchedulingParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public SchedulingParameter(IContextAware context)
            : base(context)
        {
        }
    }
}
