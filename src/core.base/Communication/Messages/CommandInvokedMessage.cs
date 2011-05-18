//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that indicates that the an <see cref="ICommandSet"/> method was
    /// invoked.
    /// </summary>
    [Serializable]
    internal sealed class CommandInvokedMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvokedMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="methodInvocation">The information about the <see cref="ICommandSet"/> method that was invoked.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="methodInvocation"/> is <see langword="null" />.
        /// </exception>
        public CommandInvokedMessage(EndpointId origin, ISerializedMethodInvocation methodInvocation)
            : base(origin)
        {
            {
                Enforce.Argument(() => methodInvocation);
            }

            Invocation = methodInvocation;
        }

        /// <summary>
        /// Gets information about the <see cref="ICommandSet"/> method that was invoked.
        /// </summary>
        public ISerializedMethodInvocation Invocation
        {
            get;
            private set;
        }
    }
}
