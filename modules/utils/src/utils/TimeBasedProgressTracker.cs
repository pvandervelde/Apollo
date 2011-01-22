//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Apollo.Utils.Properties;
using Lokad;

namespace Apollo.Utils
{
    /// <summary>
    /// Defines methods used to track progress based on an estimate of the time
    /// required to finish the operation previously.
    /// </summary>
    public sealed class TimeBasedProgressTracker : ITrackProgress, IDisposable
    {
        /// <summary>
        /// The progress at the start of the event.
        /// </summary>
        private const int s_StartingProgress = 0;

        /// <summary>
        /// The progress at the end of the event.
        /// </summary>
        private const int s_FinishingProgress = 100;

        /// <summary>
        /// The timer which is used to fire the progress event.
        /// </summary>
        private readonly IProgressTimer m_ProgressTimer;

        /// <summary>
        /// The value used to indicate an unknown progress level.
        /// </summary>
        private readonly int m_UnknownProgressValue;

        /// <summary>
        /// The object which stores the progress timing for each of the known markers.
        /// </summary>
        private readonly IStoreMarkerTimes m_MarkerTimers;
        
        /// <summary>
        /// This is the synchronization point that prevents events
        /// from running concurrently, and prevents the main thread 
        /// from executing code after the Stop method until any 
        /// event handlers are done executing.
        /// </summary>
        /// <source>
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.stop.aspx
        /// </source>
        private int m_SyncPoint;

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
        /// <param name="timer">The timer which is used to keep track of progress.</param>
        /// <param name="unknownProgressValue">The value used to indicate an unknown progress level.</param>
        /// <param name="markerTimes">The marker times.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="timer"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="unknownProgressValue"/> is between 0 and 100.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="markerTimes"/> is <see langword="null"/>.
        /// </exception>
        public TimeBasedProgressTracker(
            IProgressTimer timer,
            int unknownProgressValue,
            IStoreMarkerTimes markerTimes)
        {
            {
                Enforce.Argument(() => timer);
                Enforce.With<ArgumentOutOfRangeException>((unknownProgressValue < 0) || (unknownProgressValue > 100), Resources.Exceptions_Messages_ArgumentOutOfRange_WithArgument, unknownProgressValue);
                Enforce.Argument(() => markerTimes);
            }

            m_ProgressTimer = timer;
            m_UnknownProgressValue = unknownProgressValue;
            m_MarkerTimers = markerTimes;
        }

        /// <summary>
        /// Starts the tracking of the progress.
        /// </summary>
        /// <exception cref="CurrentProgressMarkNotSetException">Thrown if no <see cref="IProgressMark"/> has been set.</exception>
        public void StartTracking()
        {
            {
                Enforce.With<CurrentProgressMarkNotSetException>(m_CurrentMark != null, Resources.Exceptions_Messages_CurrentProgressMarkNotSet);
            }

            // Capture the time at which the timer was started
            var startTime = DateTime.Now;
            m_ElapsedTime = time => time - startTime;

            // Initialize the timer
            m_ProgressTimer.Elapsed += (s, e) => ProcessProgressTimerElapsed(e.ElapsedTime);

            // Fire the first event. If all is well we have at least one mark
            // And we'll assume there is no progress yet.
            RaiseStartupProgress(s_StartingProgress, m_CurrentMark);

            // Start the timer
            m_ProgressTimer.Start();
        }

        /// <summary>
        /// Processes the elapsed event of the progress timer.
        /// </summary>
        /// <param name="time">The time at which the timer elapsed even took place.</param>
        /// <design>
        /// <para>
        /// This code assumes that overlapping events can be
        /// discarded. That is, if an StartupProgress event is raised before 
        /// the previous event is finished processing, the second
        /// event is ignored. In this case that is probably not 
        /// directly what we want however we push all the event
        /// processing onto a seperate thread so the execution of
        /// the event should not take very long compared to the 
        /// timer interval.
        /// </para>
        /// <para>
        /// Each time the timer fires we update the progress. We assume:
        /// <list type="bullet">
        /// <item>that progress is linear over the entire process</item>
        /// <item>
        ///     that progress in the current startup is roughly the same as 
        ///     the one for which we have the data.
        /// </item>
        /// </list>
        /// </para>
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
            if (Interlocked.CompareExchange(ref m_SyncPoint, 1, 0) == 0)
            {
                // No other event was executing.
                // Note that the only other event that could be
                // executing, is us.
                ThreadPool.QueueUserWorkItem(state =>
                    {
                        try
                        {
                            var elapsedTime = m_ElapsedTime(time);
                            var estimatedTime = m_MarkerTimers.TotalTime;

                            // If there is no known time then we return
                            // a special progress count.
                            int progress = m_UnknownProgressValue;
                            if (estimatedTime != TimeSpan.Zero)
                            {
                                progress = (int)Math.Round(elapsedTime.Ticks * 100.0 / estimatedTime.Ticks, MidpointRounding.ToEven);

                                // Progress can never be larger as 100% so if it is then we just
                                // assume we're at 100%
                                if (progress > s_FinishingProgress)
                                {
                                    progress = s_FinishingProgress;
                                }
                            }

                            RaiseStartupProgress(progress, m_CurrentMark);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    });

                // Release control of syncPoint
                // We can just write to the value because integers 
                // are atomically written.
                // 
                // On top of that we only use this variable internally and
                // we'll never do anything with it if the value is unequal to 0.
                m_SyncPoint = 0;
            }
        }

        /// <summary>
        /// Marks the current time with the specified marker.
        /// </summary>
        /// <param name="progressMark">The progress mark.</param>
        public void Mark(IProgressMark progressMark)
        {
            m_CurrentMark = progressMark;
            RaiseMarkAdded(progressMark);
        }

        /// <summary>
        /// Occurs when a new mark is provided to the tracker.
        /// </summary>
        public event EventHandler<ProgressMarkEventArgs> MarkAdded;

        /// <summary>
        /// Raises the mark added event.
        /// </summary>
        /// <param name="mark">The progress mark.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method raises the MarkAdded event.")]
        private void RaiseMarkAdded(IProgressMark mark)
        {
            var local = MarkAdded;
            if (local != null)
            { 
                local(this, new ProgressMarkEventArgs(mark));
            }
        }

        /// <summary>
        /// Occurs when there is a change in the progress of the system
        /// startup.
        /// </summary>
        public event EventHandler<StartupProgressEventArgs> StartupProgress;

        /// <summary>
        /// Raises the startup progress event with the specified values.
        /// </summary>
        /// <param name="progress">The progress percentage. Should be between 0 and 100.</param>
        /// <param name="currentlyProcessing">The description of what is currently being processed.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to fire an event.")]
        private void RaiseStartupProgress(int progress, IProgressMark currentlyProcessing)
        {
            var local = StartupProgress;
            if (local != null)
            { 
                local(this, new StartupProgressEventArgs(progress, currentlyProcessing));
            }
        }

        /// <summary>
        /// Stops the tracking of the progress.
        /// </summary>
        public void StopTracking()
        {
            // Stop the progress timing. 
            StopProgressTimer();

            // Indicate that we got to the end.
            RaiseStartupProgress(s_FinishingProgress, m_CurrentMark);
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
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IDisposable disposable = m_ProgressTimer as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
