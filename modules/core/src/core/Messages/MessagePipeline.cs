using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// Provides message forwarding capabilities to the kernel of the Apollo application.
    /// </summary>
    public sealed class MessagePipeline : KernelService
    {
        /// <summary>
        /// Registers as listener.
        /// </summary>
        /// <param name="service">The service.</param>
        public void RegisterAsListener(IProcessMessages service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers as sender.
        /// </summary>
        /// <param name="service">The service.</param>
        public void RegisterAsSender(ISendMessages service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        public void Register(object service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unregisters the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        public void Unregister(object service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unregisters as listener.
        /// </summary>
        /// <param name="service">The service.</param>
        public void UnregisterAsListener(IProcessMessages service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unregisters as sender.
        /// </summary>
        /// <param name="service">The service.</param>
        public void UnregisterAsSender(ISendMessages service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="information">The information.</param>
        /// <returns></returns>
        public MessageId Send(DnsName sender, DnsName recipient, MessageBody information)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="information">The information.</param>
        /// <param name="inReplyTo">The in reply to.</param>
        /// <returns></returns>
        public MessageId Send(DnsName sender, DnsName recipient, MessageBody information, MessageId inReplyTo)
        {
            throw new NotImplementedException();
        }
    }
}
