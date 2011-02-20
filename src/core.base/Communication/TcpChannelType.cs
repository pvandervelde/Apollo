//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Utils.Configuration;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines a <see cref="IChannelType"/> that uses TCP/IP connections for communication between
    /// applications different machines.
    /// </summary>
    internal sealed class TcpChannelType : IChannelType
    {
        /// <summary>
        /// Returns the DNS name of the machine.
        /// </summary>
        /// <returns>The DNS name of the machine.</returns>
        private static string MachineDnsName()
        {
            try
            {
                var searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM Win32_NetworkAdapter WHERE NetEnabled = True AND PhysicalAdapter = True");

                string deviceCaption = (from ManagementObject queryObj in searcher.Get() 
                                        select queryObj["Caption"] as string).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(deviceCaption))
                {
                    return Environment.MachineName;
                }

                searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE ServiceName = 'tunnel'");

                string dnsHostName = (from ManagementObject queryObj in searcher.Get()
                                      select queryObj["DNSHostName"] as string).FirstOrDefault();

                return (!string.IsNullOrWhiteSpace(dnsHostName)) ? dnsHostName : Environment.MachineName;
            }
            catch (ManagementException)
            {
                return Environment.MachineName;
            }
        }

        /// <summary>
        /// Returns the next available TCP/IP port.
        /// </summary>
        /// <returns>
        /// The number of the port.
        /// </returns>
        private static int GetAvailablePort()
        {
            var endPoint = new IPEndPoint(IPAddress.Any, 0);
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(endPoint);
                IPEndPoint local = (IPEndPoint)socket.LocalEndPoint;
                return local.Port;
            }
        }

        /// <summary>
        /// The object that stores the configuration values for the
        /// named pipe WCF connection.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpChannelType"/> class.
        /// </summary>
        /// <param name="namedPipeConfiguration">The configuration for the WCF named pipe channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="namedPipeConfiguration"/> is <see langword="null" />.
        /// </exception>
        public TcpChannelType(IConfiguration namedPipeConfiguration)
        {
            {
                Enforce.Argument(() => namedPipeConfiguration);
            }

            m_Configuration = namedPipeConfiguration;
        }

        /// <summary>
        /// Generates a new URI for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated URI.
        /// </returns>
        public Uri GenerateNewChannelUri()
        {
            int port = m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpPort) ?
                m_Configuration.Value<int>(CommunicationConfigurationKeys.TcpPort) : 
                GetAvailablePort();
            string address = m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpBaseAddress) ? 
                m_Configuration.Value<string>(CommunicationConfigurationKeys.TcpBaseAddress) : 
                MachineDnsName();

            var channelUri = string.Format(CultureInfo.InvariantCulture, @"net.tcp://{0}:{1}", address, port);
            return new Uri(channelUri);
        }

        /// <summary>
        /// Generates a new address for the channel endpoint.
        /// </summary>
        /// <returns>
        /// The newly generated address for the channel endpoint.
        /// </returns>
        public string GenerateNewAddress()
        {
            return m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpSubAddress) ? 
                m_Configuration.Value<string>(CommunicationConfigurationKeys.TcpSubAddress) : 
                "ApolloThroughTcp";
        }

        /// <summary>
        /// Generates a new binding object for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        public Binding GenerateBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.None, false);
            {
                binding.MaxConnections = m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpBindingMaximumNumberOfConnections) ?
                    m_Configuration.Value<int>(CommunicationConfigurationKeys.TcpBindingMaximumNumberOfConnections) : 
                    25;
                binding.ReceiveTimeout = m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpBindingReceiveTimeout) ?
                    m_Configuration.Value<TimeSpan>(CommunicationConfigurationKeys.TcpBindingReceiveTimeout) : 
                    new TimeSpan(0, 30, 00);
            }

            return binding;
        }

        /// <summary>
        /// Creates the required channel(s) to receive a data stream across the network and returns
        /// the connection information and the task responsible for handling the data reception.
        /// </summary>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <returns>
        /// The connection information necessary to connect to the newly created channel and the task 
        /// responsible for handling the data reception.
        /// </returns>
        public Tuple<StreamTransferInformation, Task<FileInfo>> PrepareForDataReception(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Transfers the data to the receiving endpoint.
        /// </summary>
        /// <param name="transferInformation">
        /// The information which describes the data to be transferred and the remote connection over
        /// which the data is transferred.
        /// </param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <returns>
        /// An task that indicates when the transfer is complete.
        /// </returns>
        public Task TransferData(StreamTransferInformation transferInformation, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
