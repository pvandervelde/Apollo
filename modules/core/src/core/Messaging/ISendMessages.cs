//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// The interface for objects which use the <see cref="MessagePipeline"/> to send messages
    /// to the services running in the kernel.
    /// </summary>
    public interface ISendMessages : IDnsNameObject
    {
    }
}
