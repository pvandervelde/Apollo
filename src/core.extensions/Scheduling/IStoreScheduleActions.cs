//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that store <see cref="IScheduleAction"/> objects.
    /// </summary>
    public interface IStoreScheduleActions : IEnumerable<IScheduleAction>
    {
        /// <summary>
        /// Adds the <see cref="IScheduleAction"/> object with the variables it affects and the dependencies for that action.
        /// </summary>
        /// <param name="action">The action that should be stored.</param>
        /// <param name="produces">The variables that are affected by the action.</param>
        /// <param name="dependsOn">The variables for which data should be available in order to execute the action.</param>
        /// <returns>An object identifying and describing the action.</returns>
        ScheduleActionInformation Add(IScheduleAction action, IEnumerable<IScheduleVariable> produces, IEnumerable<IScheduleDependency> dependsOn);

        /// <summary>
        /// Replaces the action current stored against the given ID with a new one.
        /// </summary>
        /// <param name="actionToReplace">The ID of the action that should be replaced.</param>
        /// <param name="newAction">The new action.</param>
        void Update(ScheduleElementId actionToReplace, IScheduleAction newAction);

        /// <summary>
        /// Removes the action with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the action that should be removed.</param>
        void Remove(ScheduleElementId id);

        /// <summary>
        /// Returns a value indicating if there is an action with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <returns>
        /// <see langword="true" /> if there is an action with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool Contains(ScheduleElementId id);

        /// <summary>
        /// Gets the action that is registered with the given ID.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <returns>The required action.</returns>
        IScheduleAction this[ScheduleElementId id] 
        { 
            get; 
        }
    }
}
