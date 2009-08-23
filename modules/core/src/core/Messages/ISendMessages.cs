using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Messages
{
    public interface ISendMessages
    {
        void OnMessageDeliveryFailure(object sender, MessageEventArgs e);
    }
}
