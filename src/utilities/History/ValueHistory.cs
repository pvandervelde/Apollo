//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an object of type <typeparamref name="T"/> that is not <see cref="IAmHistoryEnabled"/>.
    /// </summary>
    /// <typeparam name="T">The type of object for which the values are stored.</typeparam>
    public sealed class ValueHistory<T> : ValueHistoryBase<T>, IVariableTimeline<T>
    {
        /// <summary>
        /// Returns the current value of <typeparamref name="T"/> stored by the <see cref="HistoryObjectValueHistory{T}"/> object.
        /// </summary>
        /// <param name="storage">The storage object that holds the value of <typeparamref name="T"/>.</param>
        /// <returns>The value of <typeparamref name="T"/> stored by the <paramref name="storage"/> object.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storage"/> is <see langword="null" />.
        /// </exception>
        public static implicit operator T(ValueHistory<T> storage)
        {
            {
                Lokad.Enforce.Argument(() => storage);
            }

            return storage.Current;
        }

        /// <summary>
        /// Returns the current value of <typeparamref name="T"/> stored by the <see cref="ValueHistory{T}"/> object.
        /// </summary>
        /// <param name="storage">The storage object that holds the value of <typeparamref name="T"/>.</param>
        /// <returns>The value of <typeparamref name="T"/> stored by the <paramref name="storage"/> object.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storage"/> is <see langword="null" />.
        /// </exception>
        public static T FromStorage(ValueHistory<T> storage)
        {
            {
                Lokad.Enforce.Argument(() => storage);
            }

            return storage.Current;
        }

        /// <summary>
        /// A method called when the current value is changed externally, e.g. through a
        /// roll-back or roll-forward.
        /// </summary>
        protected override void IndicateExternalChangeToCurrentValue()
        {
            var local = OnExternalValueUpdate;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the the stored value is changed externally.
        /// </summary>
        public event EventHandler<EventArgs> OnExternalValueUpdate;

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
