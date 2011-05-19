//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for a progress timer.
    /// </summary>
    public interface IProgressTimer
    {
        /// <summary>
        /// Starts the timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop",
            Justification = "Stop is the most logical name for the method that stops the timer.")]
        void Stop();

        /// <summary>
        /// Raised when the timer interval is elapsed.
        /// </summary>
        event EventHandler<TimerElapsedEventArgs> Elapsed;
    }
}
