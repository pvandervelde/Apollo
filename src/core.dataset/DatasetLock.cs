//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// Tracks the locks for a dataset.
    /// </summary>
    internal sealed class DatasetLock : ITrackDatasetLocks, IDisposable
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection of current read locks.
        /// </summary>
        private readonly List<DatasetLockKey> m_ReadLocks
            = new List<DatasetLockKey>();

        /// <summary>
        /// The collection of current write locks.
        /// </summary>
        private readonly List<DatasetLockKey> m_WriteLocks
            = new List<DatasetLockKey>();

        /// <summary>
        /// The event that indicates if there are any read locks.
        /// </summary>
        private readonly ManualResetEvent m_NoReadLocks = new ManualResetEvent(true);

        /// <summary>
        /// The event that signals if there are any write locks.
        /// </summary>
        private readonly ManualResetEvent m_NoWriteLocks = new ManualResetEvent(true);

        /// <summary>
        /// Indicates if the current endpoint has been disposed.
        /// </summary>
        private volatile bool m_IsDisposed = false;

        /// <summary>
        /// Locks the dataset for writing purposes.
        /// </summary>
        /// <returns>The key for the current request.</returns>
        public DatasetLockKey LockForWriting()
        {
            DatasetLockKey result = null;

            var sendEvent = false;
            var success = false;
            while (!success)
            {
                // Wait for all the locks to be removed
                m_NoReadLocks.WaitOne();
                lock (m_Lock)
                {
                    // Now check that all the locks are really, really gone
                    // If they're not then we just go around again.
                    if (m_ReadLocks.Count == 0)
                    {
                        sendEvent = m_WriteLocks.Count == 0;

                        result = new DatasetLockKey();
                        m_WriteLocks.Add(result);
                        m_NoWriteLocks.Reset();

                        success = true;
                    }
                }
            }

            if (sendEvent)
            {
                RaiseOnLockForWriting();
            }

            return result;
        }

        /// <summary>
        /// Removes a write lock.
        /// </summary>
        /// <param name="key">The key for the request.</param>
        public void RemoveWriteLock(DatasetLockKey key)
        {
            var sendEvent = false;
            lock (m_Lock)
            {
                if (m_WriteLocks.Contains(key))
                {
                    m_WriteLocks.Remove(key);
                }

                if (m_WriteLocks.Count == 0)
                {
                    sendEvent = true;
                    m_NoWriteLocks.Set();
                }
            }

            if (sendEvent)
            {
                RaiseOnUnlockFromWriting();
            }
        }

        /// <summary>
        /// Locks the dataset for reading.
        /// </summary>
        /// <returns>The key for the current lock request.</returns>
        public DatasetLockKey LockForReading()
        {
            DatasetLockKey result = null;

            var sendEvent = false;
            var success = false;
            while (!success)
            {
                // Wait for all the locks to be removed
                m_NoWriteLocks.WaitOne();
                lock (m_Lock)
                {
                    // Now check that all the locks are really, really gone
                    // If they're not then we just go around again.
                    if (m_WriteLocks.Count == 0)
                    {
                        sendEvent = m_ReadLocks.Count == 0;

                        result = new DatasetLockKey();
                        m_ReadLocks.Add(result);
                        m_NoReadLocks.Reset();

                        success = true;
                    }
                }
            }

            if (sendEvent)
            {
                RaiseOnLockForReading();
            }

            return result;
        }

        /// <summary>
        /// Removes a read lock.
        /// </summary>
        /// <param name="key">The key for the lock layer.</param>
        public void RemoveReadLock(DatasetLockKey key)
        {
            var sendEvent = false;
            lock (m_Lock)
            {
                if (m_ReadLocks.Contains(key))
                {
                    m_ReadLocks.Remove(key);
                }

                if (m_ReadLocks.Count == 0)
                {
                    sendEvent = true;
                    m_NoReadLocks.Set();
                }
            }

            if (sendEvent)
            {
                RaiseOnUnlockFromReading();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is currently considered
        /// locked for writing or not.
        /// </summary>
        public bool IsLockedForWriting
        {
            get
            {
                lock (m_Lock)
                {
                    return m_WriteLocks.Count > 0;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is currently considered
        /// locked for reading or not.
        /// </summary>
        public bool IsLockedForReading
        {
            get
            {
                lock (m_Lock)
                {
                    return m_ReadLocks.Count > 0;
                }
            }
        }

        /// <summary>
        /// An event raised if the write lock is engaged.
        /// </summary>
        public event EventHandler<EventArgs> OnLockForWriting;

        private void RaiseOnLockForWriting()
        {
            var local = OnLockForWriting;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised if the lock for writing is removed.
        /// </summary>
        public event EventHandler<EventArgs> OnUnlockFromWriting;

        private void RaiseOnUnlockFromWriting()
        {
            var local = OnUnlockFromWriting;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised if the lock for reading is engaged.
        /// </summary>
        public event EventHandler<EventArgs> OnLockForReading;

        private void RaiseOnLockForReading()
        {
            var local = OnLockForReading;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised if the lock for reading is removed.
        /// </summary>
        public event EventHandler<EventArgs> OnUnlockFromReading;

        private void RaiseOnUnlockFromReading()
        {
            var local = OnUnlockFromReading;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_IsDisposed)
            {
                // We've already disposed of the channel. Job done.
                return;
            }

            m_IsDisposed = true;
            m_NoReadLocks.Dispose();
            m_NoWriteLocks.Dispose();
        }
    }
}
