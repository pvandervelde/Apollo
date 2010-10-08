//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Apollo.Core.Properties;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// Defines methods for processing messages.
    /// </summary>
    internal sealed class MessageProcessingAssistance : IHelpMessageProcessing
    {
        /// <summary>
        /// The object used to take out locks on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection that links a message type with an action that should be taken when
        /// a specific message type is received.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The whole point of this collection is to have user provided actions.")]
        private readonly Dictionary<Type, Action<KernelMessage>> m_MessageActions = 
            new Dictionary<Type, Action<KernelMessage>>();

        /// <summary>
        /// The collection that keeps track of the outbound messages that haven't had a reply yet.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This collection holds the setter of the get-set pair that works with the IFuture return values.")]
        private readonly Dictionary<MessageId, WaitPair<MessageBody>> m_UnansweredMessages = 
            new Dictionary<MessageId, WaitPair<MessageBody>>();

        /// <summary>
        /// The collection that contains the message responses that have been answered before the message has been
        /// stored in the <see cref="m_UnansweredMessages"/> collection.
        /// </summary>
        private readonly Dictionary<MessageId, MessageBody> m_PreAnsweredMessages = 
            new Dictionary<MessageId, MessageBody>();

        /// <summary>
        /// The message pipeline which handles the message sending.
        /// </summary>
        private IMessagePipeline m_Pipeline;

        /// <summary>
        /// The name of the sender.
        /// </summary>
        private DnsName m_Name = DnsName.Nobody;

        /// <summary>
        /// The delegate used to send an error log message.
        /// </summary>
        private Action<Exception> m_SendErrorLogMessage;

        #region Implementation of IHelpMessageProcessing

        /// <summary>
        /// Defines the information necessary for the sending and receiving
        /// of messages.
        /// </summary>
        /// <param name="pipeline">The pipeline which takes care of the actual message sending.</param>
        /// <param name="sender">The <see cref="DnsName"/> of the sender.</param>
        /// <param name="errorLogSender">The function that is used to log error messages.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="pipeline"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="sender"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="sender"/> is <see cref="DnsName.Nobody"/>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="errorLogSender"/> is <see langword="null" />.
        /// </exception>
        public void DefinePipelineInformation(IMessagePipeline pipeline, DnsName sender, Action<Exception> errorLogSender)
        {
            {
                Enforce.Argument(() => pipeline);
                
                Enforce.Argument(() => sender);
                Enforce.With<ArgumentException>(!sender.Equals(DnsName.Nobody), Resources_NonTranslatable.Exceptions_Messages_SenderCannotBeNobody);

                Enforce.Argument(() => errorLogSender);
            }

            lock(m_Lock)
            {
                m_Pipeline = pipeline;
                m_Name = sender;
                m_SendErrorLogMessage = errorLogSender;
            }
        }

        /// <summary>
        /// Invalidates the information about the pipeline that is currently
        /// stored.
        /// </summary>
        public void DeletePipelineInformation()
        {
            lock (m_Lock)
            {
                m_Pipeline = null;
                m_Name = DnsName.Nobody;
            }
        }

        /// <summary>
        /// Registers the action which must be taken for a specific 
        /// message type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageAction">The message action.</param>
        public void RegisterAction(Type messageType, Action<KernelMessage> messageAction)
        {
            {
                Enforce.Argument(() => messageType);
                Enforce.With<ArgumentException>(
                    typeof(MessageBody).IsAssignableFrom(messageType), 
                    Resources_NonTranslatable.Exceptions_Messages_IncorrectType_WithTypes, 
                    typeof(MessageBody), 
                    messageType);

                Enforce.Argument(() => messageAction);
            }

            lock (m_Lock)
            {
                if (!m_MessageActions.ContainsKey(messageType))
                {
                    m_MessageActions.Add(messageType, messageAction);
                }
            }
        }

        /// <summary>
        /// Sends the message to the designated recipient.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="body">The message body.</param>
        /// <param name="originalMessage">The original message.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="recipient"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="recipient"/> is <see cref="DnsName.Nobody"/>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="body"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="originalMessage"/> is <see langword="null" />.
        /// </exception>
        public void SendMessage(DnsName recipient, MessageBody body, MessageId originalMessage)
        {
            PostMessage(recipient, body, originalMessage);
        }

        /// <summary>
        /// Sends the message to the designated recipient.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="body">The message body.</param>
        /// <param name="originalMessage">The original message.</param>
        /// <returns>
        /// An <see cref="IFuture{T}"/> object which will hold the response.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="recipient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="recipient"/> is <see cref="DnsName.Nobody"/>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="body"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="originalMessage"/> is <see langword="null"/>.
        /// </exception>
        public IFuture<MessageBody> SendMessageWithResponse(DnsName recipient, MessageBody body, MessageId originalMessage)
        {
            var id = PostMessage(recipient, body, originalMessage);
            lock (m_Lock)
            {
                // Check if the message has snuck in through the back door
                // while we weren't looking.
                if (m_PreAnsweredMessages.ContainsKey(id))
                {
                    var responseBody = m_PreAnsweredMessages[id];
                    m_PreAnsweredMessages.Remove(id);

                    var pair = new WaitPair<MessageBody>();
                    pair.Value(responseBody);

                    return new Future<MessageBody>(pair);
                }

                // The response wasn't available yet. While it
                // is expected that this is the normal case we 
                // have to check if the message has arrived first, 
                // otherwise we could do double work.
                // Also note that we still have the lock out so 
                // it is not possible that the message sneaks into
                // the back door.
                {
                    var pair = new WaitPair<MessageBody>();
                    m_UnansweredMessages.Add(id, pair);

                    return new Future<MessageBody>(pair);
                }
            }
        }

        /// <summary>
        /// Posts the message with the pipeline.
        /// </summary>
        /// <param name="recipient">The recipient of the message.</param>
        /// <param name="body">The message body.</param>
        /// <param name="originalMessage">The ID number of the message to which this message is a response.</param>
        /// <returns>
        /// The ID number of the new message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="recipient"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="recipient"/> is <see cref="DnsName.Nobody"/>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="body"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="originalMessage"/> is <see langword="null" />.
        /// </exception>
        private MessageId PostMessage(DnsName recipient, MessageBody body, MessageId originalMessage)
        {
            {
                Enforce.Argument(() => recipient);
                Enforce.With<ArgumentException>(recipient != DnsName.Nobody, Resources_NonTranslatable.Exceptions_Messages_CannotSendAMessageToNoService);

                Enforce.Argument(() => body);
                Enforce.Argument(() => originalMessage);
            }

            IMessagePipeline pipeline;
            DnsName name;
            lock (m_Lock)
            {
                pipeline = m_Pipeline;
                name = m_Name;
            }

            if (pipeline == null)
            {
                throw new MissingPipelineException();
            }

            if (name == DnsName.Nobody)
            {
                throw new UnknownDnsNameException(name);
            }

            if (name == recipient)
            {
                throw new ArgumentException(Resources_NonTranslatable.Exceptions_Messages_CannotSendAMessageBackToTheSender_WithDnsName);
            }

            return pipeline.Send(name, recipient, body, originalMessage);
        }

        /// <summary>
        /// Processes an incoming message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ReceiveMessage(KernelMessage message)
        {
            {
                Enforce.Argument(() => message);
            }

            // If the message is a response to another message, then 
            // remove the original message from the queue
            var replyTo = message.Header.InReplyTo;

            // If there is a message ID then it is a reply
            if (replyTo != MessageId.None)
            {
                ProcessMessageReply(message);

                // No need to check any further
                return;
            }

            // The message is not an answer to one of our earlier messages
            ProcessMessageByType(message);
        }

        /// <summary>
        /// Processes a message that is a reply to a message that was sender earlier.
        /// </summary>
        /// <param name="message">The reply message.</param>
        private void ProcessMessageReply(KernelMessage message)
        {
            WaitPair<MessageBody> setter = null;
            var replyTo = message.Header.InReplyTo;
            lock (m_Lock)
            {
                if (!m_UnansweredMessages.ContainsKey(replyTo))
                {
                    m_PreAnsweredMessages.Add(replyTo, message.Body);
                }
                else
                {
                    setter = m_UnansweredMessages[replyTo];
                    m_UnansweredMessages.Remove(replyTo);
                }
            }

            // The action only sets a value. The function waiting for the 
            // value is stuck on another thread so we should never
            // see the result of the value processing on the current
            // thread. However it is possible that the current thread
            // gets pre-empted just after we set the value, in which 
            // case we still have the lock. To stop that from happening
            // we first exit the lock. Because we're dealing with local
            // variables it is unlikley that we get race conditions.
            if (setter != null)
            {
                setter.Value(message.Body);
            }
        }

        /// <summary>
        /// Processes a message based on its type.
        /// </summary>
        /// <param name="message">The message.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The message sender should stay alive at all times. So we catch and log.")]
        private void ProcessMessageByType(KernelMessage message)
        {
            Action<Exception> logMessage;
            lock (m_Lock)
            {
                logMessage = m_SendErrorLogMessage;
            }

            Action<KernelMessage> action = null;
            var messageType = message.Body.GetType();
            lock (m_Lock)
            {
                if (m_MessageActions.ContainsKey(messageType))
                {
                    action = m_MessageActions[messageType];
                }
            }

            // Perform the action outside the lock so that we
            // can't deadlock easily.
            if (action != null)
            {
                try
                {
                    action(message);
                }
                catch (Exception e)
                {
                    if (logMessage != null)
                    {
                        logMessage(e);
                    }
                }
            }
        }

        #endregion
    }
}
