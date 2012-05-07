//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Stores schedule actions by ID.
    /// </summary>
    internal sealed class ScheduleActionStorage : IStoreScheduleActions
    {
        /// <summary>
        /// Adds the <see cref="IScheduleAction"/> object with the variables it affects and the dependencies for that action.
        /// </summary>
        /// <param name="action">The action that should be stored.</param>
        /// <param name="produces">The variables that are affected by the action.</param>
        /// <param name="dependsOn">The variables for which data should be available in order to execute the action.</param>
        /// <returns>An object identifying and describing the action.</returns>
        public ScheduleActionInformation Add(
            IScheduleAction action, 
            IEnumerable<IScheduleVariable> produces, 
            IEnumerable<IScheduleDependency> dependsOn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces the action current stored against the given ID with a new one.
        /// </summary>
        /// <param name="actionToReplace">The ID of the action that should be replaced.</param>
        /// <param name="newAction">The new action.</param>
        public void Update(ScheduleElementId actionToReplace, IScheduleAction newAction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the action with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the action that should be removed.</param>
        public void Remove(ScheduleElementId id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a value indicating if there is an action with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <returns>
        /// <see langword="true" /> if there is an action with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Contains(ScheduleElementId id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the action that is registered with the given ID.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <returns>The required action.</returns>
        public IScheduleAction this[ScheduleElementId id]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<IScheduleAction> GetEnumerator()
        {
            throw new NotImplementedException();
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
