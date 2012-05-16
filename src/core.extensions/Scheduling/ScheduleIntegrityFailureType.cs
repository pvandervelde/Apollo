//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the type of integrity failure found in a schedule by the
    /// <see cref="IVertifyScheduleIntegrity"/> object.
    /// </summary>
    public enum ScheduleIntegrityFailureType
    {
        /// <summary>
        /// No failure was found.
        /// </summary>
        None,

        /// <summary>
        /// The schedule is missing a start point.
        /// </summary>
        ScheduleIsMissingStart,
        
        /// <summary>
        /// The schedule is missing an end point.
        /// </summary>
        ScheduleIsMissingEnd,
        
        /// <summary>
        /// The current vertex cannot be reached from the start point.
        /// </summary>
        ScheduleVertexIsNotReachableFromStart,
        
        /// <summary>
        /// The schedule end point cannot be reached from the current vertex.
        /// </summary>
        ScheduleEndIsNotReachableFromVertex,

        /// <summary>
        /// The schedule ID is for a sub-schedule that is unknown.
        /// </summary>
        UnknownSubSchedule,
        
        /// <summary>
        /// The sub-schedule at the current vertex links back to the current 
        /// schedule.
        /// </summary>
        SubScheduleLinksBackToParentSchedule,
        
        /// <summary>
        /// There are multiple ways to traverse from the current vertex to another vertex.
        /// </summary>
        VertexLinksToOtherVertexInMultipleWays,
        
        /// <summary>
        /// A synchronization block is missing its start point.
        /// </summary>
        SynchronizationBlockMissingStart,
        
        /// <summary>
        /// A synchronization block is missing its end point.
        /// </summary>
        SynchronizationBlockMissingEnd,

        /// <summary>
        /// A synchronized variable is not updated inside the synchronization block.
        /// </summary>
        SynchronizedVariableIsNotUpdatedInsideBlock,
        
        /// <summary>
        /// An unknown failure was found.
        /// </summary>
        Unknown,
    }
}
