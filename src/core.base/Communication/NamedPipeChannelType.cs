//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Properties;
using Apollo.Utilities.Configuration;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines a <see cref="IChannelType"/> that uses named pipes for communication between
    /// applications on the same local machine.
    /// </summary>
    [ChannelRelativePerformanceAttribute(1)]
    internal sealed class NamedPipeChannelType : IChannelType
    {
        /// <summary>
        /// Defines the default buffer size used for reading and writing.
        /// </summary>
        private const int ReadBufferSize = 4096;

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
        /// Initializes a new instance of the <see cref="NamedPipeChannelType"/> class.
        /// </summary>
        /// <param name="namedPipeConfiguration">The configuration for the WCF named pipe channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="namedPipeConfiguration"/> is <see langword="null" />.
        /// </exception>
        public NamedPipeChannelType(IConfiguration namedPipeConfiguration)
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
            var channelUri = string.Format(CultureInfo.InvariantCulture, "net.pipe://localhost/apollo/pipe_{0}", CurrentProcessId());
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
            return m_Configuration.HasValueFor(CommunicationConfigurationKeys.NamedPipeSubaddress) ?
                m_Configuration.Value<string>(CommunicationConfigurationKeys.NamedPipeSubaddress) :
                string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "ApolloThroughNamedPipe", CurrentProcessId());
        }

        /// <summary>
        /// Generates a new binding object for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        public Binding GenerateBinding()
        {
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None) 
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
            return host.AddServiceEndpoint(implementedContract, GenerateBinding(), GenerateNewAddress());
        }

        /// <summary>
        /// Creates the required channel(s) to receive a data stream across the network and returns
        /// the connection information and the task responsible for handling the data reception.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the <paramref name="localFile"/> does not exist a new file will be created with the given path. If
        /// it does exist then the data will be appended to it. This means that if the transfer is interupted
        /// for some reason then it is possible to call this method again on the half-finished file. In that case only the 
        /// missing bit of the file will be transferred.
        /// </para>
        /// <para>
        /// Note that this method is not able to tell the difference between a transfer that is finished or one where the
        /// other side of the pipe disappears. This is due to the fact that the named pipe implementation does not
        /// provide methods to detect if the other side of the pipe still exists.
        /// </para>
        /// </remarks>
        /// <param name="localFile">The full file path to which the network stream should be written.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <param name="scheduler">The scheduler used to run the return task.</param>
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
        public System.Tuple<StreamTransferInformation, Task<FileInfo>> PrepareForDataReception(
            string localFile, 
            CancellationToken token,
            TaskScheduler scheduler)
        {
            {
                Enforce.Argument(() => localFile);
                Enforce.With<ArgumentException>(!string.IsNullOrWhiteSpace(localFile), Resources.Exceptions_Messages_FilePathCannotBeEmpty);
            }

            var pipeAddress = Guid.NewGuid().ToString();
            var pipe = new NamedPipeServerStream(pipeAddress);

            var info = new NamedPipeStreamTransferInformation
            {
                StartPosition = File.Exists(localFile) ? new FileInfo(localFile).Length : 0,
                Name = pipeAddress,
            };

            Task<FileInfo> result = Task<FileInfo>.Factory.StartNew(
                () =>
                {
                    // Don't catch any exception because the Task will store them if we don't catch them
                    using (var file = new FileStream(localFile, FileMode.Append, FileAccess.Write, FileShare.None))
                    {
                        using (pipe)
                        {
                            pipe.WaitForConnection();

                            byte[] message = new byte[ReadBufferSize];
                            while (true)
                            {
                                if (token.IsCancellationRequested)
                                {
                                    token.ThrowIfCancellationRequested();
                                }

                                // In theory we could just write the file name to the pipe
                                // and then read that on the other side ...
                                // Note that there is no way to detect if the client is still
                                // pushing data into the pipe or not. So from here we cannot 
                                // determine if the file transfer has completed!
                                int bytesRead = pipe.Read(message, 0, message.Length);
                                if (bytesRead == 0)
                                {
                                    break;
                                }

                                file.Write(message, 0, bytesRead);
                            }
                        }
                    }

                    return new FileInfo(localFile);
                },
                token,
                TaskCreationOptions.LongRunning,
                scheduler ?? TaskScheduler.Default);

            return new System.Tuple<StreamTransferInformation, Task<FileInfo>>(info, result);
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
                    transferInformation is NamedPipeStreamTransferInformation,
                    Resources.Exceptions_Messages_IncorrectStreamTransferInformationObjectFound_WithTypes,
                    typeof(NamedPipeStreamTransferInformation),
                    transferInformation.GetType());
            }

            var transfer = transferInformation as NamedPipeStreamTransferInformation;
            Task result = Task.Factory.StartNew(
                () =>
                {
                    // Don't catch any exception because the task will store them if we don't catch them.
                    using (NamedPipeClientStream pipe = new NamedPipeClientStream(transfer.Name))
                    {
                        pipe.Connect();
                        using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
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

                                pipe.Write(buffer, 0, bytesRead);
                                pipe.Flush();
                            }
                        }
                    }
                },
                token,
                TaskCreationOptions.LongRunning,
                scheduler ?? TaskScheduler.Default);

            return result;
        }
    }
}
