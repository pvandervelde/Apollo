//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// Defines a <see cref="IDeliveryFailureReason"/> object that indicates that
    /// the message was delivered successfully.
    /// </summary>
    [Serializable]
    public sealed class SucceededToSendMessageReason : IDeliveryFailureReason
    {
    }
}
