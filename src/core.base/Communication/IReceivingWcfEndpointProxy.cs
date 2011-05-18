//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for a proxy that handles all message sending. This interface
    /// is only used by the WCF <see cref="ChannelFactory{T}"/>.
    /// </summary>
    internal interface IReceivingWcfEndpointProxy : IReceivingEndpoint, IOutputChannel
    {
    }
}
