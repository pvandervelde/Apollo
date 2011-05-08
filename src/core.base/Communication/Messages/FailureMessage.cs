//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that indicates that a certain action has failed.
    /// </summary>
    [Serializable]
    internal sealed class FailureMessage : CommunicationMessage
    {
        // Some kind of error message stuff. Probably an error code or an error text string
        // We can't really send exception information over because that might
        // be a security problem

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMessage"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint which send the current message.</param>
        /// <param name="inResponseTo">The ID number of the message to which the current message is a response.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        public FailureMessage(EndpointId endpoint, MessageId inResponseTo)
            : base(endpoint, inResponseTo)
        {
        }
    }
}
