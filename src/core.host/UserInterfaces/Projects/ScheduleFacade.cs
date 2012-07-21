//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Host.UserInterfaces.Projects
{
    /// <summary>
    /// Defines the facade for a schedule.
    /// </summary>
    public sealed class ScheduleFacade
    {
        /// <summary>
        /// Gets the ID of the schedule.
        /// </summary>
        public ScheduleId Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the dataset that owns the current schedule.
        /// </summary>
        public DatasetFacade Owner
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the name of the schedule.
        /// </summary>
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the summary of the schedule.
        /// </summary>
        public string Summary
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
