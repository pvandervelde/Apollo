//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the base class for <see cref="KernelService"/> objects that need to send and receive messages
    /// through an <see cref="IMessagePipeline"/> object.
    /// </summary>
    public abstract class MessageEnabledKernelService : KernelService, ISendMessages, IProcessMessages
    {
        /// <summary>
        /// The object that takes care of the message processing.
        /// </summary>
        private readonly IHelpMessageProcessing m_Processor;
        
        /// <summary>
        /// The service that handles message receiving and processing.
        /// </summary>
        private IMessagePipeline m_MessageSink;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnabledKernelService"/> class.
        /// </summary>
        /// <param name="processor">The object that handles the incoming messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="processor"/> is <see langword="null" />.
        /// </exception>
        protected MessageEnabledKernelService(IHelpMessageProcessing processor)
        {
            {
                Enforce.Argument(() => processor);
            }

            m_Processor = processor;
        }

        /// <summary>
        /// Connects to message sink.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        protected void ConnectToMessageSink(IMessagePipeline pipeline)
        {
            Debug.Assert(pipeline != null, "There should be a pipeline object.");

            if (ReferenceEquals(m_MessageSink, pipeline))
            {
                return;
            }

            // If the currently stored pipeline is not null, then we
            // need to unregister.
            if (!ReferenceEquals(m_MessageSink, null))
            {
                UnregisterFromMessagePipeline();
                m_Processor.DeletePipelineInformation();
                m_MessageSink = null;
            }

            // Register the new pipeline
            m_Processor.DefinePipelineInformation(pipeline, Name);

            m_MessageSink = pipeline;
            RegisterWithMessagePipeline();
        }

        /// <summary>
        /// Disconnects from message sink.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        protected void DisconnectFromMessageSink(IMessagePipeline pipeline)
        {
            // If the currently stored pipeline is not null, then we
            // need to unregister.
            if (ReferenceEquals(m_MessageSink, pipeline))
            {
                UnregisterFromMessagePipeline();
                m_Processor.DeletePipelineInformation();
                m_MessageSink = null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected to pipeline.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this instance is connected to pipeline; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected bool IsConnectedToPipeline
        {
            get
            {
                return m_MessageSink != null;
            }
        }

        /// <summary>
        /// Sends the message to the designated recipient.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="body">The message body.</param>
        /// <param name="originalMessage">The original message.</param>
        protected void SendMessage(DnsName recipient, MessageBody body, MessageId originalMessage)
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
        protected IFuture<MessageBody> SendMessageWithResponse(DnsName recipient, MessageBody body, MessageId originalMessage)
        {
            Debug.Assert(recipient != DnsName.Nobody, "Cannot send messages to Nobody.");
            Debug.Assert(body != null, "There should be a message in order to send one.");
            Debug.Assert(originalMessage != null, "The ID number of the original message must either be specified or None.");

            return m_Processor.SendMessageWithResponse(recipient, body, originalMessage);
        }

        #region Implementation of KernelService

        /// <summary>
        /// Starts the service.
        /// </summary>
        protected sealed override void StartService()
        {
            if (m_MessageSink == null)
            {
                throw new MissingServiceDependencyException();
            }

            // Set up the message actions
            StoreMessageActions(m_Processor);

            PreMessageInitializeStartup();
            RegisterWithMessagePipeline();
            PostMessageInitializeStartup();
        }

        /// <summary>
        /// Stores the different message types and their connected actions.
        /// </summary>
        /// <param name="processor">The processor.</param>
        protected abstract void StoreMessageActions(IHelpMessageProcessing processor);

        /// <summary>
        /// Performs initialization prior to setting up the message handling.
        /// </summary>
        protected virtual void PreMessageInitializeStartup()
        {
            // In the base method we don't do anything.
        }

        /// <summary>
        /// Registers the current service the with message pipeline.
        /// </summary>
        private void RegisterWithMessagePipeline()
        {
            if (!m_MessageSink.IsRegistered(Name))
            {
                // always register as listener last so that the
                // message flow starts last.
                m_MessageSink.RegisterAsSender(this);
                m_MessageSink.RegisterAsListener(this);
            }
        }

        /// <summary>
        /// Performs initialization after setting up the message handling.
        /// </summary>
        protected virtual void PostMessageInitializeStartup()
        {
            // In the base method, we don't do anything.
        }

        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform shutdown tasks.
        /// </summary>
        protected sealed override void StopService()
        {
            PreMessageUnregisterStopAction();
            UnregisterFromMessagePipeline();
            PostMessageUnregisterStopAction();
        }

        /// <summary>
        /// Performs un-initialization prior to unregistering from the message handling.
        /// </summary>
        protected virtual void PreMessageUnregisterStopAction()
        {
            // In the base method, we don't do anything.
        }

        /// <summary>
        /// Unregisters the current instance as sender and listener from message pipeline.
        /// </summary>
        private void UnregisterFromMessagePipeline()
        {
            if (m_MessageSink != null && m_MessageSink.IsRegistered(Name))
            {
                // Always unregister as the listener first so that the 
                // message flow stops.
                m_MessageSink.UnregisterAsListener(this);
                m_MessageSink.UnregisterAsSender(this);
            }
        }

        /// <summary>
        /// Performs un-initialization after unregistering from the message handling.
        /// </summary>
        protected virtual void PostMessageUnregisterStopAction()
        {
            // In the base method, we don't do anything.
        }

        /// <summary>
        /// Gets a value indicating whether this service is capable of performing all its functions. This
        /// is the case if the service is not starting or stopping.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this service is capable of performing all its functions; otherwise, <see langword="false"/>.
        /// </value>
        protected override bool IsFullyFunctional
        {
            get
            {
                return base.IsFullyFunctional && IsConnectedToPipeline;
            }
        }

        #endregion

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

        #region IDnsNameObject

        /// <summary>
        /// Gets or sets the identifier of the object.
        /// </summary>
        /// <value>The identifier.</value>
        public DnsName Name
        {
            get;
            protected set;
        }

        #endregion
    }
}
