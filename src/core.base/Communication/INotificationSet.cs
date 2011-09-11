//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the base for classes that implement a set of notifications 
    /// that can be watched remotely through a <see cref="RemoteNotificationHub"/>.
    /// </summary>
    /// <design>
    /// The <see cref="RemoteNotificationHub"/> will generate a proxy object for all the notification sets
    /// available on a given endpoint.
    /// </design>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "This interface is used as marker interface for sets of notifications.")]
    public interface INotificationSet
    {
    }
}
