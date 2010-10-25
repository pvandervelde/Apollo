//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Utils;

namespace Apollo.Core.Test.Spec
{
    /// <summary>
    /// Stores marker times for the Project Explorer.
    /// </summary>
    internal sealed class StartupTimeStorage : IStoreMarkerTimes
    {
        /// <summary>
        /// The collection that maps between a progress mark and the amount of time taken to arrive 
        /// at that progress mark.
        /// </summary>
        private readonly Dictionary<Type, TimeSpan> m_MarkerTimes = new Dictionary<Type, TimeSpan>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupTimeStorage"/> class.
        /// </summary>
        public StartupTimeStorage()
        {
            // Normally we should fill up the structure through some
            // cunning config reading thing
            // But for now we'll cheat and do something random
            m_MarkerTimes.Add(typeof(ApplicationStartingProgressMark), new TimeSpan(0, 0, 0, 0, 0));
            m_MarkerTimes.Add(typeof(CoreLoadingProgressMark), new TimeSpan(0, 0, 0, 0, 1));
            m_MarkerTimes.Add(typeof(CoreStartingProgressMark), new TimeSpan(0, 0, 0, 0, 2));
            m_MarkerTimes.Add(typeof(ApplicationStartupFinishedProgressMark), new TimeSpan(0, 0, 0, 0, 3));
        }

        #region Implementation of IStoreMarkerTimes

        /// <summary>
        /// Gets the total time necessary for the given operation.
        /// </summary>
        /// <value>The total time.</value>
        public TimeSpan TotalTime
        {
            get
            {
                return m_MarkerTimes[typeof(ApplicationStartupFinishedProgressMark)];
            }
        }

        /// <summary>
        /// Returns the standard time necessary to reach the specified marker.
        /// </summary>
        /// <param name="mark">The current mark.</param>
        /// <returns>
        /// The time taken to reach the specified marker.
        /// </returns>
        public TimeSpan TimeFor(IProgressMark mark)
        {
            if (!m_MarkerTimes.ContainsKey(mark.GetType()))
            {
                throw new ArgumentException(mark.ToString());
            }

            return m_MarkerTimes[mark.GetType()];
        }

        /// <summary>
        /// Returns the time between the two marks.
        /// </summary>
        /// <param name="firstMark">The first mark, which is the first mark in time.</param>
        /// <param name="secondMark">The second mark, which is the last mark in time.</param>
        /// <returns>
        /// The time between the <paramref name="firstMark"/> and the <paramref name="secondMark"/>.
        /// </returns>
        public TimeSpan TimeBetween(IProgressMark firstMark, IProgressMark secondMark)
        {
            var firstTime = TimeFor(firstMark);
            var secondTime = TimeFor(secondMark);

            return secondTime - firstTime;
        }

        #endregion
    }
}
