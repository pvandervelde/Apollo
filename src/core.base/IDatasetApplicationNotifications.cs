//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;

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
        /// An event raised when the dataset is locked.
        /// </summary>
        event EventHandler<EventArgs> OnLock;

        /// <summary>
        /// An event raised when the dataset is unlocked.
        /// </summary>
        event EventHandler<EventArgs> OnUnlock;
    }
}
