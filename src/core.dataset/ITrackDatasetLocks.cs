//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Base;

namespace Apollo.Core.Dataset
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
    internal interface ITrackDatasetLocks
    {
        /// <summary>
        /// Blocks the lock from engaging. Multiple block requests can be
        /// layered.
        /// </summary>
        /// <returns>The key for the current lock request.</returns>
        DatasetLockKey BlockLocking();

        /// <summary>
        /// Unblocks the lock and allows it to engage.
        /// </summary>
        /// <param name="key">The key for the block request.</param>
        void UnblockLocking(DatasetLockKey key);

        /// <summary>
        /// Requests another lock to be taken out for the dataset. Multiple 
        /// lock requests can be issued. 
        /// </summary>
        /// <returns>The key for the current lock request.</returns>
        DatasetLockKey Lock();

        /// <summary>
        /// Removes the lock layer that coincides with the given key.
        /// </summary>
        /// <param name="key">The key for the lock layer.</param>
        void Unlock(DatasetLockKey key);

        /// <summary>
        /// Gets a value indicating whether the dataset is currently considered
        /// blocked or not.
        /// </summary>
        bool IsBlocked
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is currently considered
        /// locked or not.
        /// </summary>
        bool IsLocked
        {
            get;
        }

        /// <summary>
        /// Returns an object that describes the lock state of the dataset.
        /// </summary>
        /// <returns>The lock state of the dataset.</returns>
        DatasetLockInformation LockState();
    }
}
