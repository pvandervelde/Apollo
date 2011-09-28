﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that requests information from the receiving endpoint about its commands.
    /// </summary>
    [Serializable]
    internal sealed class CommandInformationRequestMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInformationRequestMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        public CommandInformationRequestMessage(EndpointId origin)
            : base(origin)
        {
        }
    }
}