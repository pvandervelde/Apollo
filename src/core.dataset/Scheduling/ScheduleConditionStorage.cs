//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Dataset.Properties;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Stores schedule conditions by ID.
    /// </summary>
    internal sealed class ScheduleConditionStorage : IStoreScheduleConditions
    {
        /// <summary>
        /// Maps the action to the information describing the action.
        /// </summary>
        private sealed class ConditionMap
        {
            /// <summary>
            /// The action.
            /// </summary>
            private readonly IScheduleCondition m_Condition;

            /// <summary>
            /// The information describing the action.
            /// </summary>
            private readonly ScheduleConditionInformation m_Info;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConditionMap"/> class.
            /// </summary>
            /// <param name="information">The information describing the condition.</param>
            /// <param name="condition">The condition.</param>
            public ConditionMap(ScheduleConditionInformation information, IScheduleCondition condition)
            {
                {
                    Debug.Assert(information != null, "The condition information should not be a null reference.");
                    Debug.Assert(condition != null, "The condition should not be a null reference.");
                }

                m_Info = information;
                m_Condition = condition;
            }

            /// <summary>
            /// Gets the condition information.
            /// </summary>
            public ScheduleConditionInformation Information
            {
                get
                {
                    return m_Info;
                }
            }

            /// <summary>
            /// Gets the condition.
            /// </summary>
            public IScheduleCondition Condition
            {
                get
                {
                    return m_Condition;
                }
            }
        }

        /// <summary>
        /// The collection of schedule conditions.
        /// </summary>
        private readonly Dictionary<ScheduleElementId, ConditionMap> m_Conditions
            = new Dictionary<ScheduleElementId, ConditionMap>();

        /// <summary>
        /// Adds the <see cref="IScheduleCondition"/> object with the dependencies for that condition.
        /// </summary>
        /// <param name="condition">The condition that should be stored.</param>
        /// <param name="name">The name of the condition that is being described by this information object.</param>
        /// <param name="summary">The summary of the condition that is being described by this information object.</param>
        /// <param name="description">The description of the condition that is being described by this information object.</param>
        /// <param name="dependsOn">The variables for which data should be available in order to evaluate the condition.</param>
        /// <returns>An object identifying and describing the condition.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="condition"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dependsOn"/> is <see langword="null" />.
        /// </exception>
        public ScheduleConditionInformation Add(
            IScheduleCondition condition,
            string name,
            string summary,
            string description,
            IEnumerable<IScheduleDependency> dependsOn)
        {
            {
                Lokad.Enforce.Argument(() => condition);
                Lokad.Enforce.Argument(() => dependsOn);
            }

            var id = new ScheduleElementId();
            var info = new ScheduleConditionInformation(id, name, summary, description, dependsOn);
            m_Conditions.Add(id, new ConditionMap(info, condition));

            return info;
        }

        /// <summary>
        /// Replaces the condition current stored against the given ID with a new one.
        /// </summary>
        /// <param name="conditionToReplace">The ID of the condition that should be replaced.</param>
        /// <param name="newCondition">The new condition.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="conditionToReplace"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="newCondition"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleConditionException">
        ///     Thrown if <paramref name="conditionToReplace"/> is not linked to a known condition.
        /// </exception>
        public void Update(
            ScheduleElementId conditionToReplace,
            IScheduleCondition newCondition)
        {
            {
                Lokad.Enforce.Argument(() => conditionToReplace);
                Lokad.Enforce.Argument(() => newCondition);
                Lokad.Enforce.With<UnknownScheduleConditionException>(
                    m_Conditions.ContainsKey(conditionToReplace),
                    Resources.Exceptions_Messages_UnknownScheduleCondition);
            }

            var oldInfo = m_Conditions[conditionToReplace].Information;
            var info = new ScheduleConditionInformation(
                conditionToReplace,
                oldInfo.Name,
                oldInfo.Summary,
                oldInfo.Description,
                oldInfo.DependsOn());
            m_Conditions[conditionToReplace] = new ConditionMap(info, newCondition);
        }

        /// <summary>
        /// Removes the condition with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the condition that should be removed.</param>
        public void Remove(ScheduleElementId id)
        {
            if ((id != null) && m_Conditions.ContainsKey(id))
            {
                m_Conditions.Remove(id);
            }
        }

        /// <summary>
        /// Returns a value indicating if there is an condition with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the condition.</param>
        /// <returns>
        /// <see langword="true" /> if there is an condition with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Contains(ScheduleElementId id)
        {
            return (id != null) && m_Conditions.ContainsKey(id);
        }

        /// <summary>
        /// Returns the condition that is mapped to the given ID.
        /// </summary>
        /// <param name="id">The ID for the condition.</param>
        /// <returns>The condition.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleConditionException">
        ///     Thrown if <paramref name="id"/> is not linked to a known condition.
        /// </exception>
        public IScheduleCondition Condition(ScheduleElementId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<UnknownScheduleConditionException>(
                    m_Conditions.ContainsKey(id),
                    Resources.Exceptions_Messages_UnknownScheduleCondition);
            }

            return m_Conditions[id].Condition;
        }

        /// <summary>
        /// Returns the condition information for the condition that is mapped to the given ID.
        /// </summary>
        /// <param name="id">The ID for the condition.</param>
        /// <returns>The condition information.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleConditionException">
        ///     Thrown if <paramref name="id"/> is not linked to a known condition.
        /// </exception>
        public ScheduleConditionInformation Information(ScheduleElementId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<UnknownScheduleConditionException>(
                    m_Conditions.ContainsKey(id),
                    Resources.Exceptions_Messages_UnknownScheduleAction);
            }

            return m_Conditions[id].Information;
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<ScheduleElementId> GetEnumerator()
        {
            foreach (var key in m_Conditions.Keys)
            {
                yield return key;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through 
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
