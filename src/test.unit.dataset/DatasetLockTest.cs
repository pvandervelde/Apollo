//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Core.Base;
using MbUnit.Framework;

namespace Apollo.Core.Dataset
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetLockTest
    {
        [Test]
        public void LockForWritingWithNoLocks()
        {
            var datasetLock = new DatasetLock();
            Assert.IsFalse(datasetLock.IsLockedForReading);
            Assert.IsFalse(datasetLock.IsLockedForWriting);

            var eventWasRaised = false;
            datasetLock.OnLockForWriting += (s, e) => { eventWasRaised = true; };
            var key = datasetLock.LockForWriting();
            
            Assert.IsNotNull(key);
            Assert.IsTrue(eventWasRaised);
            Assert.IsTrue(datasetLock.IsLockedForWriting);
            Assert.IsFalse(datasetLock.IsLockedForReading);
        }

        [Test]
        public void LockForWritingWithReadLocks()
        {
            var datasetLock = new DatasetLock();
            Assert.IsFalse(datasetLock.IsLockedForReading);
            Assert.IsFalse(datasetLock.IsLockedForWriting);

            var readLockEventRaised = false;
            var readUnlockEventRaised = false;
            var writeLockEventRaised = false;
            datasetLock.OnLockForReading += (s, e) => { readLockEventRaised = true; };
            datasetLock.OnUnlockFromReading += (s, e) => { readUnlockEventRaised = true; };
            datasetLock.OnLockForWriting += (s, e) => { writeLockEventRaised = true; };
            var key1 = datasetLock.LockForReading();

            var task = Task<DatasetLockKey>.Factory.StartNew(
                () =>
                {
                    return datasetLock.LockForWriting();
                });

            Assert.IsTrue(readLockEventRaised);
            Assert.IsFalse(readUnlockEventRaised);
            Assert.IsFalse(writeLockEventRaised);

            var key2 = datasetLock.LockForReading();
            Assert.IsNotNull(key2);
            Assert.IsTrue(readLockEventRaised);
            Assert.IsFalse(readUnlockEventRaised);
            Assert.IsFalse(writeLockEventRaised);

            datasetLock.RemoveReadLock(key1);
            Assert.IsTrue(readLockEventRaised);
            Assert.IsFalse(readUnlockEventRaised);
            Assert.IsFalse(writeLockEventRaised);

            datasetLock.RemoveReadLock(key2);
            var key3 = task.Result;

            Assert.IsNotNull(key3);
            Assert.IsTrue(readLockEventRaised);
            Assert.IsTrue(readUnlockEventRaised);
            Assert.IsTrue(writeLockEventRaised);
        }

        [Test]
        public void LockForReadingWithNoLocks()
        {
            var datasetLock = new DatasetLock();
            Assert.IsFalse(datasetLock.IsLockedForReading);
            Assert.IsFalse(datasetLock.IsLockedForWriting);

            var eventWasRaised = false;
            datasetLock.OnLockForReading += (s, e) => { eventWasRaised = true; };
            var key = datasetLock.LockForReading();

            Assert.IsNotNull(key);
            Assert.IsTrue(eventWasRaised);
            Assert.IsTrue(datasetLock.IsLockedForReading);
            Assert.IsFalse(datasetLock.IsLockedForWriting);
        }

        [Test]
        public void LockForReadingWithWriteLocks()
        {
            var datasetLock = new DatasetLock();
            Assert.IsFalse(datasetLock.IsLockedForReading);
            Assert.IsFalse(datasetLock.IsLockedForWriting);

            var writeLockEventRaised = false;
            var writeUnlockEventRaised = false;
            var readLockEventRaised = false;
            datasetLock.OnLockForWriting += (s, e) => { writeLockEventRaised = true; };
            datasetLock.OnUnlockFromWriting += (s, e) => { writeUnlockEventRaised = true; };
            datasetLock.OnLockForReading += (s, e) => { readLockEventRaised = true; };
            var key1 = datasetLock.LockForWriting();

            var task = Task<DatasetLockKey>.Factory.StartNew(
                () =>
                {
                    return datasetLock.LockForReading();
                });

            Assert.IsTrue(writeLockEventRaised);
            Assert.IsFalse(writeUnlockEventRaised);
            Assert.IsFalse(readLockEventRaised);

            var key2 = datasetLock.LockForWriting();
            Assert.IsNotNull(key2);
            Assert.IsTrue(writeLockEventRaised);
            Assert.IsFalse(writeUnlockEventRaised);
            Assert.IsFalse(readLockEventRaised);

            datasetLock.RemoveWriteLock(key1);
            Assert.IsTrue(writeLockEventRaised);
            Assert.IsFalse(writeUnlockEventRaised);
            Assert.IsFalse(readLockEventRaised);

            datasetLock.RemoveWriteLock(key2);
            var key3 = task.Result;

            Assert.IsNotNull(key3);
            Assert.IsTrue(writeLockEventRaised);
            Assert.IsTrue(writeUnlockEventRaised);
            Assert.IsTrue(readLockEventRaised);
        }
    }
}
