﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Host.Plugins.Definitions
{
    /// <summary>
    /// Stores information about a schedule for a component group in a serialized form, i.e. without requiring the
    /// assembly which defines the group to be loaded.
    /// </summary>
    [Serializable]
    internal sealed class SerializedScheduleDefinition : IEquatable<SerializedScheduleDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SerializedScheduleDefinition first, SerializedScheduleDefinition second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SerializedScheduleDefinition first, SerializedScheduleDefinition second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SerializedScheduleDefinition"/> class.
        /// </summary>
        /// <param name="containingGroup">The ID of the group that has registered the schedule.</param>
        /// <param name="scheduleId">The ID of the schedule that is described.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="actions">The collection that maps a schedule element to an action.</param>
        /// <param name="conditions">The collection that maps a schedule element to a condition.</param>
        /// <returns>The newly created definition.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="containingGroup"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scheduleId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actions"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="conditions"/> is <see langword="null" />.
        /// </exception>
        public static SerializedScheduleDefinition CreateDefinition(
            GroupRegistrationId containingGroup,
            ScheduleId scheduleId,
            IEditableSchedule schedule,
            IDictionary<ScheduleElementId, ScheduleActionRegistrationId> actions,
            IDictionary<ScheduleElementId, ScheduleConditionRegistrationId> conditions)
        {
            {
                Lokad.Enforce.Argument(() => containingGroup);
                Lokad.Enforce.Argument(() => scheduleId);
                Lokad.Enforce.Argument(() => schedule);
                Lokad.Enforce.Argument(() => actions);
                Lokad.Enforce.Argument(() => conditions);
            }

            return new SerializedScheduleDefinition(containingGroup, scheduleId, schedule, actions, conditions);
        }

        /// <summary>
        /// The ID of the group that has registered the schedule.
        /// </summary>
        private readonly GroupRegistrationId m_GroupId;

        /// <summary>
        /// The ID of the schedule that is described by this definition.
        /// </summary>
        private readonly ScheduleId m_ScheduleId;

        /// <summary>
        /// The schedule that is described by this definition.
        /// </summary>
        private readonly IEditableSchedule m_Schedule;

        /// <summary>
        /// The collection that maps a schedule element to a schedule action.
        /// </summary>
        private readonly IDictionary<ScheduleElementId, ScheduleActionRegistrationId> m_Actions;

        /// <summary>
        /// The collection that maps a schedule element to a schedule condition.
        /// </summary>
        private readonly IDictionary<ScheduleElementId, ScheduleConditionRegistrationId> m_Conditions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedScheduleDefinition"/> class.
        /// </summary>
        /// <param name="containingGroup">The ID of the group that has registered the schedule.</param>
        /// <param name="scheduleId">The ID of the schedule that is described.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="actions">The collection that maps a schedule element to an action.</param>
        /// <param name="conditions">The collection that maps a schedule element to a condition.</param>
        private SerializedScheduleDefinition(
            GroupRegistrationId containingGroup,
            ScheduleId scheduleId,
            IEditableSchedule schedule,
            IDictionary<ScheduleElementId, ScheduleActionRegistrationId> actions,
            IDictionary<ScheduleElementId, ScheduleConditionRegistrationId> conditions)
        {
            {
                Debug.Assert(containingGroup != null, "The containing group ID should not be a null reference.");
                Debug.Assert(scheduleId != null, "The schedule ID should not be a null reference.");
                Debug.Assert(schedule != null, "The schedule should not be a null reference.");
                Debug.Assert(actions != null, "The collection of actions should not be a null reference.");
                Debug.Assert(conditions != null, "The collection of conditions should not be a null reference.");
            }

            m_GroupId = containingGroup;
            m_ScheduleId = scheduleId;
            m_Schedule = schedule;
            m_Actions = actions;
            m_Conditions = conditions;
        }

        /// <summary>
        /// Gets the ID of the group that has registered the schedule.
        /// </summary>
        public GroupRegistrationId ContainingGroup
        {
            get
            {
                return m_GroupId;
            }
        }

        /// <summary>
        /// Gets the ID of the schedule.
        /// </summary>
        public ScheduleId ScheduleId
        {
            get
            {
                return m_ScheduleId;
            }
        }

        /// <summary>
        /// Gets the schedule for the current group.
        /// </summary>
        public IEditableSchedule Schedule
        {
            get
            {
                return m_Schedule;
            }
        }

        /// <summary>
        /// Gets the collection that maps a schedule element to a schedule action.
        /// </summary>
        public IDictionary<ScheduleElementId, ScheduleActionRegistrationId> Actions
        {
            get
            {
                return m_Actions;
            }
        }

        /// <summary>
        /// Gets the collection that maps a schedule element to a schedule condition.
        /// </summary>
        public IDictionary<ScheduleElementId, ScheduleConditionRegistrationId> Conditions
        {
            get
            {
                return m_Conditions;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializedScheduleDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializedScheduleDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializedScheduleDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(SerializedScheduleDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) 
                && ContainingGroup == other.ContainingGroup 
                && ScheduleId == other.ScheduleId;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as SerializedScheduleDefinition;
            return Equals(id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // As obtained from the Jon Skeet answer to:
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ ContainingGroup.GetHashCode();
                hash = (hash * 23) ^ ScheduleId.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Exporting [{0}] on {1}",
                ScheduleId,
                ContainingGroup);
        }
    }
}
