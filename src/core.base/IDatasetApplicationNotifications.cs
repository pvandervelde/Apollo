//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities;
using Nuclei.Communication;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Provides notifications from a dataset application.
    /// </summary>
    [InternalNotification]
    public interface IDatasetApplicationNotifications : INotificationSet
    {
        /// <summary>
        /// An event raised when progress is made on some action.
        /// </summary>
        event EventHandler<ProgressEventArgs> OnProgress;

        /// <summary>
        /// An event raised when the dataset is switched to edit mode.
        /// </summary>
        event EventHandler<EventArgs> OnSwitchToEditingMode;

        /// <summary>
        /// An event raised when the dataset is switched to executing mode.
        /// </summary>
        event EventHandler<EventArgs> OnSwitchToExecutingMode;

        /// <summary>
        /// An event raised when the dataset history is rolled back or rolled forward.
        /// </summary>
        event EventHandler<EventArgs> OnTimelineUpdate;
    }
}
