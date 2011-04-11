//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for actions relating to specific messages.
    /// </summary>
    internal interface IMessageProcessAction
    {
        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        Type MessageTypeToProcess
        {
            get;
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        void Invoke(ICommunicationMessage message);
    }
}
