//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines the commands for executing schedules.
    /// </summary>
    internal sealed class ScheduleExecutionCommands : IScheduleExecutionCommands
    {
        /// <summary>
        /// The collection of schedules for which execution has been requested via
        /// the <see cref="IScheduleExecutionCommands"/> interface.
        /// </summary>
        private readonly ConcurrentDictionary<ScheduleId, Tuple<IExecuteSchedules, DatasetLockKey>> m_RunningExecutors
            = new ConcurrentDictionary<ScheduleId, Tuple<IExecuteSchedules, DatasetLockKey>>();

        /// <summary>
        /// The object that handles distributing the execution of schedules.
        /// </summary>
        private readonly IDistributeScheduleExecutions m_Executor;

        /// <summary>
        /// The object that tracks lock requests for the current dataset.
        /// </summary>
        private readonly ITrackDatasetLocks m_DatasetLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleExecutionCommands"/> class.
        /// </summary>
        /// <param name="executor">The object that handles the schedule execution.</param>
        /// <param name="datasetLock">The object that tracks lock requests for the current dataset.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="executor"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="datasetLock"/> is <see langword="null" />.
        /// </exception>
        public ScheduleExecutionCommands(IDistributeScheduleExecutions executor, ITrackDatasetLocks datasetLock)
        {
            {
                Lokad.Enforce.Argument(() => executor);
                Lokad.Enforce.Argument(() => datasetLock);
            }

            m_Executor = executor;
            m_DatasetLock = datasetLock;
        }

        /// <summary>
        /// Starts the execution of the given schedule.
        /// </summary>
        /// <param name="scheduleToExecute">The ID of the schedule that should be executed.</param>
        /// <returns>A task that will finish once the schedule execution completes.</returns>
        public Task Start(ScheduleId scheduleToExecute)
        {
            var result = Task.Factory.StartNew(
                () =>
                {
                    var key = m_DatasetLock.LockForReading();

                    var executor = m_Executor.Execute(scheduleToExecute);
                    var resetEvent = new AutoResetEvent(false);
                    var wrapperWait = Observable.FromEventPattern<ScheduleExecutionStateEventArgs>(
                            h => executor.OnFinish += h,
                            h => executor.OnFinish -= h)
                        .Take(1)
                        .Subscribe(args => resetEvent.Set());

                    m_RunningExecutors[scheduleToExecute] = new Tuple<IExecuteSchedules, DatasetLockKey>(executor, key);
                    using (wrapperWait)
                    {
                        resetEvent.WaitOne();
                    }

                    Tuple<IExecuteSchedules, DatasetLockKey> pair;
                    m_RunningExecutors.TryRemove(scheduleToExecute, out pair);
                    m_DatasetLock.RemoveReadLock(key);
                },
                TaskCreationOptions.LongRunning);

            return result;
        }

        /// <summary>
        /// Pauses the execution of the given schedule.
        /// </summary>
        /// <param name="scheduleToPause">The ID of the schedule that should be executed.</param>
        /// <returns>A task that will finish once the schedule execution has been paused.</returns>
        public Task Pause(ScheduleId scheduleToPause)
        {
            var result = Task.Factory.StartNew(
                () =>
                {
                    Tuple<IExecuteSchedules, DatasetLockKey> pair;
                    var success = m_RunningExecutors.TryGetValue(scheduleToPause, out pair);

                    if (success)
                    {
                        pair.Item1.Pause();
                    }
                },
                TaskCreationOptions.None);

            return result;
        }

        /// <summary>
        /// Stops the execution of the given schedule.
        /// </summary>
        /// <param name="scheduleToStop">The ID of the schedule that should be executed.</param>
        /// <returns>A task that will finish once the schedule execution has been stopped.</returns>
        public Task Stop(ScheduleId scheduleToStop)
        {
            var result = Task.Factory.StartNew(
                () =>
                {
                    Tuple<IExecuteSchedules, DatasetLockKey> pair;
                    var success = m_RunningExecutors.TryGetValue(scheduleToStop, out pair);

                    if (success)
                    {
                        pair.Item1.Stop();
                    }
                },
                TaskCreationOptions.None);

            return result;
        }
    }
}
