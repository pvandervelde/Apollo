//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Apollo.Core.Base.Properties;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the sending end of a WCF channel which can resurrect the channel if it faults.
    /// </summary>
    /// <source>
    /// Original idea obtained from http://kentb.blogspot.com/2010/01/wcf-channels-faulting-and-dependency.html
    /// </source>
    internal sealed class SelfResurrectingSendingEndpoint : IChannelProxy
    {
        /// <summary>
        /// The lock object used for getting locks on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The factory which creates new WCF channels.
        /// </summary>
        private readonly ChannelFactory<IReceivingWcfEndpointProxy> m_Factory;

        /// <summary>
        /// The service on the other side of the channel.
        /// </summary>
        private IReceivingEndpoint m_Service; // @TODO: should this be marked volatile?

        /// <summary>
        /// The channel that handles the connections.
        /// </summary>
        private IChannel m_Channel;

        /// <summary>
        /// Indicates if the current endpoint has been disposed.
        /// </summary>
        private volatile bool m_IsDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfResurrectingSendingEndpoint"/> class.
        /// </summary>
        /// <param name="channelFactory">The factory that is used to create new channels.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelFactory"/> is <see langword="null" />.
        /// </exception>
        public SelfResurrectingSendingEndpoint(ChannelFactory<IReceivingWcfEndpointProxy> channelFactory)
        {
            {
                Enforce.Argument(() => channelFactory);
            }

            m_Factory = channelFactory;
        }

        /// <summary>
        /// Sends the given message.
        /// </summary>
        /// <param name="message">The message to be send.</param>
        public void Send(ICommunicationMessage message)
        {
            EnsureChannelIsAvailable();

            try
            {
                var service = m_Service;
                if (!m_IsDisposed)
                {
                    service.AcceptMessage(message);
                }
            }
            catch (FaultException e)
            {
                // The receiving end threw an exception.
                if (e.InnerException != null)
                {
                    throw new FailedToSendMessageException(Resources.Exceptions_Messages_FailedToSendMessage, e.InnerException);
                }
            }
            catch (CommunicationException e)
            { 
                // Either the connection was aborted or faulted (although it shouldn't be)
                // or something else nasty went wrong.
                throw new FailedToSendMessageException(Resources.Exceptions_Messages_FailedToSendMessage, e);
            }
        }

        private void EnsureChannelIsAvailable()
        {
            if (ShouldCreateChannel)
            {
                lock (m_Lock)
                {
                    if (ShouldCreateChannel)
                    {
                        if (m_Channel != null)
                        { 
                            try
                            {
                                // The channel is probably faulted so terminate it.
                                m_Channel.Abort();
                            }
                            catch (Exception)
                            {
                                // Just ignore anything. We're ditching the channel anyway.
                            }
                        }

                        m_Service = m_Factory.CreateChannel();
                        m_Channel = (IChannel)m_Service;
                    }
                }
            }
        }

        private bool ShouldCreateChannel
        {
            get
            {
                return (!m_IsDisposed) && ((m_Channel == null) || (m_Channel.State == CommunicationState.Faulted));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_IsDisposed)
            {
                // We've already disposed of the channel. Job done.
                return;
            }

            m_IsDisposed = true;

            IChannel local = null;
            lock (m_Lock)
            {
                local = m_Channel;
                m_Channel = null;
            }

            if (local != null && local.State != CommunicationState.Faulted)
            {
                try
                {
                    local.Close();
                }
                catch (CommunicationObjectAbortedException)
                {
                    // The channel is now faulted but there is nothing
                    // we can do about that so just ignore it.
                }
                catch (TimeoutException)
                { 
                    // The default close timeout elapsed before we were 
                    // finished closing the channel. So the channel
                    // is aborted. Nothing we can do, just ignore it.
                }
            }
        }
    }
}
