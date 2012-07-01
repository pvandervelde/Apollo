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
        /// An event raised when the dataset is locked.
        /// </summary>
        public event EventHandler<EventArgs> OnLock;

        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnLock"/> event.
        /// </summary>
        public void RaiseOnLock()
        {
            var local = OnLock;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the dataset is unlocked.
        /// </summary>
        public event EventHandler<EventArgs> OnUnlock;

        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnUnlock"/> event.
        /// </summary>
        public void RaiseOnUnlock()
        {
            var local = OnUnlock;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }
    }
}
