//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Xml.Linq;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Handles the discovery of endpoints on other computers to which a connection can be 
    /// made via the TCP protocol.
    /// </summary>
    internal sealed class TcpBasedDiscoverySource : IDiscoverOtherServices
    {
        /// <summary>
        /// The service that handles the detection of discovery announcements.
        /// </summary>
        private ServiceHost m_Host;

        /// <summary>
        /// The object used to locate the services that are already online when the
        /// current application comes online.
        /// </summary>
        private DiscoveryClient m_DiscoveryClient;

        /// <summary>
        /// An event raised when a remote endpoint becomes available.
        /// </summary>
        public event EventHandler<ConnectionInformationEventArgs> OnEndpointBecomingAvailable;

        private void RaiseOnEndpointBecomingAvailable(ChannelConnectionInformation info)
        {
            var local = OnEndpointBecomingAvailable;
            if (local != null)
            {
                local(this, new ConnectionInformationEventArgs(info));
            }
        }

        /// <summary>
        /// An event raised when a remote endpoint becomes unavailable.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointBecomingUnavailable;
        
        private void RaiseOnEndpointBecomingUnavailable(EndpointId id)
        {
            var local = OnEndpointBecomingUnavailable;
            if (local != null)
            {
                local(this, new EndpointEventArgs(id));
            }
        }

        /// <summary>
        /// Starts the endpoint discovery process.
        /// </summary>
        public void StartDiscovery()
        {
            var service = new AnnouncementService();
            service.OnlineAnnouncementReceived += (s, e) => HandleOnlineAnnoucementReceived(e.EndpointDiscoveryMetadata);
            service.OfflineAnnouncementReceived += (se, e) => HandleOfflineAnnouncementReceived(e.EndpointDiscoveryMetadata);

            m_Host = new ServiceHost(service);
            m_Host.AddServiceEndpoint(new UdpAnnouncementEndpoint());
            m_Host.Open();

            // When we log on we also need to get the information from the services
            // that are currently already online
            LocateExistingEndpoints();
        }

        private void HandleOnlineAnnoucementReceived(EndpointDiscoveryMetadata metadata)
        {
            // Only acknowledge service contracts that we actually know about.
            if (metadata.ContractTypeNames.FirstOrDefault(x => x.Name == typeof(IReceivingEndpoint).Name) != null)
            {
                var id = GetEndpointId(metadata);
                var bindingType = GetBindingType(metadata);
                var address = metadata.Address.Uri;

                RaiseOnEndpointBecomingAvailable(new ChannelConnectionInformation(id, bindingType, address));
            }
        }

        private EndpointId GetEndpointId(EndpointDiscoveryMetadata metadata)
        {
            XElement endpointElement = metadata.Extensions.Elements("EndpointId").FirstOrDefault();
            if (endpointElement == null)
            {
                throw new MissingEndpointIdException();
            }

            return new EndpointId(endpointElement.Value);
        }

        private Type GetBindingType(EndpointDiscoveryMetadata metadata)
        {
            XElement bindingTypeElement = metadata.Extensions.Elements("BindingType").FirstOrDefault();
            if (bindingTypeElement == null)
            {
                throw new MissingBindingTypeException();
            }

            var type = Type.GetType(bindingTypeElement.Value, true, false);

            Debug.Assert(typeof(IChannelType).IsAssignableFrom(type), "Found an incorrect binding type.");
            return type;
        }

        private void HandleOfflineAnnouncementReceived(EndpointDiscoveryMetadata metadata)
        {
            // Only acknowledge service contracts that we actually know about.
            if (metadata.ContractTypeNames.FirstOrDefault(x => x.Name == typeof(IReceivingEndpoint).Name) != null)
            {
                var id = GetEndpointId(metadata);
                RaiseOnEndpointBecomingUnavailable(id);
            }
        }

        private void LocateExistingEndpoints()
        {
            m_DiscoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            m_DiscoveryClient.FindProgressChanged += (s, e) => HandleOnlineAnnoucementReceived(e.EndpointDiscoveryMetadata);
            m_DiscoveryClient.FindCompleted += new EventHandler<FindCompletedEventArgs>(OnFindCompleted);

            m_DiscoveryClient.FindAsync(new FindCriteria(typeof(IReceivingEndpoint)));
        }

        private void OnFindCompleted(object sender, FindCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    return;
                }

                if (e.Error != null)
                {
                    throw e.Error;
                }
            }
            finally
            {
                CloseDiscoveryClient();
            }
        }

        private void CloseDiscoveryClient()
        {
            if (m_DiscoveryClient != null)
            {
                if (m_DiscoveryClient.InnerChannel.State == CommunicationState.Opened)
                {
                    m_DiscoveryClient.Close();
                }

                m_DiscoveryClient = null;
            }
        }

        /// <summary>
        /// Ends the endpoint discovery process.
        /// </summary>
        public void EndDiscovery()
        {
            AbortDiscovery();

            m_Host.Close();
            m_Host = null;
        }

        private void AbortDiscovery()
        {
            CloseDiscoveryClient();
        }
    }
}
