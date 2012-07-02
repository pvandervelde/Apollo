﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities;
using QuickGraph;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Creates executors for the execution of a schedule.
    /// </summary>
    internal sealed class ScheduleDistributor : IDistributeScheduleExecutions
    {
        private sealed class ExecutingScheduleKey : IEquatable<ExecutingScheduleKey>
        {
            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(ExecutingScheduleKey first, ExecutingScheduleKey second)
            {
                // Check if first is a null reference by using ReferenceEquals because
                // we overload the == operator. If first isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
                {
                    return true;
                }

                var nonNullObject = first;
                var possibleNullObject = second;
                if (ReferenceEquals(first, null))
                {
                    nonNullObject = second;
                    possibleNullObject = first;
                }

                return nonNullObject.Equals(possibleNullObject);
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(ExecutingScheduleKey first, ExecutingScheduleKey second)
            {
                // Check if first is a null reference by using ReferenceEquals because
                // we overload the == operator. If first isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
                {
                    return false;
                }

                var nonNullObject = first;
                var possibleNullObject = second;
                if (ReferenceEquals(first, null))
                {
                    nonNullObject = second;
                    possibleNullObject = first;
                }

                return !nonNullObject.Equals(possibleNullObject);
            }

            /// <summary>
            /// The ID of the schedule that is being executed.
            /// </summary>
            private readonly ScheduleId m_Id;

            /// <summary>
            /// The variables that were provided to the executor at the start of the schedule execution.
            /// </summary>
            private readonly IEnumerable<IScheduleVariable> m_Variables;

            /// <summary>
            /// The hash code for the current object. Stored here because the collection of variables
            /// may be large which makes recalculating the hash potentially slow.
            /// </summary>
            private readonly int m_Hash;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExecutingScheduleKey"/> class.
            /// </summary>
            /// <param name="id">The ID of the schedule that is being executed.</param>
            /// <param name="variables">The variables that were provided to the executor at the start of the schedule execution.</param>
            public ExecutingScheduleKey(ScheduleId id, IEnumerable<IScheduleVariable> variables)
            {
                {
                    Debug.Assert(id != null, "The schedule ID should not be a null reference.");
                }

                m_Id = id;
                m_Variables = variables;
                m_Hash = CalculateHashCode();
            }

            /// <summary>
            /// Gets the ID of the schedule being executed.
            /// </summary>
            public ScheduleId Schedule
            {
                get
                {
                    return m_Id;
                }
            }

            /// <summary>
            /// Gets the collection of variables for this execution.
            /// </summary>
            public IEnumerable<IScheduleVariable> Variables
            {
                get
                {
                    return m_Variables;
                }
            }

            private int CalculateHashCode()
            {
                // As obtained from the Jon Skeet answer to:
                // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
                // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
                //
                // Overflow is fine, just wrap
                unchecked
                {
                    // Pick a random prime number
                    int hash = 17;

                    // Mash the hash together with yet another random prime number
                    hash = (hash * 23) ^ m_Id.GetHashCode();
                    if (m_Variables != null)
                    {
                        foreach (var variable in m_Variables)
                        {
                            hash = (hash * 23) ^ variable.GetHashCode();
                        }
                    }

                    return hash;
                }
            }

            /// <summary>
            /// Determines whether the specified <see cref="ExecutingScheduleKey"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The <see cref="ExecutingScheduleKey"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="ExecutingScheduleKey"/> is equal to this instance;
            ///     otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool Equals(ExecutingScheduleKey other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // Check if other is a null reference by using ReferenceEquals because
                // we overload the == operator. If other isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                return !ReferenceEquals(other, null) 
                    && m_Id.Equals(other.m_Id)
                    && (((m_Variables == null) && (other.m_Variables == null)) 
                        || ((m_Variables != null) && (other.m_Variables != null) && m_Variables.SequenceEqual(other.m_Variables)));
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
            ///     otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public sealed override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                var id = obj as ExecutingScheduleKey;
                return Equals(id);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                return m_Hash;
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format(
                    "Key: {0} - {1}",
                    m_Id,
                    m_Variables.Aggregate<IScheduleVariable, string>(string.Empty, (text, variable) => { return text + variable.ToString(); }));
            }
        }

        /// <summary>
        /// The collection that maps an <see cref="IEditableScheduleVertex"/> type to a function that can create the
        /// equivalent <see cref="IExecutableScheduleVertex"/>.
        /// </summary>
        private static readonly Dictionary<Type, Func<IEditableScheduleVertex, IExecutableScheduleVertex>> s_VertexBuilder
            = new Dictionary<Type, Func<IEditableScheduleVertex, IExecutableScheduleVertex>>
            {
                { 
                    typeof(EditableStartVertex), 
                    vertex => new ExecutableStartVertex(vertex.Index) 
                },
                { 
                    typeof(EditableEndVertex), 
                    vertex => new ExecutableEndVertex(vertex.Index) 
                },
                { 
                    typeof(EditableInsertVertex), 
                    vertex => new ExecutableNoOpVertex(vertex.Index) 
                },
                { 
                    typeof(EditableMarkHistoryVertex), 
                    vertex => new ExecutableMarkHistoryVertex(vertex.Index) 
                },
                { 
                    typeof(EditableSynchronizationStartVertex), 
                    vertex => new ExecutableSynchronizationStartVertex(
                        vertex.Index, 
                        ((EditableSynchronizationStartVertex)vertex).VariablesToSynchronizeOn)
                },
                { 
                    typeof(EditableSynchronizationEndVertex), 
                    vertex => new ExecutableSynchronizationEndVertex(vertex.Index) 
                },
                {
                    typeof(EditableExecutingActionVertex),
                    vertex => new ExecutableActionVertex(
                        vertex.Index,
                        ((EditableExecutingActionVertex)vertex).ActionToExecute)
                },
                { 
                    typeof(EditableSubScheduleVertex), 
                    vertex => new ExecutableSubScheduleVertex(
                        vertex.Index, 
                        ((EditableSubScheduleVertex)vertex).ScheduleToExecute) 
                },
            };

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection of executors that are currently running.
        /// </summary>
        private readonly Dictionary<ExecutingScheduleKey, IExecuteSchedules> m_RunningExecutors
            = new Dictionary<ExecutingScheduleKey, IExecuteSchedules>();

        /// <summary>
        /// The collection of known schedules.
        /// </summary>
        private readonly IStoreSchedules m_KnownSchedules;

        /// <summary>
        /// The object that handles sending out the events.
        /// </summary>
        private readonly IScheduleExecutionNotificationInvoker m_Notifications;

        /// <summary>
        /// The function that creates an <see cref="IExecuteSchedules"/> object with the given 
        /// executable schedule.
        /// </summary>
        private readonly Func<ExecutableSchedule, ScheduleExecutionInfo, IExecuteSchedules> m_LoadExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDistributor"/> class.
        /// </summary>
        /// <param name="knownSchedules">The collection of known schedules.</param>
        /// <param name="notifications">The object that handles the notifications for the schedule execution.</param>
        /// <param name="executorBuilder">The function that is used to create a new <see cref="IExecuteSchedules"/> object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="knownSchedules"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="executorBuilder"/> is <see langword="null" />.
        /// </exception>
        public ScheduleDistributor(
            IStoreSchedules knownSchedules,
            IScheduleExecutionNotificationInvoker notifications,
            Func<ExecutableSchedule, ScheduleExecutionInfo, IExecuteSchedules> executorBuilder)
        {
            {
                Lokad.Enforce.Argument(() => knownSchedules);
                Lokad.Enforce.Argument(() => notifications);
                Lokad.Enforce.Argument(() => executorBuilder);
            }

            m_KnownSchedules = knownSchedules;
            m_Notifications = notifications;
            m_LoadExecutor = executorBuilder;
        }

        /// <summary>
        /// Determines the most suitable execution location for the given schedule and then starts the execution in that location.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule that should be executed.</param>
        /// <param name="scheduleParameters">The collection of parameters that have to be provided to the schedule before executing.</param>
        /// <param name="executionInfo">The object that provides information about the schedule that is currently being executed.</param>
        /// <param name="executeOutOfProcess">A flag indicating if the schedule should be executed in another processor or not.</param>
        /// <returns>
        /// The token that is related to the execution of the given schedule.
        /// </returns>
        /// <remarks>
        /// For local schedules the cancellation token is checked regularly and the schedule execution is cancelled on request. For
        /// remote schedule execution setting the cancellation token means that a termination signal is send to the remote
        /// application.
        /// </remarks>
        public IExecuteSchedules Execute(
            ScheduleId scheduleId, 
            IEnumerable<IScheduleVariable> scheduleParameters = null, 
            ScheduleExecutionInfo executionInfo = null, 
            bool executeOutOfProcess = false)
        {
            lock (m_Lock)
            {
                if (!m_KnownSchedules.Contains(scheduleId))
                {
                    throw new UnknownScheduleException();
                }

                var key = new ExecutingScheduleKey(scheduleId, scheduleParameters);
                if (m_RunningExecutors.ContainsKey(key))
                {
                    return m_RunningExecutors[key];
                }

                if (executeOutOfProcess)
                {
                    return ExecuteOutOfProcess(scheduleId, scheduleParameters, executionInfo);
                }
                else 
                {
                    return ExecuteInProcess(scheduleId, scheduleParameters, executionInfo);
                }
            }
        }

        private IExecuteSchedules ExecuteOutOfProcess(
            ScheduleId scheduleId, 
            IEnumerable<IScheduleVariable> scheduleParameters, 
            ScheduleExecutionInfo executionInfo)
        {
            // Determine what data needs to be serialized for schedule to execute
            // Serialize all data + components + schedules
            // Start external app
            // Feed data stream of data + components + schedules
            // Indicate which schedule should be executed + parameters
            // Start
            // Return distributed schedule executor
            // --> This is all based on a set of components that can handle all kinds of distribution methods / desires
            //     For instance we eventually also want to do Domain Decomp this way.
            throw new NotImplementedException();
        }

        private IExecuteSchedules ExecuteInProcess(
            ScheduleId scheduleId, 
            IEnumerable<IScheduleVariable> scheduleParameters, 
            ScheduleExecutionInfo executionInfo)
        {
            // Translate the schedule to an executable schedule
            var editableSchedule = m_KnownSchedules.Schedule(scheduleId);
            var executableSchedule = TransformToExecutableSchedule(scheduleId, editableSchedule);

            // Create a new executor and provide it with the schedule
            var executor = m_LoadExecutor(executableSchedule, executionInfo);
            {
                // Attach to events. We want to remove the executor from the collection as soon as it's finished
                executor.OnStart += HandleScheduleExecutionStart;
                executor.OnPause += HandleScheduleExecutionPause;
                executor.OnFinish += HandleScheduleExecutionFinish;
                executor.OnExecutionProgress += HandleScheduleExecutionProgress;
                executor.OnVertexProcess += HandleScheduleExecutionVertexProgress;
                m_RunningExecutors.Add(new ExecutingScheduleKey(scheduleId, scheduleParameters), executor);
                
                executor.Start(scheduleParameters);
            }

            return executor;
        }

        private ExecutableSchedule TransformToExecutableSchedule(ScheduleId id, IEditableSchedule editableSchedule)
        {
            var map = new Dictionary<IEditableScheduleVertex, IExecutableScheduleVertex>();
            var newSchedule = new AdjacencyGraph<IExecutableScheduleVertex, ExecutableScheduleEdge>();

            var start = new ExecutableStartVertex(editableSchedule.Start.Index);
            newSchedule.AddVertex(start);
            map.Add(editableSchedule.Start, start);

            var end = new ExecutableEndVertex(editableSchedule.End.Index);
            newSchedule.AddVertex(end);
            map.Add(editableSchedule.End, end);
            
            editableSchedule.TraverseSchedule(
                editableSchedule.Start,
                true,
                (vertex, edges) =>
                    {
                        foreach (var pair in edges)
                        {
                            var target = pair.Item2;
                            if (!map.ContainsKey(target))
                            {
                                var executableVertex = s_VertexBuilder[target.GetType()](target);
                                map.Add(target, executableVertex);
                                newSchedule.AddVertex(executableVertex);
                            }

                            var executableSource = map[vertex];
                            var executableTarget = map[target];
                            newSchedule.AddEdge(new ExecutableScheduleEdge(executableSource, executableTarget, pair.Item1));
                        }

                        return true;
                    });

            return new ExecutableSchedule(id, newSchedule, start, end);
        }

        private void HandleScheduleExecutionStart(object sender, EventArgs e)
        {
            var executor = sender as IExecuteSchedules;
            m_Notifications.RaiseOnStart(executor.Schedule);
        }

        private void HandleScheduleExecutionPause(object sender, EventArgs e)
        {
            var executor = sender as IExecuteSchedules;
            m_Notifications.RaiseOnPause(executor.Schedule);
        }

        private void HandleScheduleExecutionFinish(object sender, ScheduleExecutionStateEventArgs e)
        {
            lock (m_Lock)
            {
                var executor = sender as IExecuteSchedules;
                Debug.Assert(executor != null, "Received the event from a non-IExecuteSchedules object.");

                executor.OnStart -= HandleScheduleExecutionStart;
                executor.OnPause -= HandleScheduleExecutionPause;
                executor.OnFinish -= HandleScheduleExecutionFinish;
                executor.OnExecutionProgress -= HandleScheduleExecutionProgress;
                executor.OnVertexProcess -= HandleScheduleExecutionVertexProgress;

                m_Notifications.RaiseOnFinish(executor.Schedule);
                m_RunningExecutors.Remove(new ExecutingScheduleKey(executor.Schedule, executor.Parameters));
            }
        }

        private void HandleScheduleExecutionProgress(object sender, ProgressEventArgs e)
        {
            var executor = sender as IExecuteSchedules;
            m_Notifications.RaiseOnExecutionProgress(e.Progress, e.CurrentlyProcessing);
        }

        private void HandleScheduleExecutionVertexProgress(object sender, ExecutingVertexEventArgs e)
        {
            var executor = sender as IExecuteSchedules;
            m_Notifications.RaiseOnVertexProcess(executor.Schedule, e.Vertex);
        }
    }
}
