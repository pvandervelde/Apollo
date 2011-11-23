//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an object of type <typeparamref name="T"/> that is not <see cref="IAmHistoryEnabled"/>.
    /// </summary>
    /// <typeparam name="T">The type of object for which the values are stored.</typeparam>
    internal sealed class StandardObjectTimelineStorage<T> : TimelineStorage<T>, IVariableTimeline<T>
    {
        /// <summary>
        /// Returns the current value of <typeparamref name="T"/> stored by the <see cref="HistoryObjectTimelineStorage{T}"/> object.
        /// </summary>
        /// <param name="storage">The storage object that holds the value of <typeparamref name="T"/>.</param>
        /// <returns>The value of <typeparamref name="T"/> stored by the <paramref name="storage"/> object.</returns>
        public static implicit operator T(StandardObjectTimelineStorage<T> storage)
        {
            return storage.Current;
        }

        /// <summary>
        /// Gets or sets the current value for the variable.
        /// </summary>
        /// <remarks>
        /// Note that setting the current value does not directly store 
        /// this value in the timeline. Values are only stored in the 
        /// timeline when the <see cref="IStoreTimelineValues.StoreCurrent(TimeMarker)"/>
        /// method is invoked.
        /// </remarks>
        public T Current
        {
            get
            {
                return CurrentInternalValue;
            }

            set
            {
                CurrentInternalValue = value;
            }
        }
    }
}
