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
    /// Stores information about a given schedule.
    /// </summary>
    [Serializable]
    public sealed class ScheduleInformation
    {
        /// <summary>
        /// The ID of the schedule that is being described by this information object.
        /// </summary>
        private readonly ScheduleId m_Id;

        /// <summary>
        /// The name of the schedule that is being described by this information object.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// The summary of the schedule that is being described by this information object.
        /// </summary>
        private readonly string m_Summary;

        /// <summary>
        /// The description of the schedule that is being described by this information object.
        /// </summary>
        private readonly string m_Description;

        /// <summary>
        /// The collection of variables which are affected by the schedule that is being described by this
        /// information object.
        /// </summary>
        private readonly IEnumerable<IScheduleVariable> m_ProducedVariables;

        /// <summary>
        /// The collection of dependencies for the schedule that is being described by this information object.
        /// </summary>
        private readonly IEnumerable<IScheduleDependency> m_Dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleInformation"/> class.
        /// </summary>
        /// <param name="id">The ID of the schedule that is being described by this information object.</param>
        /// <param name="name">The name of the schedule that is being described by this information object.</param>
        /// <param name="summary">The summary of the schedule that is being described by this information object.</param>
        /// <param name="description">The description of the schedule that is being described by this information object.</param>
        /// <param name="produces">The collection of variables that is affected by the schedule described by this information object.</param>
        /// <param name="dependencies">The collection of dependencies for the schedule described by this information object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="produces"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dependencies"/> is <see langword="null" />.
        /// </exception>
        public ScheduleInformation(
            ScheduleId id,
            string name, 
            string summary,
            string description,
            IEnumerable<IScheduleVariable> produces,
            IEnumerable<IScheduleDependency> dependencies)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => produces);
                Lokad.Enforce.Argument(() => dependencies);
            }

            m_Id = id;
            m_Name = name;
            m_Summary = summary;
            m_Description = description;
            m_ProducedVariables = produces;
            m_Dependencies = dependencies;
        }

        /// <summary>
        /// Gets the ID for the schedule.
        /// </summary>
        public ScheduleId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the name of the schedule.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the summary of the schedule.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Summary;
            }
        }

        /// <summary>
        /// Gets the description of the schedule.
        /// </summary>
        public string Description
        {
            get
            {
                return m_Description;
            }
        }

        // Type identifier? --> Describes in code what the schedule does? Or maybe where it'll fit (flags?)

        /// <summary>
        /// Returns the collection of variables that are affected by the current schedule.
        /// </summary>
        /// <returns>
        /// The collection of variables which are affected by the current schedule.
        /// </returns>
        public IEnumerable<IScheduleVariable> Produces()
        {
            return m_ProducedVariables;
        }

        /// <summary>
        /// Returns the collection of dependencies that have to be in place for the current schedule
        /// to be executed.
        /// </summary>
        /// <returns>
        /// The collection of dependencies for the current schedule.
        /// </returns>
        public IEnumerable<IScheduleDependency> DependsOn()
        {
            return m_Dependencies;
        }
    }
}
