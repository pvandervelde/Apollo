//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Core.UserInterfaces;

namespace Apollo.Core
{
    /// <summary>
    /// Stores the <see cref="DnsName"/> objects of the different parts of the core.
    /// </summary>
    internal sealed class DnsNameConstants : IDnsNameConstants
    {
        /// <summary>
        /// The <c>DnsName</c> of the message pipeline.
        /// </summary>
        private readonly DnsName m_PipelineAddress = new DnsName(typeof(MessagePipeline).FullName);

        /// <summary>
        /// The <c>DnsName</c> of the kernel.
        /// </summary>
        private readonly DnsName m_KernelAddress = new DnsName(typeof(CoreProxy).FullName);

        /// <summary>
        /// The <c>DnsName</c> of the User Interface service.
        /// </summary>
        private readonly DnsName m_UserInterfaceAddress = new DnsName(typeof(UserInterfaceService).FullName);

        /// <summary>
        /// The <c>DnsName</c> of the Logger service.
        /// </summary>
        private readonly DnsName m_LogSink = new DnsName(typeof(LogSink).FullName);

        #region Implementation of IDnsNameConstants

        /// <summary>
        /// Gets the <see cref="DnsName"/> used by the message pipeline. Note that the pipeline may not
        /// be listening for messages.
        /// </summary>
        /// <value>The requested <c>DnsName</c>.</value>
        public DnsName AddressOfMessagePipeline
        {
            get
            {
                return m_PipelineAddress;
            }
        }

        /// <summary>
        /// Gets the <see cref="DnsName"/> used to send messages to the kernel.
        /// </summary>
        /// <value>The requested <c>DnsName</c>.</value>
        public DnsName AddressOfKernel
        {
            get
            {
                return m_KernelAddress;
            }
        }

        /// <summary>
        /// Gets the <see cref="DnsName"/> used to send messages to the user interface.
        /// </summary>
        /// <value>The requested <c>DnsName</c>.</value>
        public DnsName AddressOfUserInterface
        {
            get
            {
                return m_UserInterfaceAddress;
            }
        }

        /// <summary>
        /// Gets the <see cref="DnsName"/> used to send messages to the logsink.
        /// </summary>
        /// <value>The requested <c>DnsName</c>.</value>
        public DnsName AddressOfLogger
        {
            get
            {
                return m_LogSink;
            }
        }

        #endregion
    }
}
