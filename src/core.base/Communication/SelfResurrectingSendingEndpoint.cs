//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Apollo.Core.Base.Properties;
using Apollo.Utilities;
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
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The service on the other side of the channel.
        /// </summary>
        private IReceivingEndpoint m_Service;

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
        /// <param name="systemDiagnostics">The object that provides the diagnostic methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelFactory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public SelfResurrectingSendingEndpoint(
            ChannelFactory<IReceivingWcfEndpointProxy> channelFactory,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Enforce.Argument(() => channelFactory);
                Enforce.Argument(() => systemDiagnostics);
            }

            m_Factory = channelFactory;
            m_Diagnostics = systemDiagnostics;
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
                    m_Diagnostics.Log(
                           LogSeverityProxy.Trace,
                           string.Format(
                               CultureInfo.InvariantCulture,
                               "Sending message of type {0}.",
                               message.GetType()));

                    service.AcceptMessage(message);
                }
            }
            catch (FaultException e)
            {
                m_Diagnostics.Log(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception occurred during the sending of message of type {0}. Exception was: {1}",
                        message.GetType(),
                        e));

                if (e.InnerException != null)
                {
                    throw new FailedToSendMessageException(Resources.Exceptions_Messages_FailedToSendMessage, e.InnerException);
                }
                else 
                {
                    // There is no point in keeping the original call stack. The original
                    // exception orginates on the other side of the channel. There is no
                    // useful stack trace to keep!
                    throw new FailedToSendMessageException();
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
                            // The channel is probably faulted so terminate it.
                            m_Diagnostics.Log(
                                LogSeverityProxy.Info, 
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Channel for endpoint at {0} has faulted. Aborting channel.",
                                    m_Factory.Endpoint.Address.Uri));
                            m_Channel.Abort();
                        }

                        m_Diagnostics.Log(
                            LogSeverityProxy.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Creating channel for endpoint at {0}.",
                                m_Factory.Endpoint.Address.Uri));
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
                    m_Diagnostics.Log(
                        LogSeverityProxy.Debug,
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            "Disposed of channel for {0}",
                            m_Factory.Endpoint.Address.Uri));
                }
                catch (CommunicationObjectAbortedException e)
                {
                    // The channel is now faulted but there is nothing
                    // we can do about that so just ignore it.
                    m_Diagnostics.Log(
                        LogSeverityProxy.Debug,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for {0} failed to close normally. Exception was: {1}",
                            m_Factory.Endpoint.Address.Uri,
                            e));
                }
                catch (TimeoutException e)
                { 
                    // The default close timeout elapsed before we were 
                    // finished closing the channel. So the channel
                    // is aborted. Nothing we can do, just ignore it.
                    m_Diagnostics.Log(
                        LogSeverityProxy.Debug,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for {0} failed to close normally. Exception was: {1}",
                            m_Factory.Endpoint.Address.Uri,
                            e));
                }
            }
        }
    }
}
