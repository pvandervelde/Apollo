//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Describes the lock state of the dataset.
    /// </summary>
    [Serializable]
    public sealed class DatasetLockInformation
    {
        /// <summary>
        /// The time the dataset lock was activated.
        /// </summary>
        private readonly DateTimeOffset m_LockTime;

        /// <summary>
        /// The reason the dataset lock was activated.
        /// </summary>
        private readonly DatasetLockReason m_Reason;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetLockInformation"/> class, indicating
        /// that the dataset is not locked.
        /// </summary>
        public DatasetLockInformation()
            : this(DatasetLockReason.None, DateTimeOffset.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetLockInformation"/> class.
        /// </summary>
        /// <param name="reason">The reason that the dataset is locked.</param>
        /// <param name="lockTime">The time the dataset was locked.</param>
        public DatasetLockInformation(DatasetLockReason reason, DateTimeOffset lockTime)
        {
            m_LockTime = lockTime;
            m_Reason = reason;
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is locked against changes
        /// from the outside.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return m_Reason == DatasetLockReason.None;
            }
        }

        /// <summary>
        /// Gets the reason the dataset was locked.
        /// </summary>
        public DatasetLockReason Reason
        {
            get
            {
                return m_Reason;
            }
        }

        /// <summary>
        /// Gets the time the lock was activated.
        /// </summary>
        public DateTimeOffset TimeLockWasActivated
        {
            get
            {
                return m_LockTime;
            }
        }
    }
}
