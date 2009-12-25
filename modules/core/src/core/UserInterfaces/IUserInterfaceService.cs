//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Messaging;

namespace Apollo.Core.UserInterfaces
{
    /// <summary>
    /// Defines the interface for the User Interface <see cref="KernelService"/>.
    /// </summary>
    public interface IUserInterfaceService
    {
        // Send message --> translate from UI level to kernel level
        // void Notify(object address, Action<object> callback);
    }
}
