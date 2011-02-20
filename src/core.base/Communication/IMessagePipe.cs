//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for objects that pipe messages from one object to another object.
    /// </summary>
    internal interface IMessagePipe : IReceivingEndpoint
    {
        /// <summary>
        /// An event raised when a new message is available in the pipe.
        /// </summary>
        event EventHandler<MessageEventArgs> OnNewMessage;
    }
}
