//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Properties;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Handles incoming messages and dispatches them to the correct processors.
    /// </summary>
    internal sealed class MessageHandler : IDirectIncomingMessages, IProcessIncomingMessages
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection of filters that should be used more than once.
        /// </summary>
        /// <design>
        /// Could improve this if we store filters based on the type of messag they work on. All filters that work
        /// on the same type get placed in a collection (with their connected actions). That way the look-up will
        /// be pretty quick. On the other hand we'll be storing a collection for each message type we filter on
        /// which may mean we're storing an entire Collection object for a single filter.
        /// </design>
        private readonly Dictionary<IMessageFilter, IMessageProcessAction> m_MultiUseFilters
            = new Dictionary<IMessageFilter, IMessageProcessAction>();

        /// <summary>
        /// The collection that maps the ID numbers of the messages that are waiting for a response
        /// message to the endpoint.
        /// </summary>
        private readonly Dictionary<MessageId, Tuple<EndpointId, TaskCompletionSource<ICommunicationMessage>>> m_TasksWaitingForResponse
            = new Dictionary<MessageId, Tuple<EndpointId, TaskCompletionSource<ICommunicationMessage>>>();

        /// <summary>
        /// On arrival of a message which responds to the message with the
        /// <paramref name="inResponseTo"/> ID number the caller will be
        /// able to get the message through the <see cref="Task{T}"/> object.
        /// </summary>
        /// <param name="messageReceiver">The ID of the endpoint to which the original message was send.</param>
        /// <param name="inResponseTo">The ID number of the message for which a response is expected.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> implementation which returns the response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageReceiver"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="messageReceiver"/> is equal to <see cref="MessageId.None"/>.
        /// </exception>
        public Task<ICommunicationMessage> ForwardResponse(EndpointId messageReceiver, MessageId inResponseTo)
        {
            {
                Enforce.Argument(() => messageReceiver);
                Enforce.Argument(() => inResponseTo);
                Enforce.With<ArgumentException>(!inResponseTo.Equals(MessageId.None), Resources.Exceptions_Messages_AMessageNeedsToHaveAnId);
            }

            lock (m_Lock)
            {
                if (!m_TasksWaitingForResponse.ContainsKey(inResponseTo))
                {
                    var source = new TaskCompletionSource<ICommunicationMessage>(TaskCreationOptions.LongRunning);
                    m_TasksWaitingForResponse.Add(inResponseTo, Tuple.Create(messageReceiver, source));
                }

                return m_TasksWaitingForResponse[inResponseTo].Item2.Task;
            }
        }

        /// <summary>
        /// On arrival of a message which passes the given filter the caller
        /// will be notified though the given delegate.
        /// </summary>
        /// <param name="messageFilter">The message filter.</param>
        /// <param name="notifyAction">The action invoked when a matching message arrives.</param>
        public void ActOnArrival(IMessageFilter messageFilter, IMessageProcessAction notifyAction)
        {
            {
                Enforce.Argument(() => messageFilter);
                Enforce.Argument(() => notifyAction);
            }

            lock (m_Lock)
            {
                if (!m_MultiUseFilters.ContainsKey(messageFilter))
                {
                    m_MultiUseFilters.Add(messageFilter, notifyAction);
                }
            }
        }

        /// <summary>
        /// Processes the message and invokes the desired functions based on the 
        /// message contents or type.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        public void ProcessMessage(ICommunicationMessage message)
        {
            {
                Enforce.Argument(() => message);
            }

            // First check that the message isn't a response
            if (!message.InResponseTo.Equals(MessageId.None))
            {
                TaskCompletionSource<ICommunicationMessage> source = null;
                lock (m_Lock)
                {
                    if (m_TasksWaitingForResponse.ContainsKey(message.InResponseTo))
                    {
                        source = m_TasksWaitingForResponse[message.InResponseTo].Item2;
                        m_TasksWaitingForResponse.Remove(message.InResponseTo);
                    }
                }

                // Invoke the SetResult outside the lock because the setting of the 
                // result may lead to other messages being send and more responses 
                // being required to be handled. All of that may need access to the lock.
                if (source != null)
                {
                    source.SetResult(message);
                }

                return;
            }

            // The message isn't a response so go to the filters
            // First copy the filters and their associated actions so that we can
            // invoke the actions outside the lock. This is necessary because
            // a message might invoke the requirement for more messages and eventual
            // responses. Setting up a response requires that we can take out the lock
            // again.
            // Once we have the filters then we can just run past all of them and invoke
            // the actions based on the filter pass through.
            Dictionary<IMessageFilter, IMessageProcessAction> localCollection = null;
            lock (m_Lock)
            {
                localCollection = new Dictionary<IMessageFilter, IMessageProcessAction>(m_MultiUseFilters);
            }

            foreach (var pair in localCollection)
            {
                if (pair.Key.PassThrough(message))
                {
                    pair.Value.Invoke(message);
                }
            }

            // Only now do we check if anything should be cleaned up. By waiting till here we
            // give the system a chance to process the message the normal way first, that means
            // that the rest of the system can be notified.
            if (IsMessageIndicatingEndpointDisconnect(message))
            {
                TerminateWaitingResponsesForEndpoint(message.OriginatingEndpoint);
            }
        }

        private bool IsMessageIndicatingEndpointDisconnect(ICommunicationMessage message)
        {
            return message.GetType().Equals(typeof(EndpointDisconnectMessage));
        }

        private void TerminateWaitingResponsesForEndpoint(EndpointId endpointId)
        {
            lock (m_Lock)
            {
                var messagesThatWillNotBeAnswered = new List<MessageId>();
                foreach (var pair in m_TasksWaitingForResponse)
                {
                    if (pair.Value.Item1.Equals(endpointId))
                    {
                        messagesThatWillNotBeAnswered.Add(pair.Key);
                        pair.Value.Item2.SetCanceled();
                    }
                }

                foreach (var id in messagesThatWillNotBeAnswered)
                {
                    m_TasksWaitingForResponse.Remove(id);
                }
            }
        }

        /// <summary>
        /// Handles the case that the local channel, from which the input messages are send,
        /// is closed.
        /// </summary>
        public void OnLocalChannelClosed()
        {
            lock (m_Lock)
            {
                // No single message will get a response anymore. 
                // Nuke them all
                foreach (var pair in m_TasksWaitingForResponse)
                {
                    pair.Value.Item2.SetCanceled();
                }

                m_TasksWaitingForResponse.Clear();
            }
        }
    }
}
