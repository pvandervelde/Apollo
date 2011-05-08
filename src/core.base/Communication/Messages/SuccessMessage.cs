﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that indicates that a certain action has succeeded.
    /// </summary>
    [Serializable]
    internal sealed class SuccessMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessMessage"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint which send the current message.</param>
        /// <param name="inResponseTo">The ID number of the message to which the current message is a response.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        public SuccessMessage(EndpointId endpoint, MessageId inResponseTo)
            : base(endpoint, inResponseTo)
        {
        }
    }
}
