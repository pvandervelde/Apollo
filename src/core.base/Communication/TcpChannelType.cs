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
using Apollo.Core.Base.Properties;
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
        /// <remarks>
        /// <para>
        /// If the <paramref name="localFile"/> does not exist a new file will be created with the given path. If
        /// it does exist then the data will be appended to it.
        /// </para>
        /// </remarks>
        /// <param name="localFile">The full file path to which the network stream should be written.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
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
        public Tuple<StreamTransferInformation, Task<FileInfo>> PrepareForDataReception(string localFile, CancellationToken token)
        {
            {
                Enforce.Argument(() => localFile);
                Enforce.With<ArgumentException>(!string.IsNullOrWhiteSpace(localFile), Resources.Exceptions_Messages_FilePathCannotBeEmpty);
            }

            var tcpListener = new TcpListener(IPAddress.Any, 0);
            var endpoint = tcpListener.LocalEndpoint as IPEndPoint;

            var info = new StreamTransferInformation
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
                            using (TcpClient client = tcpListener.AcceptTcpClient())
                            {
                                NetworkStream clientStream = client.GetStream();

                                byte[] message = new byte[ReadBufferSize];
                                while (!token.IsCancellationRequested)
                                {
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
                TaskScheduler.Default);

            return new Tuple<StreamTransferInformation, Task<FileInfo>>(info, result);
        }

        /// <summary>
        /// Transfers the data to the receiving endpoint.
        /// </summary>
        /// <param name="file">The file stream that contains the file that should be transferred.</param>
        /// <param name="transferInformation">
        /// The information which describes the data to be transferred and the remote connection over
        /// which the data is transferred.
        /// </param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <returns>
        /// An task that indicates when the transfer is complete.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="file"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="transferInformation"/> is <see langword="null" />.
        /// </exception>
        public Task TransferData(FileStream file, StreamTransferInformation transferInformation, CancellationToken token)
        {
            {
                Enforce.Argument(() => file);
                Enforce.Argument(() => transferInformation);
            }

            Task result = Task.Factory.StartNew(
                () =>
                {
                    // Don't catch any exception because the task will store them if we don't catch them.
                    using (var client = new TcpClient())
                    {
                        var serverEndPoint = new IPEndPoint(transferInformation.IPAddress, transferInformation.Port);

                        client.Connect(serverEndPoint);
                        using (NetworkStream clientStream = client.GetStream())
                        {
                            using (file)
                            {
                                file.Position = transferInformation.StartPosition;

                                byte[] buffer = new byte[ReadBufferSize];
                                while (!token.IsCancellationRequested)
                                {
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
                });

            return result;
        }
    }
}
