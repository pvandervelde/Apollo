//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Utilities;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// Defines the interface for objects that implement <see cref="IDatasetApplicationNotifications"/>
    /// and need to provide external objects with access to their events.
    /// </summary>
    internal interface IDatasetApplicationNotificationInvoker : IDatasetApplicationNotifications
    {
        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnProgress"/> event.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="currentlyProcessing">The action that is currently being processed.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        void RaiseOnProgress(int progress, IProgressMark currentlyProcessing);

        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnSwitchToEditingMode"/> event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        void RaiseOnSwitchToEditingMode();

        /// <summary>
        /// Raises the <see cref="IDatasetApplicationNotifications.OnSwitchToExecutingMode"/> event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        void RaiseOnSwitchToExecutingMode();
    }
}
