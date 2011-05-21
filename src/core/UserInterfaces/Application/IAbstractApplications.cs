// -----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.UserInterfaces.Application
{
    /// <summary>
    /// Defines the interface for objects that provide an interface abstraction to the
    /// application.
    /// </summary>
    public interface IAbstractApplications
    {
        /// <summary>
        /// Shuts the application down.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Gets the object that provides information about the application status.
        /// </summary>
        /// <value>The object that provides information about the application status.</value>
        IHoldSystemInformation ApplicationStatus
        {
            get;
        }

        /// <summary>
        /// Registers the notification.
        /// </summary>
        /// <param name="name">The name of the notification.</param>
        /// <param name="callback">The callback method that is called when the notification is activated.</param>
        void RegisterNotification(NotificationName name, Action<INotificationArguments> callback);
    }
}
