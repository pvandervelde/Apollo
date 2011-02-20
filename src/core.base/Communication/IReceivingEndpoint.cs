//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Net.Security;
using System.ServiceModel;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for receiving messages on the receiving side of the communication channel.
    /// </summary>
    [ServiceContract()]
    internal interface IReceivingEndpoint
    {
        /// <summary>
        /// Accepts the messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [UseNetDataContractSerializer]
        [OperationContract(
            IsOneWay = true, 
            IsInitiating = true, 
            IsTerminating = false, 
            ProtectionLevel = ProtectionLevel.None)]
        void AcceptMessage(ICommunicationMessage message);
    }
}
