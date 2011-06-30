//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Apollo.Core.Base.Properties;
using Apollo.Utilities.Configuration;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines a <see cref="IChannelType"/> that uses TCP/IP connections for communication between
    /// applications different machines.
    /// </summary>
    [ChannelRelativePerformanceAttribute(2)]
    [ExcludeFromCodeCoverage]
    internal sealed class TcpChannelType : IChannelType
    {
        /// <summary>
        /// Defines the default buffer size used for reading and writing.
        /// </summary>
        private const int ReadBufferSize = 4096;

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
        private static int DetermineNextAvailablePort()
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
        /// Returns the process ID of the process that is currently executing
        /// this code.
        /// </summary>
        /// <returns>
        /// The ID number of the current process.
        /// </returns>
        private static int CurrentProcessId()
        {
            var process = Process.GetCurrentProcess();
            return process.Id;
        }

        /// <summary>
        /// The object that stores the configuration values for the
        /// named pipe WCF connection.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// A flag that indicates if the TCP channel should participate in the UDP 
        /// discovery or not.
        /// </summary>
        private readonly bool m_ShouldProvideDiscovery;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpChannelType"/> class.
        /// </summary>
        /// <param name="tcpConfiguration">The configuration for the WCF tcp channel.</param>
        /// <param name="shouldProvideDiscovery">
        ///     A flag that indicates if the TCP channels should participate in the UDP discovery.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="tcpConfiguration"/> is <see langword="null" />.
        /// </exception>
        public TcpChannelType(IConfiguration tcpConfiguration, bool shouldProvideDiscovery)
        {
            {
                Enforce.Argument(() => tcpConfiguration);
            }

            m_Configuration = tcpConfiguration;
            m_ShouldProvideDiscovery = shouldProvideDiscovery;
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
                DetermineNextAvailablePort();
            string address = m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpBaseAddress) ? 
                m_Configuration.Value<string>(CommunicationConfigurationKeys.TcpBaseAddress) : 
                MachineDnsName();

            var channelUri = string.Format(CultureInfo.InvariantCulture, @"net.tcp://{0}:{1}", address, port);
            return new Uri(channelUri);
        }

        /// <summary>
        /// Generates a new binding object for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        public Binding GenerateBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.None, false)
            {
                MaxConnections = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) ?
                    m_Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) :
                    25,
                ReceiveTimeout = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingReceiveTimeout) ?
                    m_Configuration.Value<TimeSpan>(CommunicationConfigurationKeys.BindingReceiveTimeout) :
                    new TimeSpan(0, 30, 00),
            };

            return binding;
        }

        /// <summary>
        /// Attaches a new endpoint to the given host.
        /// </summary>
        /// <param name="host">The host to which the endpoint should be attached.</param>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <param name="localEndpoint">The ID of the local endpoint, to be used in the endpoint metadata.</param>
        /// <returns>The newly attached endpoint.</returns>
        public ServiceEndpoint AttachEndpoint(ServiceHost host, Type implementedContract, EndpointId localEndpoint)
        {
            // Add the normal endpoint
            var endpoint = host.AddServiceEndpoint(implementedContract, GenerateBinding(), GenerateNewAddress());
            if (m_ShouldProvideDiscovery)
            {
                var discoveryBehavior = new ServiceDiscoveryBehavior();
                discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
                host.Description.Behaviors.Add(discoveryBehavior);
                host.Description.Endpoints.Add(new UdpDiscoveryEndpoint());

                // As additional information add the EndpointId of the current endpoint.
                var endpointDiscoveryBehavior = new EndpointDiscoveryBehavior();
                endpointDiscoveryBehavior.Extensions.Add(new XElement("root", new XElement("EndpointId", localEndpoint.ToString())));
                endpointDiscoveryBehavior.Extensions.Add(new XElement("root", new XElement("BindingType", GetType().FullName)));
                endpoint.Behaviors.Add(endpointDiscoveryBehavior);
            }

            return endpoint;
        }

        /// <summary>
        /// Generates a new address for the channel endpoint.
        /// </summary>
        /// <returns>
        /// The newly generated address for the channel endpoint.
        /// </returns>
        public string GenerateNewAddress()
        {
            return m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpSubaddress) ?
                m_Configuration.Value<string>(CommunicationConfigurationKeys.TcpSubaddress) :
                string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "ApolloThroughTcp", CurrentProcessId());
        }

        /// <summary>
        /// Creates the required channel(s) to receive a data stream across the network and returns
        /// the connection information and the task responsible for handling the data reception.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the <paramref name="localFile"/> does not exist a new file will be created with the given path. If
        /// it does exist then the data will be appended to it.
        /// </para>
        /// </remarks>
        /// <param name="localFile">The full file path to which the network stream should be written.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <param name="scheduler">The scheduler that is used to run the return task.</param>
        /// <returns>
        /// The connection information necessary to connect to the newly created channel and the task 
        /// responsible for handling the data reception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localFile"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="localFile"/> is an empty string.
        /// </exception>
        public Tuple<StreamTransferInformation, Task<FileInfo>> PrepareForDataReception(
            string localFile, 
            CancellationToken token,
            TaskScheduler scheduler)
        {
            {
                Enforce.Argument(() => localFile);
                Enforce.With<ArgumentException>(!string.IsNullOrWhiteSpace(localFile), Resources.Exceptions_Messages_FilePathCannotBeEmpty);
            }

            var tcpListener = new TcpListener(IPAddress.Any, 0);
            var endpoint = tcpListener.LocalEndpoint as IPEndPoint;
            var info = new TcpStreamTransferInformation
                {
                    StartPosition = File.Exists(localFile) ? new FileInfo(localFile).Length : 0,
                    IPAddress = endpoint.Address,
                    Port = endpoint.Port,
                };

            Task<FileInfo> result = Task<FileInfo>.Factory.StartNew(
                () =>
                {
                    // Don't catch any exception because the Task will store them if we don't catch them
                    try
                    {
                        using (var file = new FileStream(localFile, FileMode.Append, FileAccess.Write, FileShare.None))
                        {
                            tcpListener.Start();

                            // Wait for some connection to appear but don't block so that we can
                            // check if the transfer should be cancelled.
                            while (!tcpListener.Pending())
                            {
                                if (token.IsCancellationRequested)
                                {
                                    token.ThrowIfCancellationRequested();
                                }

                                Thread.Sleep(10);
                            }

                            // There should be a connection now so connect to it and then
                            // extract the data from the network stream
                            using (TcpClient client = tcpListener.AcceptTcpClient())
                            {
                                NetworkStream clientStream = client.GetStream();

                                byte[] message = new byte[ReadBufferSize];
                                while (true)
                                {
                                    if (token.IsCancellationRequested)
                                    {
                                        token.ThrowIfCancellationRequested();
                                    }

                                    int bytesRead = clientStream.Read(message, 0, message.Length);
                                    if (bytesRead == 0)
                                    {
                                        break;
                                    }

                                    file.Write(message, 0, bytesRead);
                                }
                            }
                        }
                    }
                    finally
                    {
                        tcpListener.Stop();
                    }

                    return new FileInfo(localFile);
                },
                token,
                TaskCreationOptions.LongRunning,
                scheduler);

            return new Tuple<StreamTransferInformation, Task<FileInfo>>(info, result);
        }

        /// <summary>
        /// Transfers the data to the receiving endpoint.
        /// </summary>
        /// <param name="filePath">The file path to the file that should be transferred.</param>
        /// <param name="transferInformation">
        /// The information which describes the data to be transferred and the remote connection over
        /// which the data is transferred.
        /// </param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <param name="scheduler">The scheduler that is used to run the return task.</param>
        /// <returns>
        /// An task that indicates when the transfer is complete.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="filePath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="filePath"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="transferInformation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="transferInformation"/> is not a <see cref="NamedPipeStreamTransferInformation"/> object.
        /// </exception>
        public Task TransferData(
            string filePath, 
            StreamTransferInformation transferInformation, 
            CancellationToken token,
            TaskScheduler scheduler)
        {
            {
                Enforce.Argument(() => filePath);
                Enforce.With<ArgumentException>(
                    !string.IsNullOrWhiteSpace(filePath),
                    Resources.Exceptions_Messages_FilePathCannotBeEmpty);

                Enforce.Argument(() => transferInformation);
                Enforce.With<ArgumentException>(
                    transferInformation is TcpStreamTransferInformation, 
                    Resources.Exceptions_Messages_IncorrectStreamTransferInformationObjectFound_WithTypes,
                    typeof(TcpStreamTransferInformation),
                    transferInformation.GetType());
            }

            var transfer = transferInformation as TcpStreamTransferInformation;
            Task result = Task.Factory.StartNew(
                () =>
                {
                    // Don't catch any exception because the task will store them if we don't catch them.
                    using (var client = new TcpClient())
                    {
                        var serverEndPoint = new IPEndPoint(transfer.IPAddress, transfer.Port);

                        client.Connect(serverEndPoint);
                        using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            using (NetworkStream clientStream = client.GetStream())
                            {
                                file.Position = transferInformation.StartPosition;

                                byte[] buffer = new byte[ReadBufferSize];
                                while (true)
                                {
                                    if (token.IsCancellationRequested)
                                    {
                                        token.ThrowIfCancellationRequested();
                                    }

                                    int bytesRead = file.Read(buffer, 0, ReadBufferSize);
                                    if (bytesRead == 0)
                                    {
                                        break;
                                    }

                                    clientStream.Write(buffer, 0, bytesRead);
                                    clientStream.Flush();
                                }
                            }
                        }
                    }
                },
                token,
                TaskCreationOptions.LongRunning,
                scheduler);

            return result;
        }
    }
}
