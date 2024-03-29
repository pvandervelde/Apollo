﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for a given object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object for which the values are stored.</typeparam>
    /// <design>
    /// At the moment we just use 2 linked lists for the undo / redo collections. This means that we
    /// have the two lists and a LinkedListNode for each level (with 2 pointers + a value pointer) allocated.
    /// If we find that this is too much then we can always change the design to something more 'cunning'.
    /// e.g. a system where we store everything in an array or something.
    /// </design>
    public abstract class ValueHistoryBase<T> : IStoreTimelineValues
    {
        /// <summary>
        /// The object that stores the past and future values.
        /// </summary>
        private readonly ValueAtTimeStorage<T> m_History = new ValueAtTimeStorage<T>();

        /// <summary>
        /// The current value of the variable.
        /// </summary>
        private T m_Current;

        /// <summary>
        /// A flag that indicates that there has been a change to the current value 
        /// due to the change in history.
        /// </summary>
        private bool m_HasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueHistoryBase{T}"/> class.
        /// </summary>
        protected ValueHistoryBase()
        {
            m_History.OnCurrentValueChange += (s, e) => { m_HasChanged = true; };
        }

        /// <summary>
        /// A method called when the current value is changed externally, e.g. through a
        /// roll-back or roll-forward.
        /// </summary>
        protected abstract void IndicateExternalChangeToCurrentValue();

        /// <summary>
        /// Returns a value indicating if the history is currently at the beginning of known time, 
        /// meaning that we can only move forward.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if history is at the beginning of known time; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsAtBeginOfTime()
        {
            return m_History.IsAtBeginOfTime();
        }

        /// <summary>
        /// Returns a value indicating if moving back to the given time marker would 
        /// require moving to a point prior to the known beginning of time.
        /// </summary>
        /// <param name="mark">The mark to move back to.</param>
        /// <returns>
        ///     <see langword="true" /> if moving back to the marker requires moving to a point prior to the 
        ///     known beginning of time; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private bool WouldRollBackPastTheBeginningOfTime(TimeMarker mark)
        {
            return m_History.WouldRollBackPastTheBeginningOfTime(mark);
        }

        /// <summary>
        /// Returns a value indicating if the history is currently at the end of known time, 
        /// meaning that we can only move backward.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if history is at the end of known time; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsAtEndOfTime()
        {
            return m_History.IsAtEndOfTime();
        }

        /// <summary>
        /// Rolls the current value back to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        public void RollBackTo(TimeMarker marker)
        {
            if (!IsAtBeginOfTime())
            {
                var hasLocalChanged = IsLastValueDifferent();
                
                m_Current = m_History.RollBackTo(marker);
                if (m_HasChanged || hasLocalChanged)
                {
                    m_HasChanged = false;
                    IndicateExternalChangeToCurrentValue();
                }
            }
        }

        /// <summary>
        /// Rolls the current value back to the start point.
        /// </summary>
        public void RollBackToStart()
        {
            if (!IsAtBeginOfTime())
            {
                var hasLocalChanged = IsLastValueDifferent();

                m_Current = m_History.RollBackToStart();
                if (m_HasChanged || hasLocalChanged)
                {
                    m_HasChanged = false;
                    IndicateExternalChangeToCurrentValue();
                }
            }
        }

        /// <summary>
        /// Rolls the current value forward to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        public void RollForwardTo(TimeMarker marker)
        {
            if (!IsAtEndOfTime())
            {
                var hasLocalChanged = IsLastValueDifferent();

                m_Current = m_History.RollForwardTo(marker);
                if (m_HasChanged || hasLocalChanged)
                {
                    m_HasChanged = false;
                    IndicateExternalChangeToCurrentValue();
                }
            }
        }

        /// <summary>
        /// Stores the current value as the default value which will be returned if there are no values stored.
        /// </summary>
        /// <exception cref="CannotSetDefaultAfterStartOfTimeException">
        /// Thrown when the user tries to store the default value after one or more value have been stored.
        /// </exception>
        public void StoreCurrentAsDefault()
        {
            {
                Lokad.Enforce.With<CannotSetDefaultAfterStartOfTimeException>(
                    m_History.LastValue == null,
                    Resources.Exceptions_Messages_CannotSetDefaultAfterStartOfTime);
            }

            m_History.StoreCurrent(TimeMarker.TheBeginOfTime, m_Current);
        }

        /// <summary>
        /// Stores the current value in the history list with the given marker.
        /// </summary>
        /// <param name="marker">The marker which indicates at which point on the timeline the data is stored.</param>
        /// <exception cref="CannotStoreValuesAtTheStartOfTimeException">
        ///     Thrown when <paramref name="marker"/> is equal to <see cref="TimeMarker.TheBeginOfTime"/>.
        /// </exception>
        public void StoreCurrent(TimeMarker marker)
        {
            {
                Lokad.Enforce.With<CannotStoreValuesAtTheStartOfTimeException>(
                    marker > TimeMarker.TheBeginOfTime,
                    Resources.Exceptions_Messages_CannotStoreValuesAtTheStartOfTime);
            }

            if (IsLastValueDifferent())
            {
                m_History.StoreCurrent(marker, m_Current);
            }
        }

        private bool IsLastValueDifferent()
        {
            if (IsAtBeginOfTime())
            {
                return true;
            }

            var lastValue = m_History.LastValue;
            if (ReferenceEquals(lastValue, null))
            {
                return !ReferenceEquals(m_Current, null);
            }
            else 
            {
                return !lastValue.Equals(m_Current);
            }
        }

        /// <summary>
        /// Clears all the history storage and forgets all the 
        /// stored historic information.
        /// </summary>
        public void ForgetAllHistory()
        {
            m_History.ForgetAllHistory();
            m_Current = default(T);
        }

        /// <summary>
        /// Clears all the history information that is in the future.
        /// </summary>
        public void ForgetTheFuture()
        {
            m_History.ForgetTheFuture();
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
        protected T CurrentInternalValue
        {
            [DebuggerStepThrough]
            get
            {
                return m_Current;
            }

            [DebuggerStepThrough]
            set
            {
                m_Current = value;
            }
        }
    }
}
