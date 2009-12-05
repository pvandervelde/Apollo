// -----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Properties;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// Provides message forwarding capabilities to the kernel of the Apollo application.
    /// </summary>
    [PrivateBinPathRequirements(PrivateBinPathOption.Core)]
    internal sealed class MessagePipeline : KernelService, IMessagePipeline
    {
        /// <summary>
        /// The collection of objects that have registered to receive messages.
        /// </summary>
        private readonly Dictionary<DnsName, IProcessMessages> m_Listeners = new Dictionary<DnsName, IProcessMessages>();

        /// <summary>
        /// The object used to take locks out on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection of objects that have registered to send messages.
        /// </summary>
        private readonly Dictionary<DnsName, ISendMessages> m_Senders = new Dictionary<DnsName, ISendMessages>();

        #region IMessagePipeline Members

        /// <summary>
        /// Registers as listener.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="service"/> is <see langword="null" />.
        /// </exception>
        public void RegisterAsListener(IProcessMessages service)
        {
            {
                Enforce.Argument(() => service);
            }

            lock (m_Lock)
            {
                if (m_Listeners.ContainsKey(service.Name))
                {
                    throw new DuplicateDnsNameException(service.Name);
                }

                m_Listeners.Add(service.Name, service);
            }

            Log(string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.MessagePipeline_LogMessage_ListenerAdded,
                    service.Name));
        }

        /// <summary>
        /// Registers as sender.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="service"/> is <see langword="null" />.
        /// </exception>
        public void RegisterAsSender(ISendMessages service)
        {
            {
                Enforce.Argument(() => service);
            }

            lock (m_Lock)
            {
                if (m_Senders.ContainsKey(service.Name))
                {
                    throw new DuplicateDnsNameException(service.Name);
                }

                m_Senders.Add(service.Name, service);
            }

            Log(string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.MessagePipeline_LogMessage_SenderAdded,
                    service.Name));
        }

        /// <summary>
        /// Registers the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="service"/> is <see langword="null" />.
        /// </exception>
        public void Register(object service)
        {
            {
                Enforce.Argument(() => service);
            }

            var listener = service as IProcessMessages;
            if (listener != null)
            {
                RegisterAsListener(listener);
            }

            var sender = service as ISendMessages;
            if (sender != null)
            {
                RegisterAsSender(sender);
            }
        }

        /// <summary>
        /// Unregisters as listener.
        /// </summary>
        /// <param name="service">The service.</param>
        public void UnregisterAsListener(IProcessMessages service)
        {
            {
                Enforce.Argument(() => service);
            }

            lock (m_Lock)
            {
                if (!m_Listeners.ContainsKey(service.Name))
                {
                    throw new UnknownDnsNameException(service.Name);
                }

                m_Listeners.Remove(service.Name);
            }

            Log(string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.MessagePipeline_LogMessage_ListenerRemoved,
                    service.Name));
        }

        /// <summary>
        /// Unregisters as sender.
        /// </summary>
        /// <param name="service">The service.</param>
        public void UnregisterAsSender(ISendMessages service)
        {
            {
                Enforce.Argument(() => service);
            }

            lock (m_Lock)
            {
                if (!m_Senders.ContainsKey(service.Name))
                {
                    throw new UnknownDnsNameException(service.Name);
                }

                m_Senders.Remove(service.Name);
            }

            Log(string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.MessagePipeline_LogMessage_SenderRemoved,
                    service.Name));
        }

        /// <summary>
        /// Unregisters the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="service"/> is <see langword="null" />.
        /// </exception>
        public void Unregister(object service)
        {
            {
                Enforce.Argument(() => service);
            }

            var listener = service as IProcessMessages;
            if (listener != null)
            {
                UnregisterAsListener(listener);
            }

            var sender = service as ISendMessages;
            if (sender != null)
            {
                UnregisterAsSender(sender);
            }
        }

        /// <summary>
        /// Sends the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="information">The information.</param>
        /// <returns>The ID number of the newly send message.</returns>
        public MessageId Send(DnsName sender, DnsName recipient, MessageBody information)
        {
            return Send(sender, recipient, information, MessageId.None);
        }

        /// <summary>
        /// Sends the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="information">The information.</param>
        /// <param name="inReplyTo">The in reply to.</param>
        /// <returns>The ID number of the newly send message.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The message pipeline reports errors back to the caller. That way we don't destabilise the current AppDomain.")]
        public MessageId Send(DnsName sender, DnsName recipient, MessageBody information, MessageId inReplyTo)
        {
            {
                Enforce.Argument(() => sender);
                Enforce.With<ArgumentException>(
                    !sender.Equals(DnsName.Nobody),
                    Resources.Exceptions_Messages_CannotCreateAMessageWithoutSender);
                Enforce.With<ArgumentException>(
                    !sender.Equals(DnsName.AllServices),
                    Resources.Exceptions_Messages_CannotSendAMessageFromAllServices);

                Enforce.Argument(() => recipient);
                Enforce.With<ArgumentException>(
                    !recipient.Equals(DnsName.Nobody),
                    Resources.Exceptions_Messages_CannotSendAMessageToNoService);
                Enforce.With<ArgumentException>(
                    !recipient.Equals(sender),
                    Resources.Exceptions_Messages_CannotSendAMessageBackToTheSender);

                Enforce.Argument(() => information);
                Enforce.Argument(() => inReplyTo);
            }

            // Check that the sender is known
            ISendMessages senderObj = null;
            lock (m_Lock)
            {
                // See if the sender exists. If it does then
                // grab it.
                // Do not do anything else here so that we block
                // for the least amount of time.
                if (m_Senders.ContainsKey(sender))
                {
                    senderObj = m_Senders[sender];
                }
            }

            // If we can't find the sender, then we'll have to throw an exception
            // because we can't report the error back
            // Note that this could be a security problem because nefarious code could then just
            // call the Send method with an incorrect name, and thus kill the system.
            if (senderObj == null)
            {
                throw new UnknownDnsNameException(sender);
            }

            // Find the recipients
            var recipients = new List<IProcessMessages>();
            if (recipient == DnsName.AllServices)
            {
                // If the message gets send to lots of recipients then
                // we don't care if there are 0, 1, or many.
                lock (m_Lock)
                {
                    recipients.AddRange(m_Listeners.Values);
                }
            }
            else
            {
                // See if the recipient exists. If it does then
                // grab it.
                // Do not do anything else here so that we block
                // for the least amount of time.
                if (m_Listeners.ContainsKey(recipient))
                {
                    IProcessMessages recipientObj;
                    lock (m_Lock)
                    {
                        recipientObj = m_Listeners[recipient];
                    }

                    if (recipientObj == null)
                    {
                        var failureReason = new MessageRecipientUnknownReason();
                        ReportErrorToMessageSender(senderObj, recipient, MessageId.None, information, failureReason);

                        return MessageId.None;
                    }

                    recipients.Add(recipientObj);
                }
            }

            // Get the ID number. We don't expect this to
            // fail and if it does we're in so much trouble that
            // we would want the system to die anyway.
            return SendMessage(senderObj, recipients, information, inReplyTo);
        }

        /// <summary>
        /// Sends the message to the list of recipients.
        /// </summary>
        /// <remarks>
        /// This method tries to send out the message to all recipients. If multiple
        /// recipients throw an exception then the sender will get multiple error notifications.
        /// </remarks>
        /// <param name="senderObj">The sender.</param>
        /// <param name="recipientObjs">The collection of recipients.</param>
        /// <param name="information">The information that needs to be send out.</param>
        /// <param name="inReplyTo">The ID number of the message to which this is a reply.</param>
        /// <returns>
        /// The ID number of the message that was send.
        /// </returns>
        private MessageId SendMessage(
            ISendMessages senderObj,
            IEnumerable<IProcessMessages> recipientObjs,
            MessageBody information,
            MessageId inReplyTo)
        {
            var id = MessageId.Next();
            foreach (var recipient in recipientObjs)
            {
                // Create the message. Report back if it fails.
                KernelMessage message;
                try
                {
                    message = new KernelMessage(
                        new MessageHeader(
                            id,
                            senderObj.Name,
                            recipient.Name,
                            inReplyTo),
                        information);
                }
                catch (Exception e)
                {
                    var failureReason = new CouldNotCreateMessageReason(e);
                    ReportErrorToMessageSender(senderObj, recipient.Name, id, information, failureReason);

                    // Move on to the next recipient
                    continue;
                }

                try
                {
                    recipient.ProcessMessage(message);
                }
                catch (Exception e)
                {
                    var failureReason = new FailedToSendMessageReason(e);
                    ReportErrorToMessageSender(senderObj, recipient.Name, id, information, failureReason);

                    // Move on to the next recipient
                    continue;
                }
            }

            // Even though the message transmittion failed, we can't tell how
            // far we actually got, so we have to assume that the message
            // has arrived on the other side. So we pass the ID number back
            // just in case.
            return id;
        }

        #endregion

        /// <summary>
        /// Starts the service.
        /// </summary>
        protected override void StartService()
        {
            Log(Resources.MessagePipeline_LogMessage_PipelineStarted);
        }

        /// <summary>
        /// Sends out a log message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void Log(string message)
        {
            // Create a log message
            // Send the message to the log service if it exists.
            // throw new NotImplementedException();
        }

        /// <summary>
        /// Reports the error back to message sender.
        /// </summary>
        /// <param name="sender">The sender obj.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="messageId">The message id.</param>
        /// <param name="information">The information.</param>
        /// <param name="failureReason">The failure reason.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The message pipeline reports errors back to the caller. That way we don't destabilise the current AppDomain.")]
        private void ReportErrorToMessageSender(
            ISendMessages sender,
            DnsName recipient,
            MessageId messageId,
            MessageBody information,
            IDeliveryFailureReason failureReason)
        {
            try
            {
                sender.OnMessageDeliveryFailure(messageId, recipient, information, failureReason);
            }
            catch (Exception e)
            {
                Log(string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.MessagePipeline_LogMessage_MessageFailureDeliveryFailed_WithSenderRecipientIdReasonAndException,
                        sender.Name,
                        recipient,
                        messageId,
                        failureReason,
                        e));
            }
        }
    }
}