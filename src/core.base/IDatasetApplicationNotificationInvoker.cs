//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utilities;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines the interface for objects that implement <see cref="IDatasetApplicationNotifications"/>
    /// and need to provide external objects with access to their events.
    /// </summary>
    public interface IDatasetApplicationNotificationInvoker : IDatasetApplicationNotifications
    {
        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnProgress"/> event.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="currentlyProcessing">The action that is currently being processed.</param>
        void RaiseOnProgress(int progress, IProgressMark currentlyProcessing);
    }
}
