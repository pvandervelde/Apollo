//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the base methods for communication methods.
    /// </summary>
    internal interface ICommunicationMessage
    {
        /// <summary>
        /// Gets a value indicating the ID number of the message.
        /// </summary>
        MessageId Id
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the message to which 
        /// the current message is a response.
        /// </summary>
        MessageId InResponseTo
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the endpoint that 
        /// send the current message.
        /// </summary>
        EndpointId OriginatingEndpoint
        {
            get;
        }
    }
}
