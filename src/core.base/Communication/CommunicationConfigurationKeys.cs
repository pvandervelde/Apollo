//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utils;
using Apollo.Utils.Configuration;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKey"/> objects for the communication layers.
    /// </summary>
    [ExcludeFromCoverage("The configuration keys will be tested in the integration tests.")]
    internal static class CommunicationConfigurationKeys
    {
        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the TCP port (int).
        /// </summary>
        public static readonly ConfigurationKey TcpPort = new ConfigurationKey();

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the TCP base address (string).
        /// </summary>
        public static readonly ConfigurationKey TcpBaseAddress = new ConfigurationKey();

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the TCP sub-address (string).
        /// </summary>
        public static readonly ConfigurationKey TcpSubaddress = new ConfigurationKey();

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the named pipe sub-address (string).
        /// </summary>
        public static readonly ConfigurationKey NamedPipeSubaddress = new ConfigurationKey();

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the maximum
        /// number of connections for the WCF binding .
        /// </summary>
        public static readonly ConfigurationKey BindingMaximumNumberOfConnections = new ConfigurationKey();

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the receive timeout
        /// for the WCF binding .
        /// </summary>
        public static readonly ConfigurationKey BindingReceiveTimeout = new ConfigurationKey();
    }
}
