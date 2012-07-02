//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.Core.Base;

namespace Apollo.Core.Dataset
{
    internal sealed class DatasetLock : ITrackDatasetLocks
    {
        /// <summary>
        /// Blocks the lock from engaging. Multiple block requests can be
        /// layered.
        /// </summary>
        /// <returns>The key for the current lock request.</returns>
        public DatasetLockKey BlockLocking()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unblocks the lock and allows it to engage.
        /// </summary>
        /// <param name="key">The key for the block request.</param>
        public void UnblockLocking(DatasetLockKey key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Requests another lock to be taken out for the dataset. Multiple 
        /// lock requests can be issued. 
        /// </summary>
        /// <returns>The key for the current lock request.</returns>
        public DatasetLockKey Lock()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the lock layer that coincides with the given key.
        /// </summary>
        /// <param name="key">The key for the lock layer.</param>
        public void Unlock(DatasetLockKey key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is currently considered
        /// blocked or not.
        /// </summary>
        public bool IsBlocked
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is currently considered
        /// locked or not.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns an object that describes the lock state of the dataset.
        /// </summary>
        /// <returns>The lock state of the dataset.</returns>
        public DatasetLockInformation LockState()
        {
            throw new NotImplementedException();
        }
    }
}
