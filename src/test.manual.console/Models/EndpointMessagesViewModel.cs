//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Apollo.Core.Base.Communication;

namespace Test.Manual.Console.Models
{
    /// <summary>
    /// Stores the messages that have been received from a given endpoint.
    /// </summary>
    internal sealed class EndpointMessagesViewModel
    {
        /// <summary>
        /// The list of messages for the current endpoint.
        /// </summary>
        private readonly ObservableCollection<string> m_Messages = new ObservableCollection<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointMessagesViewModel"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public EndpointMessagesViewModel(EndpointId endpoint)
        {
            Endpoint = endpoint;
        }

        /// <summary>
        /// Gets the ID of the endpoint from which the messages have been received.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds the new message to the collection.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddMessage(string message)
        {
            m_Messages.Add(message);
        }

        /// <summary>
        /// Gets the collection of messages that have been received from the given endpoint.
        /// </summary>
        public ObservableCollection<string> Messages
        {
            get
            {
                return m_Messages;
            }
        }
    }
}
