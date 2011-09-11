//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Properties;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines a collection that contains notification objects for the local endpoint.
    /// </summary>
    internal sealed class LocalNotificationCollection : INotificationSendersCollection, ISendNotifications
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection of registered notification.
        /// </summary>
        /// <remarks>
        /// We only ever add to this collection so there's no need to protect it against multiple threads
        /// accessing it.
        /// </remarks>
        private readonly IDictionary<Type, INotificationSet> m_Notifications
            = new SortedList<Type, INotificationSet>(new TypeComparer());

        /// <summary>
        /// The collection that maps an notification to a collection of registered listeners.
        /// </summary>
        private readonly IDictionary<ISerializedEventRegistration, List<EndpointId>> m_RegisteredListeners
            = new SortedList<ISerializedEventRegistration, List<EndpointId>>();

        /// <summary>
        /// The communication layer that is used to send out messages about newly
        /// registered commands.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalNotificationCollection"/> class.
        /// </summary>
        /// <param name="layer">
        ///     The communication layer that is used to send out messages about newly registered notifications.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        public LocalNotificationCollection(ICommunicationLayer layer)
        {
            {
                Enforce.Argument(() => layer);
            }

            m_Layer = layer;
        }

        /// <summary>
        /// Registers a <see cref="INotificationSet"/> object.
        /// </summary>
        /// <para>
        /// A proper notification set class has the following characteristics:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The interface must derrive from <see cref="INotificationSet"/>.</description>
        ///     </item>
        ///     <item>
        ///         <description>The interface must only have events, no properties or methods.</description>
        ///     </item>
        ///     <item>
        ///         <description>Each event be based on <see cref="EventHandler{T}"/> delegate.</description>
        ///     </item>
        ///     <item>
        ///         <description>The event must be based on a closed constructed type.</description>
        ///     </item>
        ///     <item>
        ///         <description>The <see cref="EventArgs"/> of <see cref="EventHandler{T}"/> must be serializable.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// <param name="notificationType">The interface that defines the notification events.</param>
        /// <param name="notifications">The notification object.</param>
        public void Register(Type notificationType, INotificationSet notifications)
        {
            {
                Enforce.Argument(() => notificationType);
                Enforce.Argument(() => notifications);
                Enforce.With<ArgumentException>(
                    notificationType.IsAssignableFrom(notifications.GetType()),
                    Resources.Exceptions_Messages_NotificationObjectMustImplementNotificationInterface);
            }

            CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(notificationType);
            if (m_Notifications.ContainsKey(notificationType))
            {
                throw new CommandAlreadyRegisteredException();
            }
            
            ConnectToEvents(notifications);

            m_Notifications.Add(notificationType, notifications);
            if (m_Layer.IsSignedIn)
            {
                foreach (var endpoint in m_Layer.KnownEndpoints())
                {
                    m_Layer.SendMessageTo(endpoint, new NewNotificationRegisteredMessage(m_Layer.Id, notificationType));
                }
            }
        }

        private void ConnectToEvents(INotificationSet notifications)
        {
            var events = notifications.GetType().GetEvents();
            foreach (var eventInfo in events)
            {
                var serializedInfo = ProxyExtensions.FromEventInfo(eventInfo);
                EventHandler<EventArgs> handler = 
                    (s, e) => 
                    {
                        HandleEventAndForwardToListeners(s, serializedInfo, e);
                    };
                eventInfo.AddEventHandler(notifications, handler);
            }
        }

        private void HandleEventAndForwardToListeners(object sender, ISerializedEventRegistration originatingEvent, EventArgs args)
        {
            List<EndpointId> endpoints = null;
            lock (m_Lock)
            {
                if (m_RegisteredListeners.ContainsKey(originatingEvent))
                {
                    endpoints = new List<EndpointId>(m_RegisteredListeners[originatingEvent]);
                }
            }

            if (endpoints != null)
            {
                foreach (var endpoint in endpoints)
                {
                    m_Layer.SendMessageTo(endpoint, new NotificationRaisedMessage(m_Layer.Id, originatingEvent, args));
                }
            }
        }

        /// <summary>
        /// Returns the notification object that was registered for the given interface type.
        /// </summary>
        /// <param name="interfaceType">The <see cref="INotificationSet"/> derived interface type.</param>
        /// <returns>
        /// The desired notification set.
        /// </returns>
        public INotificationSet NotificationsFor(Type interfaceType)
        {
            if (!m_Notifications.ContainsKey(interfaceType))
            {
                return null;
            }

            return m_Notifications[interfaceType];
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<KeyValuePair<Type, INotificationSet>> GetEnumerator()
        {
            return m_Notifications.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through 
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Registers a specific endpoint so that it may be notified when the specified event
        /// is raised.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="notification">The object that describes to which event the endpoint wants to be subscribed.</param>
        public void RegisterForNotification(EndpointId endpoint, ISerializedEventRegistration notification)
        {
            {
                Enforce.Argument(() => endpoint);
                Enforce.Argument(() => notification);
            }
            
            lock (m_Lock)
            {
                if (!m_RegisteredListeners.ContainsKey(notification))
                {
                    m_RegisteredListeners.Add(notification, new List<EndpointId>());
                }

                var list = m_RegisteredListeners[notification];
                if (!list.Contains(endpoint))
                {
                    list.Add(endpoint);
                }
            }
        }

        /// <summary>
        /// Deregisters a specific endpoint so that it will no longer be notified when the specified event
        /// is raised.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="notification">The object that describes from which event the endpoint wants to be unsubscribed.</param>
        public void UnregisterFromNotification(EndpointId endpoint, ISerializedEventRegistration notification)
        {
            {
                Enforce.Argument(() => endpoint);
                Enforce.Argument(() => notification);
            }

            lock (m_Lock)
            {
                if (m_RegisteredListeners.ContainsKey(notification))
                {
                    var list = m_RegisteredListeners[notification];
                    if (list.Remove(endpoint))
                    {
                        if (list.Count == 0)
                        {
                            m_RegisteredListeners.Remove(notification);
                        }
                    }
                }
            }
        }
    }
}
