//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines the interface for objects that provide lock tracking capabilities
    /// for datasets.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Datasets can be 'locked' against changes from the outside (but never against
    /// changes from the inside). This allows a dataset to execute a schedule without
    /// having to worry that the given schedule or its components are changed 
    /// while the schedule is being executed.
    /// </para>
    /// <para>
    /// Each lock requests adds an additional layer. Each unlock request removes the
    /// specific layer that matches up with the unlocking key. Once all locking layers
    /// have been removed then, and only then, will the dataset be considered available
    /// for changes.
    /// </para>
    /// </remarks>
    public interface ITrackDatasetLocks
    {
        /// <summary>
        /// Locks the dataset for writing purposes.
        /// </summary>
        /// <returns>The key for the current request.</returns>
        DatasetLockKey LockForWriting();

        /// <summary>
        /// Removes a write lock.
        /// </summary>
        /// <param name="key">The key for the request.</param>
        void RemoveWriteLock(DatasetLockKey key);

        /// <summary>
        /// Locks the dataset for reading.
        /// </summary>
        /// <returns>The key for the current request.</returns>
        DatasetLockKey LockForReading();

        /// <summary>
        /// Removes a read lock.
        /// </summary>
        /// <param name="key">The key for the request.</param>
        void RemoveReadLock(DatasetLockKey key);

        /// <summary>
        /// Gets a value indicating whether the dataset is currently considered
        /// locked for writing or not.
        /// </summary>
        bool IsLockedForWriting
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is currently considered
        /// locked for reading or not.
        /// </summary>
        bool IsLockedForReading
        {
            get;
        }

        /// <summary>
        /// An event raised if the write lock is engaged.
        /// </summary>
        event EventHandler<EventArgs> OnLockForWriting;

        /// <summary>
        /// An event raised if the lock for writing is removed.
        /// </summary>
        event EventHandler<EventArgs> OnUnlockFromWriting;

        /// <summary>
        /// An event raised if the lock for reading is engaged.
        /// </summary>
        event EventHandler<EventArgs> OnLockForReading;

        /// <summary>
        /// An event raised if the lock for reading is removed.
        /// </summary>
        event EventHandler<EventArgs> OnUnlockFromReading;
    }
}
