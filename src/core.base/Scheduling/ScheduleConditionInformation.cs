//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Base.Scheduling
{
    /// <summary>
    /// Stores information about a given schedule condition.
    /// </summary>
    public sealed class ScheduleConditionInformation
    {
        /// <summary>
        /// The ID of the condition that is being described by this information object.
        /// </summary>
        private readonly ScheduleElementId m_Id;

        /// <summary>
        /// The name of the condition that is being described by this information object.
        /// </summary>
        private readonly string m_Name;
        
        /// <summary>
        /// The description of the condition that is being described by this information object.
        /// </summary>
        private readonly string m_Description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleConditionInformation"/> class.
        /// </summary>
        /// <param name="id">The ID of the condition that is being described by this information object.</param>
        /// <param name="name">The name of the condition that is being described by this information object.</param>
        /// <param name="description">The description of the condition that is being described by this information object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        public ScheduleConditionInformation(
            ScheduleElementId id,
            string name, 
            string description)
        {
            {
                Lokad.Enforce.Argument(() => id);
            }

            m_Id = id;
            m_Name = name;
            m_Description = description;
        }

        /// <summary>
        /// Gets the ID for the condition.
        /// </summary>
        public ScheduleElementId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the name of the condition.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the description of the condition.
        /// </summary>
        public string Description
        {
            get
            {
                return m_Description;
            }
        }
    }
}
