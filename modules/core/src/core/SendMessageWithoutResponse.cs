﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Messaging;

namespace Apollo.Core
{
    /// <summary>
    /// A delegate used to to send messages for which a response is expected.
    /// </summary>
    /// <param name="recipient">The <see cref="DnsName"/> of the service to which the message should be send.</param>
    /// <param name="message">The message that should be passed on.</param>
    /// <param name="inResponseTo">The <see cref="MessageId"/> of the message to which this message is a response.</param>
    public delegate void SendMessageWithoutResponse(DnsName recipient, MessageBody message, MessageId inResponseTo);
}