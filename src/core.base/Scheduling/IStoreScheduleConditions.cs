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
    /// Defines the interface for objects that store <see cref="IScheduleCondition"/> objects.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "While we store schedules here, it's more than just a collection.")]
    public interface IStoreScheduleConditions : IEnumerable<ScheduleElementId>
    {
        /// <summary>
        /// Adds the <see cref="IScheduleCondition"/> object with the dependencies for that condition.
        /// </summary>
        /// <param name="id">The ID of the condition.</param>
        /// <param name="condition">The condition that should be stored.</param>
        /// <param name="name">The name of the condition that is being described by this information object.</param>
        /// <param name="summary">The summary of the condition that is being described by this information object.</param>
        /// <param name="description">The description of the condition that is being described by this information object.</param>
        /// <param name="dependsOn">The variables for which data should be available in order to evaluate the condition.</param>
        /// <returns>An object identifying and describing the condition.</returns>
        ScheduleConditionInformation Add(
            ScheduleElementId id,
            IScheduleCondition condition,
            string name,
            string summary,
            string description,
            IEnumerable<IScheduleDependency> dependsOn);

        /// <summary>
        /// Replaces the condition current stored against the given ID with a new one.
        /// </summary>
        /// <param name="conditionToReplace">The ID of the condition that should be replaced.</param>
        /// <param name="newCondition">The new condition.</param>
        void Update(
            ScheduleElementId conditionToReplace,
            IScheduleCondition newCondition);

        /// <summary>
        /// Removes the condition with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the condition that should be removed.</param>
        void Remove(ScheduleElementId id);

        /// <summary>
        /// Returns a value indicating if there is an condition with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the condition.</param>
        /// <returns>
        /// <see langword="true" /> if there is an condition with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool Contains(ScheduleElementId id);

        /// <summary>
        /// Returns the condition that is mapped to the given ID.
        /// </summary>
        /// <param name="id">The ID for the condition.</param>
        /// <returns>The condition.</returns>
        IScheduleCondition Condition(ScheduleElementId id);

        /// <summary>
        /// Returns the condition information for the condition that is mapped to the given ID.
        /// </summary>
        /// <param name="id">The ID for the condition.</param>
        /// <returns>The condition information.</returns>
        ScheduleConditionInformation Information(ScheduleElementId id);
    }
}
