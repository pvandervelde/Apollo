//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities.Commands;

namespace Apollo.Core.Host.UserInterfaces
{
    /// <summary>
    /// Defines the interface for the User Interface <see cref="KernelService"/>.
    /// </summary>
    public interface IUserInterfaceService : IInvokeCommands
    {
        /// <summary>
        /// Registers the notification.
        /// </summary>
        /// <param name="name">The name of the notification.</param>
        /// <param name="callback">The callback method that is called when the notification is activated.</param>
        void RegisterNotification(NotificationName name, Action<INotificationArguments> callback);
    }
}
