//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Scheduling
{
    /// <summary>
    /// Defines the different states the execution of the schedule can be in.
    /// </summary>
    public enum ScheduleExecutionState
    {
        /// <summary>
        /// There is no state because the schedule hasn't been started yet.
        /// </summary>
        None,

        /// <summary>
        /// The schedule is being executed.
        /// </summary>
        Executing,

        /// <summary>
        /// The schedule execution has been canceled by the user.
        /// </summary>
        Canceled,

        /// <summary>
        /// The schedule execution has been terminated for some reason.
        /// </summary>
        Terminated,

        /// <summary>
        /// The schedule execution has been stopped because the end of the schedule was reached.
        /// </summary>
        Completed,

        /// <summary>
        /// The schedule execution has been stopped because no processor was found for a given vertex.
        /// </summary>
        NoProcessorForVertex,

        /// <summary>
        /// The schedule execution has been stopped because the processor that was used was the 
        /// wrong type for the given vertex.
        /// </summary>
        IncorrectProcessorForVertex,

        /// <summary>
        /// The schedule execution has been stopped because there was no way to move from the current vertex
        /// to another vertex.
        /// </summary>
        NoTraversableEdgeFound,

        /// <summary>
        /// The schedule execution has been stopped because an unhandled exception occurred.
        /// </summary>
        UnhandledException,
    }
}
