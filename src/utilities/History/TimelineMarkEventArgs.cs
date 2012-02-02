//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that indicates that a new set of changes have been
    /// pushed into the timeline.
    /// </summary>
    public sealed class TimelineMarkEventArgs : EventArgs
    {
        /// <summary>
        /// The marker created when the changes were pushed into the timeline.
        /// </summary>
        private readonly TimeMarker m_Marker;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineMarkEventArgs"/> class.
        /// </summary>
        /// <param name="marker">The new marker.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="marker"/> is <see langword="null" />.
        /// </exception>
        public TimelineMarkEventArgs(TimeMarker marker)
        {
            {
                Lokad.Enforce.Argument(() => marker);
            }

            m_Marker = marker;
        }

        /// <summary>
        /// Gets the new marker.
        /// </summary>
        public TimeMarker Marker
        {
            get
            {
                return m_Marker;
            }
        }
    }
}
