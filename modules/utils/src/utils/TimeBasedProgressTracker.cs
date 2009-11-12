//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using Apollo.Utils.Configuration;
using Lokad;

namespace Apollo.Utils
{
    /// <summary>
    /// Defines methods used to track progress based on an estimate of the time
    /// required to finish the operation previously.
    /// </summary>
    public sealed class TimeBasedProgressTracker : ITrackProgress
    {
        /// <summary>
        /// The timer which is used to fire the progress event.
        /// </summary>
        private readonly System.Timers.Timer m_ProgressTimer = new System.Timers.Timer();

        /// <summary>
        /// The time delay between two successive timer updates.
        /// </summary>
        private readonly TimeSpan m_TimerUpdateInterval;

        /// <summary>
        /// The configuration object which holds the appliation configuration values.
        /// </summary>
        private readonly IConfiguration m_Configuration;
        
        /// <summary>
        /// The object which stores the progress timing for each of the known markers.
        /// </summary>
        private readonly IStoreMarkerTimes m_MarkerTimers;
        
        /// <summary>
        /// The function invoked each time the timer interval elapses.
        /// </summary>
        private Action<int, IProgressMark> m_Progress;

        /// <summary>
        /// This is the synchronization point that prevents events
        /// from running concurrently, and prevents the main thread 
        /// from executing code after the Stop method until any 
        /// event handlers are done executing.
        /// </summary>
        /// <source>
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.stop.aspx
        /// </source>
        private int m_SyncPoint = 0;

        /// <summary>
        /// Returns the time that has elapsed since starting the progress
        /// indication.
        /// </summary>
        private Func<DateTime, TimeSpan> m_ElapsedTime;

        /// <summary>
        /// The mark which indicates what is currently being processed.
        /// </summary>
        private IProgressMark m_CurrentMark;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeBasedProgressTracker"/> class.
        /// </summary>
        /// <param name="timerUpdateInterval">The time delay between two succesive timer updates.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="markerTimes">The marker times.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="timerUpdateInterval"/> is smaller or equal to <see cref="TimeSpan.Zero"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="markerTimes"/> is <see langword="null" />.
        /// </exception>
        public TimeBasedProgressTracker(
            TimeSpan timerUpdateInterval,
            IConfiguration configuration,
            IStoreMarkerTimes markerTimes)
        {
            {
                Enforce.That(timerUpdateInterval > TimeSpan.Zero);
                Enforce.Argument(() => configuration);
                Enforce.Argument(() => markerTimes);
            }

            m_TimerUpdateInterval = timerUpdateInterval;
            m_Configuration = configuration;
            m_MarkerTimers = markerTimes;
        }

        /// <summary>
        /// Starts the tracking of the progress.
        /// </summary>
        /// <param name="progress">The function called each time the progress event is invoked.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="progress"/> is <see langword="null" />.
        /// </exception>
        public void StartTracking(Action<int, IProgressMark> progress)
        {
            {
                Enforce.Argument(() => progress);
            }

            m_Progress = progress;

            // Capture the time at which the timer was started
            var startTime = DateTime.Now;
            m_ElapsedTime = time => time - startTime;

            // Initialize the timer
            m_ProgressTimer.AutoReset = true;
            m_ProgressTimer.Enabled = true;
            m_ProgressTimer.Interval = m_TimerUpdateInterval.TotalMilliseconds;
            m_ProgressTimer.Elapsed += (s, e) => ProcessProgressTimerElapsed(e.SignalTime);

            // Start the timer
            m_ProgressTimer.Start();
        }

        /// <summary>
        /// Processes the elapsed event of the progress timer.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <design>
        /// This example assumes that overlapping events can be
        /// discarded. That is, if an Elapsed event is raised before 
        /// the previous event is finished processing, the second
        /// event is ignored. In this case that is probably not 
        /// directly what we want however we push all the event
        /// processing onto a seperate thread so the execution of
        /// the event should not take very long compared to the 
        /// timer interval.
        /// </design>
        /// <source>
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.stop.aspx
        /// </source>
        private void ProcessProgressTimerElapsed(DateTime time)
        {
            // CompareExchange is used to take control of m_SyncPoint, 
            // and to determine whether the attempt was successful. 
            // CompareExchange attempts to put 1 into syncPoint, but
            // only if the current value of syncPoint is zero 
            // (specified by the third parameter). If another thread
            // has set syncPoint to 1, or if the control thread has
            // set syncPoint to -1, the current event is skipped. 
            // (Normally it would not be necessary to use a local 
            // variable for the return value. A local variable is 
            // used here to determine the reason the event was 
            // skipped.)
            int sync = Interlocked.CompareExchange(ref m_SyncPoint, 1, 0);
            if (sync == 0)
            {
                // No other event was executing.
                // Note that the only other event that could be
                // executing, is us.
                //
                // Each time the timer fires we update the
                // progress. We assume:
                // - that progress is linear(?) between two progress points
                // - that progress in the current startup is roughly the same
                //   as in the last start up
                ThreadPool.QueueUserWorkItem(state =>
                    {
                        var elapsedTime = m_ElapsedTime(time);
                        var estimatedTime = m_MarkerTimers.TotalTime;

                        var progress = (int)(elapsedTime.Ticks * 100.0 / estimatedTime.Ticks);
                        m_Progress(progress, m_CurrentMark);
                    });

                // Release control of syncPoint.
                m_SyncPoint = 0;
            }
        }

        /// <summary>
        /// Marks the current time with the specified marker.
        /// </summary>
        /// <param name="mark">The mark.</param>
        public void Mark(IProgressMark mark)
        {
            m_CurrentMark = mark;
        }

        /// <summary>
        /// Stops the tracking of the progress.
        /// </summary>
        public void StopTracking()
        {
            // Stop the progress timing. 
            StopProgressTimer();
        }

        /// <summary>
        /// Stops the progress timer so that all currently running events
        /// are finished.
        /// </summary>
        /// <source>
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.stop.aspx
        /// </source>
        private void StopProgressTimer()
        {
            // Stop the timer
            m_ProgressTimer.Stop();

            // The 'counted' flag ensures that if this thread has
            // to wait for an event to finish, the wait only gets 
            // counted once.
            bool counted = false;

            // Ensure that if an event is currently executing,
            // no further processing is done on this thread until
            // the event handler is finished. This is accomplished
            // by using CompareExchange to place -1 in syncPoint,
            // but only if syncPoint is currently zero (specified
            // by the third parameter of CompareExchange). 
            // CompareExchange returns the original value that was
            // in syncPoint. If it was not zero, then there's an
            // event handler running, and it is necessary to try
            // again.
            while (Interlocked.CompareExchange(ref m_SyncPoint, -1, 0) != 0)
            {
                // Give up the rest of this thread's current time
                // slice. This is a naive algorithm for yielding.
                Thread.Sleep(1);

                // Tally a wait, but don't count multiple calls to
                // Thread.Sleep.
                if (!counted)
                {
                    counted = true;
                }
            }

            // Any processing done after this point does not conflict
            // with timer events. This is the purpose of the call to
            // CompareExchange. If the processing done here would not
            // cause a problem when run concurrently with timer events,
            // then there is no need for the extra synchronization.
        }

        /// <summary>
        /// Resets the progress tracker.
        /// </summary>
        public void Reset()
        {
            StopProgressTimer();
            m_Progress = null;
        }
    }
}
