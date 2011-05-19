//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using Apollo.Utilities.Properties;
using Lokad;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the default <see cref="IProgressTimer"/> object.
    /// </summary>
    [ExcludeFromCodeCoverage()]
    public sealed class ProgressTimer : IProgressTimer, IDisposable
    {
        /// <summary>
        /// The timer which is used to fire the progress event.
        /// </summary>
        private readonly Timer m_ProgressTimer = new Timer();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressTimer"/> class.
        /// </summary>
        /// <param name="updateInterval">The time delay between two succesive timer updates.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="updateInterval"/> is smaller or equal to <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public ProgressTimer(TimeSpan updateInterval)
        {
            {
                Enforce.With<ArgumentOutOfRangeException>(updateInterval > TimeSpan.Zero, Resources.Exceptions_Messages_ArgumentOutOfRange_WithArgument, updateInterval);
            }

            m_ProgressTimer.AutoReset = true;
            m_ProgressTimer.Interval = updateInterval.TotalMilliseconds;
            m_ProgressTimer.Elapsed += (s, e) => RaiseElapsed(e.SignalTime);
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            m_ProgressTimer.Start();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            m_ProgressTimer.Stop();
        }

        /// <summary>
        /// Raised when the timer interval is elapsed.
        /// </summary>
        public event EventHandler<TimerElapsedEventArgs> Elapsed;

        /// <summary>
        /// Raises the elapsed event.
        /// </summary>
        /// <param name="signalTime">The signal time.</param>
        private void RaiseElapsed(DateTime signalTime)
        {
            var local = Elapsed;
            if (local != null)
            {
                local(this, new TimerElapsedEventArgs(signalTime));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_ProgressTimer != null)
            {
                m_ProgressTimer.Dispose();
            }
        }
    }
}