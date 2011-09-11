//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Reports the registration of new commands on remote endpoints.
    /// </summary>
    internal sealed class ManualProxyRegistrationReporter : IReportNewProxies, IAcceptExternalProxyInformation
    {
        /// <summary>
        /// An event raised when a new remote command is registered.
        /// </summary>
        public event EventHandler<ProxyInformationEventArgs> OnNewProxyRegistered;

        private void RaiseOnNewProxyRegistered(EndpointId endpoint, ISerializedType proxy)
        {
            var local = OnNewProxyRegistered;
            if (local != null)
            {
                local(this, new ProxyInformationEventArgs(endpoint, proxy));
            }
        }

        /// <summary>
        /// Stores or forwards information about a proxy that has recently been
        /// registered at a remote endpoint.
        /// </summary>
        /// <param name="id">The ID of the endpoint.</param>
        /// <param name="command">The recently registered proxy.</param>
        public void RecentlyRegisteredProxy(EndpointId id, ISerializedType command)
        {
            RaiseOnNewProxyRegistered(id, command);
        }
    }
}
