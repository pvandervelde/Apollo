//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;
using Lokad;
using NManto;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// Defines the commands that allow a dataset application to handle the dataset 
    /// persistence.
    /// </summary>
    internal sealed class DatasetApplicationCommands : IDatasetApplicationCommands
    {
        /// <summary>
        /// The collection of locking keys.
        /// </summary>
        private readonly ConcurrentStack<DatasetLockKey> m_EditKeys = new ConcurrentStack<DatasetLockKey>();

        /// <summary>
        /// The object that handles the communication with the remote endpoints.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// The object that tracks dataset locks.
        /// </summary>
        private readonly ITrackDatasetLocks m_DatasetLock;

        /// <summary>
        /// The action that closes the application.
        /// </summary>
        private readonly Action m_CloseAction;

        /// <summary>
        /// The action that loads the dataset from the given file path.
        /// </summary>
        private readonly Action<FileInfo> m_LoadAction;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetApplicationCommands"/> class.
        /// </summary>
        /// <param name="layer">The object that handles the communication with remote endpoints.</param>
        /// <param name="datasetLock">The object that handles the locking of the dataset for editing or running.</param>
        /// <param name="closeAction">The action that closes the application.</param>
        /// <param name="loadAction">The action that is used to load the dataset from a given file path.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="datasetLock"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="closeAction"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="loadAction"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown is <paramref name="systemDiagnostics"/> is <see langword="null"/>
        /// </exception>
        public DatasetApplicationCommands(
            ICommunicationLayer layer, 
            ITrackDatasetLocks datasetLock,
            Action closeAction,
            Action<FileInfo> loadAction,
            SystemDiagnostics systemDiagnostics,
            TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => layer);
                Enforce.Argument(() => closeAction);
                Enforce.Argument(() => loadAction);
                Enforce.Argument(() => systemDiagnostics);
            }

            m_Layer = layer;
            m_DatasetLock = datasetLock;
            m_CloseAction = closeAction;
            m_LoadAction = loadAction;
            m_Diagnostics = systemDiagnostics;
            m_Scheduler = scheduler ?? TaskScheduler.Default;
        }

        /// <summary>
        /// Loads the dataset into the dataset application.
        /// </summary>
        /// <param name="ownerId">The ID of the endpoint which requested the load of the dataset.</param>
        /// <param name="token">The token that indicates which file should be uploaded.</param>
        /// <returns>A task that will finish once the dataset is loaded.</returns>
        public Task Load(EndpointId ownerId, UploadToken token)
        {
            // This one is more tricky than you think. We need to return a Task (not a Task<T>), however
            // if we do Task<T>.ContinueWith we get a System.Threading.Tasks.ContinuationTaskFromResultTask<System.IO.Stream>
            // object. When we give that back to the command system it tries to push the stream across the network ... not
            // really what we want.
            //
            // So to fix this we create a task and then inside that task we load the file (via another task) and continue
            // of the nested task. The continuation task is a child of the global task and all is well because the 
            // global task doesn't finish until the nested child task is done, and we get a Task object back
            var globalTask = Task.Factory.StartNew(
                () =>
                {
                    var filePath = Path.GetTempFileName();

                    var source = new CancellationTokenSource();
                    var task = m_Layer.DownloadData(ownerId, token, filePath, null, source.Token, m_Scheduler);
                    task.ContinueWith(
                        t =>
                        {
                            using (var interval = m_Diagnostics.Profiler.Measure("Loading dataset from file."))
                            {
                                m_LoadAction(new FileInfo(filePath));
                            }
                        }, 
                        TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously);
                });

            return globalTask;
        }

        /// <summary>
        /// Closes the remote dataset application.
        /// </summary>
        /// <returns>A task that will finish once the application is closed.</returns>
        public Task Close()
        {
            // This one is tricky. We need to be able to send out the success message
            // for the shutdown task but we can't do that if we shut down the app,
            // so we create a fake do-nothing task to send out the command succes result,
            // then in the attached task we actually shut down. Draw back is that we 
            // can't report in that there is a problem but there is nothing we can do 
            // about that.
            var result = Task.Factory.StartNew(
                () => { },
                new CancellationToken(),
                TaskCreationOptions.None,
                m_Scheduler);

            result.ContinueWith(
                t => 
                {
                    // Just do this the ugly way. We're about to kill the app anyway
                    Thread.Sleep(2000);
                    m_CloseAction();
                },
                new CancellationToken(),
                TaskContinuationOptions.None,
                m_Scheduler);
            return result;
        }

        /// <summary>
        /// Indicates if the current dataset has changed since the last time it
        /// has been saved.
        /// </summary>
        /// <returns>
        ///     A task that will return a value indicating if the dataset has been
        ///     changed since the last save.
        /// </returns>
        public Task<bool> HasChanged()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Requests that the current dataset is saved and the saved copy is transfered
        /// back to the host machine.
        /// </summary>
        /// <returns>A task which will complete once the transfer is complete.</returns>
        public Task Save()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the dataset will be edited in the near future.
        /// </summary>
        /// <returns>A task which will complete once the dataset is ready for editing.</returns>
        public Task SwitchToEditMode()
        {
            return Task.Factory.StartNew(
                () =>
                {
                    var key = m_DatasetLock.LockForWriting();
                    m_EditKeys.Push(key);
                },
                TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Indicates that the dataset should prepare to execute a sequence.
        /// </summary>
        /// <returns>A task that will complete once the dataset is ready for executing.</returns>
        public Task SwitchToExecuteMode()
        {
            return Task.Factory.StartNew(
                () =>
                {
                    DatasetLockKey key;
                    if (m_EditKeys.TryPop(out key))
                    {
                        m_DatasetLock.RemoveWriteLock(key);
                    }
                },
                TaskCreationOptions.LongRunning);
        }
    }
}
