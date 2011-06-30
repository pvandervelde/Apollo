//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;
using Lokad;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines the commands that allow a dataset application to handle the dataset 
    /// persistence.
    /// </summary>
    internal sealed class DatasetApplicationCommands : IDatasetApplicationCommands
    {
        /// <summary>
        /// The object that handles the communication with the remote endpoints.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// The action that closes the application.
        /// </summary>
        private readonly Action m_CloseAction;

        /// <summary>
        /// The action that loads the dataset from the given file path.
        /// </summary>
        private readonly Action<FileInfo> m_LoadAction;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetApplicationCommands"/> class.
        /// </summary>
        /// <param name="layer">The object that handles the communication with remote endpoints.</param>
        /// <param name="closeAction">The action that closes the application.</param>
        /// <param name="loadAction">The action that is used to load the dataset from a given file path.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="closeAction"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="loadAction"/> is <see langword="null" />.
        /// </exception>
        public DatasetApplicationCommands(
            ICommunicationLayer layer, 
            Action closeAction,
            Action<FileInfo> loadAction,
            TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => layer);
                Enforce.Argument(() => closeAction);
                Enforce.Argument(() => loadAction);
            }

            m_Layer = layer;
            m_CloseAction = closeAction;
            m_LoadAction = loadAction;
            m_Scheduler = scheduler;
        }

        /// <summary>
        /// Loads the dataset into the dataset application.
        /// </summary>
        /// <param name="ownerId">The ID of the endpoint which requested the load of the dataset.</param>
        /// <param name="token">The token that indicates which file should be uploaded.</param>
        /// <returns>A task that will finish once the dataset is loaded.</returns>
        public Task Load(EndpointId ownerId, UploadToken token)
        {
            var filePath = Path.GetTempFileName();

            var source = new CancellationTokenSource();
            var task = m_Layer.DownloadData(ownerId, token, filePath, source.Token, m_Scheduler);
            return task.ContinueWith(
                t =>
                {
                    m_LoadAction(new FileInfo(filePath));
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Closes the remote dataset application.
        /// </summary>
        /// <returns>A task that will finish once the application is closed.</returns>
        public Task Close()
        {
            return Task.Factory.StartNew(
                () => m_CloseAction,
                new CancellationToken(),
                TaskCreationOptions.None,
                m_Scheduler);
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
        /// An event raised when the endpoint to which the commandset belongs
        /// becomes available or unavailable.
        /// </summary>
        /// <remarks>
        /// Note that changes in availability do not mean that the endpoint has
        /// permanently been terminated (although that may be the case). It merely
        /// means that the endpoint is temporarily not available.
        /// </remarks>
        public event EventHandler<EventArgs> OnAvailabilityChange;

        private void RaiseOnAvailabilityChange()
        {
            var local = OnAvailabilityChange;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the endpoint to which the command set belongs
        /// becomes invalid.
        /// </summary>
        public event EventHandler<EventArgs> OnTerminated;

        private void RaiseOnTerminated()
        {
            var local = OnTerminated;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }
    }
}
