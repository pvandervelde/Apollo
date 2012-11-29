//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Base.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that store <see cref="IEditableSchedule"/> objects.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "While we store schedules here, it's more than just a collection.")]
    public interface IStoreSchedules : IEnumerable<ScheduleId>
    {
        /// <summary>
        /// Adds the <see cref="IEditableSchedule"/> object with the variables it affects and the dependencies for that schedule.
        /// </summary>
        /// <param name="schedule">The schedule that should be stored.</param>
        /// <param name="name">The name of the schedule that is being described by this information object.</param>
        /// <param name="description">The description of the schedule that is being described by this information object.</param>
        /// <returns>An object identifying and describing the schedule.</returns>
        ScheduleInformation Add(
            IEditableSchedule schedule,
            string name,
            string description);

        /// <summary>
        /// Replaces the schedule current stored against the given ID with a new one.
        /// </summary>
        /// <param name="scheduleToReplace">The ID of the schedule that should be replaced.</param>
        /// <param name="newSchedule">The new schedule.</param>
        void Update(
            ScheduleId scheduleToReplace,
            IEditableSchedule newSchedule);

        /// <summary>
        /// Removes the schedule with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the schedule that should be removed.</param>
        void Remove(ScheduleId id);

        /// <summary>
        /// Returns a value indicating if there is an schedule with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <returns>
        /// <see langword="true" /> if there is an schedule with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool Contains(ScheduleId id);

        /// <summary>
        /// Returns the schedule that is mapped to the given ID.
        /// </summary>
        /// <param name="id">The ID for the schedule.</param>
        /// <returns>The schedule.</returns>
        IEditableSchedule Schedule(ScheduleId id);

        /// <summary>
        /// Returns the schedule information for the schedule that is mapped to the given ID.
        /// </summary>
        /// <param name="id">The ID for the schedule.</param>
        /// <returns>The schedule information.</returns>
        ScheduleInformation Information(ScheduleId id);
    }
}
