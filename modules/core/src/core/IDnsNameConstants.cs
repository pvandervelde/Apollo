//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Messaging;

namespace Apollo.Core
{
    /// <summary>
    /// Provides the <see cref="DnsName"/> objects of all the different parts of the system.
    /// </summary>
    public interface IDnsNameConstants
    {
        /// <summary>
        /// Gets the <see cref="DnsName"/> used to send messages to the kernel.
        /// </summary>
        /// <value>The requested <c>DnsName</c>.</value>
        DnsName AddressOfKernel
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="DnsName"/> used to send messages to the user interface.
        /// </summary>
        /// <value>The requested <c>DnsName</c>.</value>
        DnsName AddressOfUserInterface
        {
            get;
        }
    }
}
