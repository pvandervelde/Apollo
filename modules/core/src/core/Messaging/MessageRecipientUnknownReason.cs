//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Messages
{
    /// <summary>
    /// Defines a <see cref="IDeliveryFailureReason"/> for cases where the message delivery
    /// failed due to an unknown recipient.
    /// </summary>
    public sealed class MessageRecipientUnknownReason : IDeliveryFailureReason
    {
    }
}
