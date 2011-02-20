//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utils.Configuration;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKey"/> objects for the communication layers.
    /// </summary>
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
        public static readonly ConfigurationKey TcpSubAddress = new ConfigurationKey();

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the maximum
        /// number of connections for the TCP binding .
        /// </summary>
        public static readonly ConfigurationKey TcpBindingMaximumNumberOfConnections = new ConfigurationKey();

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the receive timeout
        /// for the TCP binding .
        /// </summary>
        public static readonly ConfigurationKey TcpBindingReceiveTimeout = new ConfigurationKey();
    }
}
