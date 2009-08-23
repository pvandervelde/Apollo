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
        public void RegisterAsListener(IProcessMessages service)
        {
            throw new NotImplementedException();
        }

        public void RegisterAsSender(ISendMessages service)
        {
            throw new NotImplementedException();
        }

        public void Register(object service)
        {
            throw new NotImplementedException();
        }

        public void Unregister(object service)
        {
            throw new NotImplementedException();
        }

        public void UnregisterAsListener(IProcessMessages service)
        {
            throw new NotImplementedException();
        }

        public void UnregisterAsSender(ISendMessages service)
        {
            throw new NotImplementedException();
        }

        public MessageId Send(DnsName sender, DnsName recipient, MessageBody information)
        {
            throw new NotImplementedException();
        }

        public MessageId Send(DnsName sender, DnsName recipient, MessageBody information, MessageId inReplyTo)
        {
            throw new NotImplementedException();
        }
    }
}
