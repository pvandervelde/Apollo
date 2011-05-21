﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Apollo.Core.Base.Communication;

namespace Test.Manual.Console.Models
{
    /// <summary>
    /// Stores information about the currently known and active connections.
    /// </summary>
    internal sealed class ConnectionViewModel
    {
        /// <summary>
        /// The collection of endpoints that can be contacted.
        /// </summary>
        private readonly ObservableCollection<ConnectionInformationViewModel> m_KnownEndpoints 
            = new ObservableCollection<ConnectionInformationViewModel>();

        /// <summary>
        /// The collection of endpoints for the application.
        /// </summary>
        private readonly ObservableCollection<ConnectionInformationViewModel> m_LocalEndpoints 
            = new ObservableCollection<ConnectionInformationViewModel>();

        /// <summary>
        /// The collection of messages that have been received from the active endpoints.
        /// </summary>
        private readonly ObservableCollection<EndpointMessagesViewModel> m_Messages 
            = new ObservableCollection<EndpointMessagesViewModel>();

        /// <summary>
        /// The thread dispatcher that is connected to the window.
        /// </summary>
        private readonly Dispatcher m_Dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionViewModel"/> class.
        /// </summary>
        /// <param name="windowDispatcher">
        /// The dispatcher that is connected to the main display window.
        /// </param>
        public ConnectionViewModel(Dispatcher windowDispatcher)
        {
            m_Dispatcher = windowDispatcher;
        }

        /// <summary>
        /// Adds a new known endpoint.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        public void AddKnownEndpoint(ConnectionInformationViewModel endpoint)
        {
            Action action = () => m_KnownEndpoints.Add(endpoint);
            InvokeAction(action);
        }

        private void InvokeAction(Action action)
        {
            if (!m_Dispatcher.CheckAccess())
            {
                m_Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Removes a known endpoint.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        public void RemoveKnownEndpoint(EndpointId endpoint)
        {
            Action action = () =>
                {
                    var toRemove = new List<ConnectionInformationViewModel>();
                    foreach (var storedEndpoint in m_KnownEndpoints)
                    {
                        if (storedEndpoint.Id.Equals(endpoint))
                        {
                            toRemove.Add(storedEndpoint);
                        }
                    }

                    foreach (var storedEndpoint in toRemove)
                    {
                        m_KnownEndpoints.Remove(storedEndpoint);
                    }
                };
            InvokeAction(action);
        }

        /// <summary>
        /// Gets a collection containing information about all the known endpoints.
        /// </summary>
        public ObservableCollection<ConnectionInformationViewModel> KnownEndpoints
        {
            get
            {
                return m_KnownEndpoints;
            }
        }

        /// <summary>
        /// Adds a new local endpoint.
        /// </summary>
        /// <param name="localEndpoint">The endpoint.</param>
        public void AddLocalEndpoint(ConnectionInformationViewModel localEndpoint)
        {
            Action action = () => m_LocalEndpoints.Add(localEndpoint);
            InvokeAction(action);
        }

        /// <summary>
        /// Gets a collection containing information about all the endpoints for the
        /// current application.
        /// </summary>
        public ObservableCollection<ConnectionInformationViewModel> LocalEndpoints
        {
            get
            {
                return m_LocalEndpoints;
            }
        }

        /// <summary>
        /// Adds a new message for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="message">The message.</param>
        public void AddNewMessage(EndpointId endpoint, string message)
        {
            Action action = () =>
                {
                    foreach (var messageSet in m_Messages)
                    {
                        if (messageSet.Endpoint.Equals(endpoint))
                        {
                            messageSet.AddMessage(message);
                            return;
                        }
                    }

                    var newMessageSet = new EndpointMessagesViewModel(endpoint);
                    newMessageSet.AddMessage(message);
                    m_Messages.Add(newMessageSet);
                };
            InvokeAction(action);
        }

        /// <summary>
        /// Gets a collection containing all the messages that were send from the currently
        /// connected endpoints.
        /// </summary>
        public ObservableCollection<EndpointMessagesViewModel> EndpointMessages
        {
            get
            {
                return m_Messages;
            }
        }
    }
}
