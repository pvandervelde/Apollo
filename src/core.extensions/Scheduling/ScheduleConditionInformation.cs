//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.Extensions.Scheduling
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
        /// The summary of the condition that is being described by this information object.
        /// </summary>
        private readonly string m_Summary;

        /// <summary>
        /// The description of the condition that is being described by this information object.
        /// </summary>
        private readonly string m_Description;

        /// <summary>
        /// The collection of dependencies that are required to evaluate the condition being described by this information object.
        /// </summary>
        private readonly IEnumerable<IScheduleDependency> m_Dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleConditionInformation"/> class.
        /// </summary>
        /// <param name="id">The ID of the condition that is being described by this information object.</param>
        /// <param name="name">The name of the condition that is being described by this information object.</param>
        /// <param name="summary">The summary of the condition that is being described by this information object.</param>
        /// <param name="description">The description of the condition that is being described by this information object.</param>
        /// <param name="dependencies">
        /// The collection of dependencies that are required to evaluate the condition described by this information object.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dependencies"/> is <see langword="null" />.
        /// </exception>
        public ScheduleConditionInformation(
            ScheduleElementId id,
            string name, 
            string summary,
            string description,
            IEnumerable<IScheduleDependency> dependencies)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => dependencies);
            }

            m_Id = id;
            m_Name = name;
            m_Summary = summary;
            m_Description = description;
            m_Dependencies = dependencies;
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
        /// Gets the summary of the condition.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Summary;
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

        // Type identifier? --> Describes in code what the condition does? Or maybe where it'll fit (flags?)

        /// <summary>
        /// Returns the collection of dependencies that have to be in place for the current schedule
        /// condition to be evaluated.
        /// </summary>
        /// <returns>
        /// The collection of dependencies for the current schedule condition.
        /// </returns>
        public IEnumerable<IScheduleDependency> DependsOn()
        {
            return m_Dependencies;
        }
    }
}
