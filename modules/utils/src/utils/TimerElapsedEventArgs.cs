//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utils
{
    /// <summary>
    /// Event arguments used for <see cref="IProgressTimer"/> elapsed events.
    /// </summary>
    public sealed class TimerElapsedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerElapsedEventArgs"/> class.
        /// </summary>
        /// <param name="time">The elapsed time.</param>
        public TimerElapsedEventArgs(DateTime time)
        {
            ElapsedTime = time;
        }

        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        /// <value>The elapsed time.</value>
        public DateTime ElapsedTime
        {
            get;
            private set;
        }
    }
}
