//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// An <see cref="EventArgs"/> object that carries a message.
    /// </summary>
    internal sealed class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageEventArgs(ICommunicationMessage message)
        {
            {
                Enforce.Argument(() => message);
            }

            Message = message;
        }

        /// <summary>
        /// Gets a value indicating the message.
        /// </summary>
        public ICommunicationMessage Message
        {
            get;
            private set;
        }
    }
}
