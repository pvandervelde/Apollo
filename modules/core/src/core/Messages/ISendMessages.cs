using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// The interface for objects which use the <see cref="MessagePipeline"/> to send messages
    /// to the services running in the kernel.
    /// </summary>
    public interface ISendMessages
    {
        /// <summary>
        /// Called when a message could not be delivered.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Apollo.Core.Messages.MessageEventArgs"/> instance containing the event data.</param>
        void OnMessageDeliveryFailure(object sender, MessageEventArgs e);
    }
}
