//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Nuclei.Progress;

namespace Apollo.Core.Dataset
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
        /// An event raised when the dataset is switched to edit mode.
        /// </summary>
        public event EventHandler<EventArgs> OnSwitchToEditingMode;

        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnSwitchToEditingMode"/> event.
        /// </summary>
        public void RaiseOnSwitchToEditingMode()
        {
            var local = OnSwitchToEditingMode;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the dataset is switched to executing mode.
        /// </summary>
        public event EventHandler<EventArgs> OnSwitchToExecutingMode;

        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnSwitchToExecutingMode"/> event.
        /// </summary>
        public void RaiseOnSwitchToExecutingMode()
        {
            var local = OnSwitchToExecutingMode;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the dataset history is rolled back or rolled forward.
        /// </summary>
        public event EventHandler<EventArgs> OnTimelineUpdate;

        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnTimelineUpdate"/> event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        public void RaiseOnTimelineUpdate()
        {
            var local = OnTimelineUpdate;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }
    }
}
