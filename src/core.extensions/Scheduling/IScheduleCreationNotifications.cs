//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Provides notifications about the creation of a schedule.
    /// </summary>
    public interface IScheduleCreationNotifications
    {
        /// <summary>
        /// An event raised when a schedule has been created.
        /// </summary>
        event EventHandler<ScheduleEventArgs> OnCreated;

        /// <summary>
        /// An event raised when a schedule has been updated.
        /// </summary>
        event EventHandler<ScheduleEventArgs> OnUpdated;
    }
}
