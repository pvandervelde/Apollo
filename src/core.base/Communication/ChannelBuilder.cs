//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Builds new communication channels based on the information provided.
    /// </summary>
    internal sealed class ChannelBuilder
    {
        /// <summary>
        /// Builds a new communication channel for the given endpoint.
        /// </summary>
        /// <param name="endpointToConnectTo">The endpoint to which the new channel must connect.</param>
        /// <returns>
        /// The newly created endpoint.
        /// </returns>
        public ICommunicationChannel Build(ChannelConnectionInformation endpointToConnectTo)
        {
            throw new NotImplementedException();
        }
    }
}
