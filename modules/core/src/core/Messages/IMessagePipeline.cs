//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// Defines the interface for objects that act as the center of the 
    /// message sending system.
    /// </summary>
    public interface IMessagePipeline
    {
        /// <summary>
        /// Registers as listener.
        /// </summary>
        /// <param name="service">The service.</param>
        void RegisterAsListener(IProcessMessages service);

        /// <summary>
        /// Registers as sender.
        /// </summary>
        /// <param name="service">The service.</param>
        void RegisterAsSender(ISendMessages service);

        /// <summary>
        /// Registers the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        void Register(object service);

        /// <summary>
        /// Unregisters the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        void Unregister(object service);

        /// <summary>
        /// Unregisters as listener.
        /// </summary>
        /// <param name="service">The service.</param>
        void UnregisterAsListener(IProcessMessages service);

        /// <summary>
        /// Unregisters as sender.
        /// </summary>
        /// <param name="service">The service.</param>
        void UnregisterAsSender(ISendMessages service);

        /// <summary>
        /// Sends the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="information">The information.</param>
        /// <returns>The ID number of the newly send message.</returns>
        MessageId Send(DnsName sender, DnsName recipient, MessageBody information);

        /// <summary>
        /// Sends the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="information">The information.</param>
        /// <param name="inReplyTo">The in reply to.</param>
        /// <returns>The ID number of the newly send message.</returns>
        MessageId Send(DnsName sender, DnsName recipient, MessageBody information, MessageId inReplyTo);
    }
}
