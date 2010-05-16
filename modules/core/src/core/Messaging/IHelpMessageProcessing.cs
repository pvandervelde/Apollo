//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// Defines the interface for object that assist in the message
    /// sending and receiving process.
    /// </summary>
    internal interface IHelpMessageProcessing
    {
        /// <summary>
        /// Defines the information necessary for the sending and receiving
        /// of messages.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline which takes care of the actual message sending.
        /// </param>
        /// <param name="sender">The <see cref="DnsName"/> of the sender.</param>
        /// <param name="errorLogSender">The function that is used to log error messages.</param>
        void DefinePipelineInformation(IMessagePipeline pipeline, DnsName sender, Action<Exception> errorLogSender);

        /// <summary>
        /// Invalidates the information about the pipeline that is currently
        /// stored.
        /// </summary>
        void DeletePipelineInformation();

        /// <summary>
        /// Registers the action which must be taken for a specific 
        /// message type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageAction">The message action.</param>
        void RegisterAction(Type messageType, Action<KernelMessage> messageAction);

        /// <summary>
        /// Sends the message to the designated recipient.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="body">The message body.</param>
        /// <param name="originalMessage">The original message.</param>
        void SendMessage(DnsName recipient, MessageBody body, MessageId originalMessage);

        /// <summary>
        /// Sends the message to the designated recipient and returns a <see cref="IFuture{T}"/>
        /// object which will hold the response.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="body">The message body.</param>
        /// <param name="originalMessage">The original message.</param>
        /// <returns>
        /// An <see cref="IFuture{T}"/> object which will hold the response.
        /// </returns>
        IFuture<MessageBody> SendMessageWithResponse(DnsName recipient, MessageBody body, MessageId originalMessage);

        /// <summary>
        /// Processes an incoming message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ReceiveMessage(KernelMessage message);
    }
}
