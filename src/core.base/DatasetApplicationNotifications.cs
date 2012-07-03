//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines the notifications that allow a dataset to signal the outside world.
    /// </summary>
    internal sealed class DatasetApplicationNotifications : IDatasetApplicationNotificationInvoker
    {
        /// <summary>
        /// An event raised when progress is made on some action.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnProgress;

        /// <summary>
        /// Raises the <see cref="OnProgress"/> event.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="currentlyProcessing">The action that is currently being processed.</param>
        public void RaiseOnProgress(int progress, IProgressMark currentlyProcessing)
        {
            var local = OnProgress;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, currentlyProcessing));
            }
        }

        /// <summary>
        /// An event raised when the dataset is locked for reading.
        /// </summary>
        public event EventHandler<EventArgs> OnReadLock;

        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnReadLock"/> event.
        /// </summary>
        public void RaiseOnReadLock()
        {
            var local = OnReadLock;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the dataset is unlocked from reading.
        /// </summary>
        public event EventHandler<EventArgs> OnRemoveReadLock;

        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnRemoveReadLock"/> event.
        /// </summary>
        public void RaiseOnRemoveReadLock()
        {
            var local = OnRemoveReadLock;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }
    }
}
