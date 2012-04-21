//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationInformationRequestProcessActionTest
    {
        public interface IMockNotificationSetWithTypedEventHandler : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        [Test]
        public void MessageTypeToProcess()
        {
            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { };
            var collection = new Mock<INotificationSendersCollection>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new NotificationInformationRequestProcessAction(endpoint, sendAction, collection.Object, systemDiagnostics);
            Assert.AreEqual(typeof(NotificationInformationRequestMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            var actionObject = new Mock<IMockNotificationSetWithTypedEventHandler>();
            var notificationSets = new List<KeyValuePair<Type, INotificationSet>> 
                { 
                    new KeyValuePair<Type, INotificationSet>(typeof(IMockNotificationSetWithTypedEventHandler), actionObject.Object)
                };

            var endpoint = new EndpointId("id");

            EndpointId storedEndpoint = null;
            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
            {
                storedEndpoint = e;
                storedMsg = m;
            };

            var collection = new Mock<INotificationSendersCollection>();
            {
                collection.Setup(c => c.GetEnumerator())
                    .Returns(notificationSets.GetEnumerator());
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new NotificationInformationRequestProcessAction(endpoint, sendAction, collection.Object, systemDiagnostics);
            
            var otherEndpoint = new EndpointId("otherId");
            action.Invoke(new NotificationInformationRequestMessage(otherEndpoint));

            Assert.AreSame(otherEndpoint, storedEndpoint);
            Assert.IsInstanceOfType(typeof(EndpointProxyTypesResponseMessage), storedMsg);

            var responseMsg = storedMsg as EndpointProxyTypesResponseMessage;
            Assert.AreElementsEqual(
                new List<ISerializedType> { ProxyExtensions.FromType(typeof(IMockNotificationSetWithTypedEventHandler)) },
                responseMsg.ProxyTypes,
                (x, y) => x.Equals(y));
        }

        [Test]
        public void InvokeWithFailingResponse()
        {
            var actionObject = new Mock<IMockNotificationSetWithTypedEventHandler>();
            var notificationSets = new List<KeyValuePair<Type, INotificationSet>> 
                { 
                    new KeyValuePair<Type, INotificationSet>(typeof(IMockNotificationSetWithTypedEventHandler), actionObject.Object)
                };

            var endpoint = new EndpointId("id");

            int count = 0;
            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
            {
                count++;
                if (count <= 1)
                {
                    throw new Exception();
                }
                else
                {
                    storedMsg = m;
                }
            };
            var collection = new Mock<INotificationSendersCollection>();
            {
                collection.Setup(c => c.GetEnumerator())
                    .Returns(notificationSets.GetEnumerator());
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new NotificationInformationRequestProcessAction(endpoint, sendAction, collection.Object, systemDiagnostics);
            action.Invoke(new NotificationInformationRequestMessage(new EndpointId("otherId")));

            Assert.AreEqual(2, count);
            Assert.IsInstanceOfType(typeof(FailureMessage), storedMsg);
        }

        [Test]
        public void InvokeWithFailedChannel()
        {
            var actionObject = new Mock<IMockNotificationSetWithTypedEventHandler>();
            var notificationSets = new List<KeyValuePair<Type, INotificationSet>> 
                { 
                    new KeyValuePair<Type, INotificationSet>(typeof(IMockNotificationSetWithTypedEventHandler), actionObject.Object)
                };

            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { throw new Exception(); };
            var collection = new Mock<INotificationSendersCollection>();
            {
                collection.Setup(c => c.GetEnumerator())
                    .Returns(notificationSets.GetEnumerator());
            }

            int count = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { count++; }, null);

            var action = new NotificationInformationRequestProcessAction(endpoint, sendAction, collection.Object, systemDiagnostics);
            action.Invoke(new NotificationInformationRequestMessage(new EndpointId("otherId")));

            Assert.AreEqual(2, count);
        }
    }
}
