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
    /// Stores information about a given action.
    /// </summary>
    public sealed class ScheduleActionInformation
    {
        /// <summary>
        /// The ID of the action that is being described by this information object.
        /// </summary>
        private readonly ScheduleElementId m_Id;

        /// <summary>
        /// The name of the action that is being described by this information object.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// The summary of the action that is being described by this information object.
        /// </summary>
        private readonly string m_Summary;

        /// <summary>
        /// The description of the action that is being described by this information object.
        /// </summary>
        private readonly string m_Description;

        /// <summary>
        /// The collection of variables which are affected by the action that is being described by this
        /// information object.
        /// </summary>
        private readonly IEnumerable<IScheduleVariable> m_ProducedVariables;

        /// <summary>
        /// The collection of dependencies for the action that is being described by this information object.
        /// </summary>
        private readonly IEnumerable<IScheduleDependency> m_Dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleActionInformation"/> class.
        /// </summary>
        /// <param name="id">The ID of the action that is being described by this information object.</param>
        /// <param name="name">The name of the action that is being described by this information object.</param>
        /// <param name="summary">The summary of the action that is being described by this information object.</param>
        /// <param name="description">The description of the action that is being described by this information object.</param>
        /// <param name="produces">The collection of variables that is affected by the action described by this information object.</param>
        /// <param name="dependencies">The collection of dependencies for the action described by this information object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="produces"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dependencies"/> is <see langword="null" />.
        /// </exception>
        public ScheduleActionInformation(
            ScheduleElementId id,
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
        /// Gets the ID for the action.
        /// </summary>
        public ScheduleElementId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the summary of the action.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Summary;
            }
        }

        /// <summary>
        /// Gets the description of the action.
        /// </summary>
        public string Description
        {
            get
            {
                return m_Description;
            }
        }

        // Type identifier? --> Describes in code what the action does? Or maybe where it'll fit (flags?)

        /// <summary>
        /// Returns the collection of variables that are affected by the current schedule action.
        /// </summary>
        /// <returns>
        /// The collection of variables which are affected by the current schedule action.
        /// </returns>
        public IEnumerable<IScheduleVariable> Produces()
        {
            return m_ProducedVariables;
        }

        /// <summary>
        /// Returns the collection of dependencies that have to be in place for the current schedule
        /// action to run.
        /// </summary>
        /// <returns>
        /// The collection of dependencies for the current schedule action.
        /// </returns>
        public IEnumerable<IScheduleDependency> DependsOn()
        {
            return m_Dependencies;
        }
    }
}
