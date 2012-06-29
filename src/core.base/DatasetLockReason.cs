//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines the different reasons for which a dataset may be locked for changes from the outside.
    /// </summary>
    public enum DatasetLockReason
    {
        /// <summary>
        /// There is no reason the dataset is locked. Only used when the dataset
        /// is in fact not locked.
        /// </summary>
        None,

        /// <summary>
        /// The dataset is locked because one or more schedules are executing or starting
        /// execution.
        /// </summary>
        ScheduleExecution,

        /// <summary>
        /// The reason for the dataset lock is unknown.
        /// </summary>
        Unknown,
    }
}
