// -----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using Apollo.Core.Logging;
using Apollo.Core.Properties;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// Provides message forwarding capabilities to the kernel of the Apollo application.
    /// </summary>
    [PrivateBinPathRequirements(PrivateBinPathOption.Core)]
    internal sealed class MessagePipeline : KernelService, IMessagePipeline
    {
        /// <summary>
        /// The object used to take locks out on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection of objects that have registered to receive messages.
        /// </summary>
        private readonly Dictionary<DnsName, IProcessMessages> m_Listeners = new Dictionary<DnsName, IProcessMessages>();

        /// <summary>
        /// The collection of objects that have registered to send messages.
        /// </summary>
        private readonly Dictionary<DnsName, ISendMessages> m_Senders = new Dictionary<DnsName, ISendMessages>();

        /// <summary>
        /// The collection of DnsNames.
        /// </summary>
        private readonly IDnsNameConstants m_DnsNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePipeline"/> class.
        /// </summary>
        /// <param name="dnsNames">The object that stores all the <see cref="DnsName"/> objects for the application.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dnsNames"/> is <see langword="null"/>.
        /// </exception>
        public MessagePipeline(IDnsNameConstants dnsNames)
        {
            {
                Enforce.Argument(() => dnsNames);
            }

            Name = dnsNames.AddressOfMessagePipeline;
            m_DnsNames = dnsNames;
        }

        /// <summary>
        /// Gets or sets the identifier of the object.
        /// </summary>
        /// <value>The identifier.</value>
        private DnsName Name
        {
            get;
            set;
        }

        #region IMessagePipeline Members

        /// <summary>
        /// Determines whether a service with the specified name is registered.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>
        ///     <see langword="true"/> if a service with the specified name is registered; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsRegistered(DnsName name)
        {
            lock (m_Lock)
            {
                return name != null && (m_Senders.ContainsKey(name) || m_Listeners.ContainsKey(name) || name.Equals(Name));
            }
        }

        /// <summary>
        /// Determines whether the specified service is registered.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified service is registered; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsRegistered(IProcessMessages service)
        {
            lock (m_Lock)
            {
                return service != null && m_Listeners.ContainsKey(service.Name);
            }
        }

        /// <summary>
        /// Determines whether the specified service is registered.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified service is registered; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsRegistered(ISendMessages service)
        {
            lock (m_Lock)
            {
                return service != null && (m_Senders.ContainsKey(service.Name) || service.Name.Equals(Name));
            }
        }

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

            // It is not possible to register a service with the same name as the pipeline.
            if (service.Name.Equals(Name))
            {
                throw new DuplicateDnsNameException(service.Name);
            }

            lock (m_Lock)
            {
                if (m_Listeners.ContainsKey(service.Name))
                {
                    throw new DuplicateDnsNameException(service.Name);
                }

                m_Listeners.Add(service.Name, service);
            }

            LogInfo(string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_NonTranslatable.MessagePipeline_LogMessage_ListenerAdded,
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

            // It is not possible to register a service with the same name as the pipeline.
            if (service.Name.Equals(Name))
            {
                throw new DuplicateDnsNameException(service.Name);
            }

            lock (m_Lock)
            {
                if (m_Senders.ContainsKey(service.Name))
                {
                    throw new DuplicateDnsNameException(service.Name);
                }

                m_Senders.Add(service.Name, service);
            }

            LogInfo(string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_NonTranslatable.MessagePipeline_LogMessage_SenderAdded,
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

            LogInfo(string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_NonTranslatable.MessagePipeline_LogMessage_ListenerRemoved,
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

            LogInfo(string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_NonTranslatable.MessagePipeline_LogMessage_SenderRemoved,
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
        /// <remarks>
        /// All messages are send in parallel.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when the service is not fully functional.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="sender"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="sender"/> is <see cref="DnsName.Nobody"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="recipient"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="recipient"/> is <see cref="DnsName.Nobody"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="recipient"/> is equal to <paramref name="sender"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="information"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownDnsNameException">
        /// Thrown if <paramref name="sender"/> cannot be found in the collection of known senders.
        /// </exception>
        /// <exception cref="UnknownDnsNameException">
        /// Thrown if <paramref name="recipient"/> cannot be found in the collection of known listeners.
        /// </exception>
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
        /// <remarks>
        /// All messages are send in parallel.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when the service is not fully functional.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="sender"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="sender"/> is <see cref="DnsName.Nobody"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="recipient"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="recipient"/> is <see cref="DnsName.Nobody"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="recipient"/> is equal to <paramref name="sender"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="information"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="inReplyTo"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownDnsNameException">
        /// Thrown if <paramref name="sender"/> cannot be found in the collection of known senders.
        /// </exception>
        /// <exception cref="UnknownDnsNameException">
        /// Thrown if <paramref name="recipient"/> cannot be found in the collection of known listeners.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The message pipeline reports errors back to the caller. That way we don't destabilise the current AppDomain.")]
        [SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods",
            Justification = "The SendMessage method is under our control and only transmits data to the other side of the pipeline. It doesn't invoke external code.")]
        public MessageId Send(DnsName sender, DnsName recipient, MessageBody information, MessageId inReplyTo)
        {
            {
                Enforce.With<ArgumentException>(IsFullyFunctional, Resources_NonTranslatable.Exception_Messages_ServicesIsNotFullyFunctional, StartupState);

                Enforce.Argument(() => sender);
                Enforce.With<ArgumentException>(
                    !sender.Equals(DnsName.Nobody),
                    Resources_NonTranslatable.Exception_Messages_CannotCreateAMessageWithoutSender);

                Enforce.Argument(() => recipient);
                Enforce.With<ArgumentException>(
                    !recipient.Equals(DnsName.Nobody),
                    Resources_NonTranslatable.Exception_Messages_CannotSendAMessageToNoService);
                Enforce.With<ArgumentException>(
                    !recipient.Equals(sender),
                    Resources_NonTranslatable.Exception_Messages_CannotSendAMessageBackToTheSender_WithDnsName,
                    sender);

                Enforce.Argument(() => information);
                Enforce.Argument(() => inReplyTo);
            }

            // Check that the sender is known
            lock (m_Lock)
            {
                // If we can't find the sender, then we'll have to throw an exception
                // because we can't report the error back
                // Note that this could be a security problem because nefarious code could then just
                // call the Send method with an incorrect name, and thus kill the system.
                // Do not do anything else here so that we block
                // for the least amount of time.
                if ((!m_Senders.ContainsKey(sender)) && (!sender.Equals(Name)))
                {
                    throw new UnknownDnsNameException(sender);
                }
            }

            // Find the recipients
            IProcessMessages recipientObj = null;

            // See if the recipient exists. If it does then
            // grab it.
            // Do not do anything else here so that we block
            // for the least amount of time.
            if (m_Listeners.ContainsKey(recipient))
            {
                lock (m_Lock)
                {
                    recipientObj = m_Listeners[recipient];
                }

                if (recipientObj == null)
                {
                    throw new UnknownDnsNameException(recipient);
                }
            }

            var id = MessageId.Next();
            Parallel.Invoke(() => SendMessage(sender, recipientObj, information, id, inReplyTo));

            return id;
        }

        /// <summary>
        /// Sends the message to the list of recipients.
        /// </summary>
        /// <param name="sender">The <see cref="DnsName"/> of the sender.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="information">The information that needs to be send out.</param>
        /// <param name="id">The id of the message that is about to be send.</param>
        /// <param name="inReplyTo">The ID number of the message to which this is a reply.</param>
        /// <remarks>
        /// This method tries to send out the message to all recipients. If multiple
        /// recipients throw an exception then the sender will get multiple error notifications.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The message pipeline should stay alive at all times. So we catch and log.")]
        private void SendMessage(
            DnsName sender,
            IProcessMessages recipient,
            MessageBody information,
            MessageId id,
            MessageId inReplyTo)
        {
            // Create the message and send it. This should never fail
            // because the calling code should have checked everything,
            // however we don't completely trust that so ...
            {
                Debug.Assert(sender != null, "Need a sender DNS name.");
                Debug.Assert(!sender.Equals(DnsName.Nobody), "The sender DNS cannot be DnsName.Nobody.");

                Debug.Assert(recipient != null, "Need a recipient DNS name.");
                Debug.Assert(!recipient.Equals(DnsName.Nobody), "The recipient DNS cannot be DnsName.Nobody.");
                Debug.Assert(!recipient.Equals(sender), "It makes no sense trying to send a message from the sender to the sender.");

                Debug.Assert(information != null, "There must be some information to pass on.");
                Debug.Assert(inReplyTo != null, "The response ID shouldn't be null. Try MessageId.None instead.");
            }

            var message = new KernelMessage(
                new MessageHeader(
                    id,
                    sender,
                    recipient.Name,
                    inReplyTo),
                information);

            try
            {
                recipient.ProcessMessage(message);
            }
            catch (Exception e)
            {
                LogInfo(string.Format(
                        CultureInfo.InvariantCulture,
                        Resources_NonTranslatable.MessagePipeline_LogMessage_MessageDeliveryFailed_WithSenderRecipientIdAndException,
                        sender,
                        recipient,
                        id,
                        e));

                // @Todo: we should really send back a message fail now ..
            }
        }

        #endregion

        /// <summary>
        /// Starts the service.
        /// </summary>
        protected override void StartService()
        {
            LogInfo(Resources_NonTranslatable.MessagePipeline_LogMessage_PipelineStarted);
        }

        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform shutdown tasks.
        /// </summary>
        protected override void StopService()
        {
            LogInfo(Resources_NonTranslatable.MessagePipeline_LogMessage_PipelineStopped);
        }

        /// <summary>
        /// Sends out a log message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LogInfo(string message)
        {
            if (IsFullyFunctional && m_Listeners.ContainsKey(m_DnsNames.AddressOfLogger))
            {
                Send(
                    Name,
                    m_DnsNames.AddressOfLogger,
                    new LogEntryRequestMessage(
                        new LogMessage(
                            Name.ToString(),
                            LevelToLog.Info,
                            message),
                        LogType.Debug));
            }
        }
    }
}
