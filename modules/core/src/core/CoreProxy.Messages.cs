//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Messaging;
using Apollo.Utils;

namespace Apollo.Core
{
    /// <content>
    /// Defines the methods necessary for the handling of messages.
    /// </content>
    internal sealed partial class CoreProxy : ISendMessages, IProcessMessages
    {
        /// <summary>
        /// Determines whether the specified services can be shutdown.
        /// </summary>
        /// <param name="services">The collection of services for which the shutdown capability must be requested.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified services can be shutdown; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanShutdownService(IEnumerable<KernelService> services)
        {
            // Send the messages. We want to do this actively (not lazily)
            // so we use a good old foreach loop.
            var responses = new List<IFuture<MessageBody>>();
            foreach (var kernelService in services)
            {
                var nameObject = kernelService as IDnsNameObject;
                if (nameObject == null)
                {
                    // Move on. We assume that services that don't
                    // listen for messages can just be terminated.
                    continue;
                }

                var future = SendMessageWithResponse(nameObject.Name, new ShutdownCapabilityRequestMessage(), MessageId.None);
                responses.Add(future);
            }

            // Technically we could do an even better job here. Currently we just iterate over the collection
            // and wait when we need to. We could iterate over the collection and check for each future
            // if the results are in already. If so we use them, if not then we move on and come back
            // later.
            return responses.Aggregate(
                true, 
                (current, future) =>
                    {
                        var body = future.Result() as ShutdownCapabilityResponseMessage;
                        return body != null ? current && body.CanShutdown : current;
                    });
        }

        /// <summary>
        /// Sends the message to the designated recipient.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="body">The message body.</param>
        /// <param name="originalMessage">The original message.</param>
        private void SendMessage(DnsName recipient, MessageBody body, MessageId originalMessage)
        {
            Debug.Assert(recipient != DnsName.Nobody, "Cannot send messages to Nobody.");
            Debug.Assert(body != null, "There should be a message in order to send one.");
            Debug.Assert(originalMessage != null, "The ID number of the original message must either be specified or None.");

            m_Processor.SendMessage(recipient, body, originalMessage);
        }

        /// <summary>
        /// Sends the message to the designated recipient and returns a wait object which
        /// allows waiting for the response.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="body">The message body.</param>
        /// <param name="originalMessage">The original message.</param>
        /// <returns>
        /// An <see cref="IFuture{T}"/> object which will eventually hold the message response.
        /// </returns>
        private IFuture<MessageBody> SendMessageWithResponse(DnsName recipient, MessageBody body, MessageId originalMessage)
        {
            Debug.Assert(recipient != DnsName.Nobody, "Cannot send messages to Nobody.");
            Debug.Assert(body != null, "There should be a message in order to send one.");
            Debug.Assert(originalMessage != null, "The ID number of the original message must either be specified or None.");

            return m_Processor.SendMessageWithResponse(recipient, body, originalMessage);
        }

        /// <summary>
        /// Fills the collection of message actions.
        /// </summary>
        private void SetupMessageActions()
        {
            // Define the response to a shutdown request
            m_Processor.RegisterAction(
                typeof(ShutdownRequestMessage),
                message =>
                    {
                        var request = message.Body as ShutdownRequestMessage;
                        Debug.Assert(request != null, "Message type mapping failed for ShutdownRequestMessage");

                        HandleShutdownRequest(message.Header.Recipient, message.Header.Id, request);
                    });

            // Restart request. Restarts one of the services
            // Create new AppDomain request
            // Request for system information --> send an entire block in one go
        }

        private void HandleShutdownRequest(DnsName recipient, MessageId id, ShutdownRequestMessage request)
        {
            bool canShutdown = request.IsShutdownForced ? true : m_Owner.CanShutdown();
            if (request.IsResponseRequired)
            {
                SendMessage(recipient, new ShutdownRequestResponseMessage(canShutdown), id);
            }

            if (canShutdown)
            {
                m_Owner.Shutdown();
            }
        }

        #region Implementation of IProcessMessages

        /// <summary>
        /// Processes a single message that is directed at the current service.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        public void ProcessMessage(KernelMessage message)
        {
            m_Processor.ReceiveMessage(message);
        }

        /// <summary>
        /// Processes a set of messages which are directed at the current service.
        /// </summary>
        /// <param name="messages">The set of messages which should be processed.</param>
        public void ProcessMessages(IEnumerable<KernelMessage> messages)
        {
            foreach (var message in messages)
            {
                ProcessMessage(message);
            }
        }

        #endregion
    }
}
