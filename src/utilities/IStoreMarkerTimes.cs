//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for objects which store marker times.
    /// </summary>
    public interface IStoreMarkerTimes
    {
        /// <summary>
        /// Gets the total time necessary for the given operation.
        /// </summary>
        /// <value>The total time.</value>
        TimeSpan TotalTime
        {
            get;
        }

        /// <summary>
        /// Returns the standard time necessary to reach the specified marker.
        /// </summary>
        /// <param name="mark">The current mark.</param>
        /// <returns>
        /// The time taken to reach the specified marker.
        /// </returns>
        TimeSpan TimeFor(IProgressMark mark);

        /// <summary>
        /// Returns the time between the two marks.
        /// </summary>
        /// <param name="firstMark">The first mark.</param>
        /// <param name="secondMark">The second mark.</param>
        /// <returns>
        /// The time between the two marks.
        /// </returns>
        TimeSpan TimeBetween(IProgressMark firstMark, IProgressMark secondMark);
    }
}
