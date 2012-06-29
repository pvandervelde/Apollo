//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dataset.Properties;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines the methods necessary for the execution of a schedule inside the current application.
    /// </summary>
    internal sealed class ScheduleExecutor : IExecuteSchedules
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The object that stores information about the currently running schedule.
        /// </summary>
        private readonly ScheduleExecutionInfo m_ExecutionInfo;

        /// <summary>
        /// The collection of objects that processes the actual schedule vertices.
        /// </summary>
        private readonly Dictionary<Type, IProcesExecutableScheduleVertices> m_Executors;

        /// <summary>
        /// The collection that contains all the schedule conditions.
        /// </summary>
        private readonly IStoreScheduleConditions m_Conditions;

        /// <summary>
        /// The schedule that is being executed.
        /// </summary>
        private readonly ExecutableSchedule m_Schedule;

        /// <summary>
        /// The parameters that were provided when the schedule execution started.
        /// </summary>
        private IEnumerable<IScheduleVariable> m_Parameters;

        /// <summary>
        /// The task that is used to execute the schedule.
        /// </summary>
        private Task m_ExecutionTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleExecutor"/> class.
        /// </summary>
        /// <param name="executors">The collection of vertex processors.</param>
        /// <param name="conditions">The collection of execution conditions.</param>
        /// <param name="schedule">The schedule that should be executed.</param>
        /// <param name="executionInfo">The object that stores information about the current running schedule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="executors"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="conditions"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="CannotExecuteScheduleWithoutProcessorsException">
        ///     Thrown if <paramref name="executors"/> is an empty collection.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        public ScheduleExecutor(
            IEnumerable<IProcesExecutableScheduleVertices> executors,
            IStoreScheduleConditions conditions,
            ExecutableSchedule schedule,
            ScheduleExecutionInfo executionInfo = null)
        {
            {
                Lokad.Enforce.Argument(() => executors);
                Lokad.Enforce.With<CannotExecuteScheduleWithoutProcessorsException>(
                    executors.Any(),
                    Resources.Exceptions_Messages_CannotExecuteScheduleWithoutProcessors);

                Lokad.Enforce.Argument(() => conditions);
                Lokad.Enforce.Argument(() => schedule);
            }

            m_Executors = executors.ToDictionary(v => v.VertexTypeToProcess, v => v);
            m_Conditions = conditions;
            m_Schedule = schedule;
            m_ExecutionInfo = new ScheduleExecutionInfo(executionInfo);
        }

        /// <summary>
        /// Gets the ID of the schedule that is being executed by the current executor.
        /// </summary>
        public ScheduleId Schedule
        {
            get
            {
                return m_Schedule.Id;
            }
        }

        /// <summary>
        /// Gets the collection of parameters that were provided when the schedule execution started.
        /// </summary>
        public IEnumerable<IScheduleVariable> Parameters
        {
            get
            {
                return m_Parameters;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the schedule is being executed in the current application or not.
        /// </summary>
        public bool IsLocal
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the schedule is currently being executed.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return m_ExecutionTask != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the schedule execution is currently paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return m_ExecutionInfo.PauseHandler.IsPaused;
            }
        }

        /// <summary>
        /// Starts the execution of the schedule if it is currently not being executed.
        /// </summary>
        /// <param name="scheduleParameters">The collection of parameters that have to be provided to the schedule before executing.</param>
        public void Start(IEnumerable<IScheduleVariable> scheduleParameters = null)
        {
            if (m_ExecutionInfo.PauseHandler.IsPaused)
            {
                RaiseOnStart();
                m_ExecutionInfo.PauseHandler.Unpause();
            }
            else
            {
                lock (m_Lock)
                {
                    // If we are already running then we don't do anything
                    if (m_ExecutionTask != null)
                    {
                        return;
                    }
                }

                RaiseOnStart();
                lock (m_Lock)
                {
                    // What are we going to do with the parameters?
                    m_Parameters = scheduleParameters;

                    // @Todo: Lock the dataset against changes from the outside
                    var token = m_ExecutionInfo.Cancellation;
                    m_ExecutionTask = Task.Factory.StartNew(
                        ExecuteSchedule, 
                        token,
                        TaskCreationOptions.LongRunning,
                        m_ExecutionInfo.TaskScheduler ?? TaskScheduler.Default);
                }
            }
        }

        // At some point we'd like to add:
        // Loop counters
        // tight loop detection
        // Allow pause to be more granular (e.g. only pause outside loops etc.)
        private void ExecuteSchedule()
        {
            var graph = m_Schedule.Graph;
            IExecutableScheduleVertex current = m_Schedule.Start;

            ScheduleExecutionState state = ScheduleExecutionState.Executing;
            Exception unhandledException = null;
            while (state == ScheduleExecutionState.Executing)
            {
                if (m_ExecutionInfo.Cancellation.IsCancellationRequested)
                {
                    state = ScheduleExecutionState.Cancelled;
                    continue;
                }

                // If we need to pause we do it here
                m_ExecutionInfo.PauseHandler.WaitForUnPause(m_ExecutionInfo.Cancellation);

                // Get the executor for the current node type and run it
                // on the current node. If we fail then we exit the loop
                {
                    var type = current.GetType();
                    if (!m_Executors.ContainsKey(type))
                    {
                        state = ScheduleExecutionState.NoProcessorForVertex;
                        continue;
                    }

                    RaiseOnVertexProcess(Schedule, current.Index);
                    var processor = m_Executors[type];
                    try
                    {
                        ScheduleExecutionState shouldContinue = processor.Process(current, m_ExecutionInfo);
                        if (shouldContinue != ScheduleExecutionState.Executing)
                        {
                            state = shouldContinue;
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        unhandledException = e;
                        state = ScheduleExecutionState.UnhandledException;
                        continue;
                    }
                }

                // Get all the outedges and find the first edge that we can traverse.
                // If we don't find any edges then we fail.
                {
                    var lastVertex = current;
                    var outEdges = graph.OutEdges(current);
                    foreach (var edge in outEdges)
                    {
                        if (m_ExecutionInfo.Cancellation.IsCancellationRequested)
                        {
                            state = ScheduleExecutionState.Cancelled;
                            break;
                        }

                        bool canTraverse = true;
                        if (edge.TraversingCondition != null)
                        {
                            Debug.Assert(m_Conditions.Contains(edge.TraversingCondition), "The traversing condition for the edge does not exist");
                            var condition = m_Conditions.Condition(edge.TraversingCondition);
                            try
                            {
                                canTraverse = condition.CanTraverse(m_ExecutionInfo.Cancellation);
                            }
                            catch (Exception e)
                            {
                                unhandledException = e;
                                state = ScheduleExecutionState.UnhandledException;
                                break;
                            }
                        }

                        if (canTraverse)
                        {
                            current = edge.Target;
                            break;
                        }
                    }

                    // If we get here then there were no edges we could traverse. Fail the execution
                    if (ReferenceEquals(lastVertex, current) && (state == ScheduleExecutionState.Executing))
                    {
                        state = ScheduleExecutionState.NoTraversableEdgeFound;
                    }
                }

                RaiseOnExecutionProgress(-1, new ScheduleExecutionProgressMark());
            }

            RaiseOnFinish(state);

            // clean-up, but only after we have notified everybody so that all the 
            // data is still there. Also we won't be able to restart after finishing
            lock (m_Lock)
            {
                m_ExecutionTask = null;
                m_Parameters = null;
            }
        }

        /// <summary>
        /// Pauses the execution of the schedule. The execution of the schedule
        /// can be resumed from this state.
        /// </summary>
        public void Pause()
        {
            RaiseOnPause();
            m_ExecutionInfo.PauseHandler.Pause();
        }

        /// <summary>
        /// Stops the execution of the schedule. The execution of the schedule cannot
        /// be resumed from this state.
        /// </summary>
        public void Stop()
        {
            Task task;
            lock (m_Lock)
            {
                if (m_ExecutionTask == null)
                {
                    return;
                }

                task = m_ExecutionTask;
                m_ExecutionTask = null;
            }

            m_ExecutionInfo.CancelScheduleExecution();
            task.Wait();
        }

        /// <summary>
        /// An event raised when the execution of the schedule starts.
        /// </summary>
        public event EventHandler<EventArgs> OnStart;

        private void RaiseOnStart()
        {
            var local = OnStart;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the execution of the schedule has been paused.
        /// </summary>
        public event EventHandler<EventArgs> OnPause;

        private void RaiseOnPause()
        {
            var local = OnPause;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when a vertex is being processed.
        /// </summary>
        public event EventHandler<ExecutingVertexEventArgs> OnVertexProcess;

        private void RaiseOnVertexProcess(ScheduleId schedule, int vertexIndex)
        {
            var local = OnVertexProcess;
            if (local != null)
            {
                local(this, new ExecutingVertexEventArgs(schedule, vertexIndex));
            }
        }

        /// <summary>
        /// An event raised when there is progress in the execution of the schedule.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnExecutionProgress;

        private void RaiseOnExecutionProgress(int progress, IProgressMark mark)
        {
            var local = OnExecutionProgress;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, mark));
            }
        }

        /// <summary>
        /// An event raised when the execution of the schedule has been stopped, either
        /// due to the user stopping the execution directly or if the schedule executor 
        /// reaches the end of the schedule.
        /// </summary>
        public event EventHandler<ScheduleExecutionStateEventArgs> OnFinish;

        private void RaiseOnFinish(ScheduleExecutionState state)
        {
            var local = OnFinish;
            if (local != null)
            {
                local(this, new ScheduleExecutionStateEventArgs(state));
            }
        }
    }
}
