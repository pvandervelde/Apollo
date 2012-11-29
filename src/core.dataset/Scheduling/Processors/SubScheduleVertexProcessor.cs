//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    /// <summary>
    /// Defines the actions taken when a <see cref="ExecutableSubScheduleVertex"/> is encountered while processing
    /// an executable schedule.
    /// </summary>
    internal sealed class SubScheduleVertexProcessor : IProcesExecutableScheduleVertices
    {
        /// <summary>
        /// The object that distributes the execution of schedules.
        /// </summary>
        private readonly IDistributeScheduleExecutions m_Executor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubScheduleVertexProcessor"/> class.
        /// </summary>
        /// <param name="executor">The executor that handles the execution of the sub-schedule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="executor"/> is <see langword="null" />.
        /// </exception>
        public SubScheduleVertexProcessor(IDistributeScheduleExecutions executor)
        {
            {
                Lokad.Enforce.Argument(() => executor);
            }

            m_Executor = executor;
        }

        /// <summary>
        /// Gets the type of the <see cref="IScheduleVertex"/> that will be processed by
        /// this processor.
        /// </summary>
        public Type VertexTypeToProcess
        {
            get
            {
                return typeof(ExecutableSubScheduleVertex);
            }
        }

        /// <summary>
        /// Processes action provided by the given vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="executionInfo">The object that stores the information about the execution of the schedule.</param>
        /// <returns>A value indicating if the execution of the schedule should continue.</returns>
        public ScheduleExecutionState Process(IScheduleVertex vertex, ScheduleExecutionInfo executionInfo)
        {
            var subScheduleVertex = vertex as ExecutableSubScheduleVertex;
            if (subScheduleVertex == null)
            {
                Debug.Assert(false, "The vertex is of the incorrect type.");
                return ScheduleExecutionState.IncorrectProcessorForVertex;
            }

            if (executionInfo.Cancellation.IsCancellationRequested)
            {
                return ScheduleExecutionState.Canceled;
            }

            if (executionInfo.PauseHandler.IsPaused)
            {
                executionInfo.PauseHandler.WaitForUnPause(executionInfo.Cancellation);
            }

            // And how do we deal with loops? We'll cache the schedule if we're in-process but what about being out-of-process? We'll need to 
            // keep the remote app alive ...
            // --> We'll need some kind of class that handles making the decision for in-process / out-of-process. 
            //     * If it's small then we want to do in-process because the loading of a new dataset takes a while
            //     * If it's a parametrized thing then we might want to do out-of-process because we can run many more in one go
            //
            // Determine if we're running the sub-simulation synchronous (in-process) or ascynchronous (out-of-process)
            // Depends on:
            // - If the original schedule is allowed to split out sub-schedules out-of-process (when running in interactive mode we may 
            //   not allow this)
            // - 'size' of the sub-schedule
            // 
            // Determine if we should go out of process here
            // Going out of process only makes sense if the overhead of going out of process is less than 
            // the overhead of staying in process.
            // The overhead of going out of process is:
            // * Serialize all the data we need
            // * Load an application
            // * Stream the data across
            // * Deserialize data
            // * Re-serialize the data that we need (this may be less than what we send across)
            // * Send data across
            // * Deserialize data in original app
            // The overhead for staying in process is given by the fact that we can only run one schedule
            // at the time
            //
            // So going out of process makes sense when:
            // * There are multiple sub-schedules before the next sync block 
            //   ==> that means we can run all of them in parallel and the sync block indicates when we'll need the data
            // * We want to run the same sub-schedule with multiple parameters
            //   ==> Run the parameter values all at the same time
            // * We want to split the current calculation out into multiple bits (domain decomposition type)
            // 
            // Can we determine this when we build the schedule?
            // Possibly for some sections?
            bool executeOutOfProcess = false;
            IEnumerable<IScheduleVariable> parameters = null;
            var executor = m_Executor.Execute(subScheduleVertex.SubSchedule, parameters, executionInfo, executeOutOfProcess);

            // if we're running in-process then we probably have to wait for the sub-schedule to
            // finish executing because we don't want to have to make the schedule execution thread safe
            // If it's running out-of-process then we don't bother waiting. There should be a sync block
            // around this section that will sort out the variable synchronization.
            if (executor.IsLocal)
            {
                var resetEvent = new AutoResetEvent(false);
                var wrapperWait = Observable.FromEventPattern<ScheduleExecutionStateEventArgs>(
                        h => executor.OnFinish += h,
                        h => executor.OnFinish -= h)
                    .Take(1)
                    .Subscribe(args => resetEvent.Set());

                using (wrapperWait)
                {
                    resetEvent.WaitOne();
                }
            }

            return ScheduleExecutionState.Executing;
        }
    }
}
