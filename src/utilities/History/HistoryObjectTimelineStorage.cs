//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an object of type <typeparamref name="T"/> that is <see cref="IAmHistoryEnabled"/>.
    /// </summary>
    /// <typeparam name="T">The type of object for which the values are stored.</typeparam>
    /// <remarks>
    ///     Objects that implement <see cref="IAmHistoryEnabled"/> get special treatment because we always want to return 
    ///     the object with a given ID even if that object has changed (i.e. the object reference has changed) due to 
    ///     changes in the timeline.
    /// </remarks>
    internal sealed class HistoryObjectTimelineStorage<T> : TimelineStorage<HistoryId>, IVariableTimeline<T> where T : IAmHistoryEnabled
    {
        /// <summary>
        /// Returns the current value of <typeparamref name="T"/> stored by the <see cref="HistoryObjectTimelineStorage{T}"/> object.
        /// </summary>
        /// <param name="storage">The storage object that holds the value of <typeparamref name="T"/>.</param>
        /// <returns>The value of <typeparamref name="T"/> stored by the <paramref name="storage"/> object.</returns>
        public static implicit operator T(HistoryObjectTimelineStorage<T> storage)
        {
            return storage.Current;
        }

        /// <summary>
        /// The function that is used to find the object that belongs to the given 
        /// <see cref="HistoryId"/>.
        /// </summary>
        private readonly Func<HistoryId, T> m_Lookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryObjectTimelineStorage{T}"/> class.
        /// </summary>
        /// <param name="lookupFunc">The function that is used to find the object that belongs to the given <see cref="HistoryId"/>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="lookupFunc"/> is <see langword="null" />.
        /// </exception>
        public HistoryObjectTimelineStorage(Func<HistoryId, T> lookupFunc)
        {
            {
                Lokad.Enforce.Argument(() => lookupFunc);
            }

            m_Lookup = lookupFunc;
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
                return m_Lookup(CurrentInternalValue);
            }

            set
            {
                CurrentInternalValue = value.HistoryId;
            }
        }
    }
}
