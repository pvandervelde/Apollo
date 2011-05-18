//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;

namespace Test.Manual.Console
{
    /// <summary>
    /// Defines a message that carries a simple text string.
    /// </summary>
    [Serializable]
    internal sealed class EchoMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoMessage"/> class.
        /// </summary>
        /// <param name="id">The ID number of the local endpoint.</param>
        /// <param name="message">The message text.</param>
        public EchoMessage(EndpointId id, string message)
            : base(id)
        {
            Text = message;
        }

        /// <summary>
        /// Gets the echo message text.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }
    }
}
