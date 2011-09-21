//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;
using MbUnit.Framework;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation. Especially not in partial classes.")]
    public sealed partial class NotificationProxyBuilderTest
    {
        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonAssignableType()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(object)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonInterface()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(MockNotificationSetNotAnInterface)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithGenericInterface()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithGenericParameters<>)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithProperties()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithProperties)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithMethods()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithMethods)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithoutEvents()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithoutEvents)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonEventHandlerEvent()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithNonEventHandlerEvent)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonSerializableEventArgs()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithNonSerializableEventArgs)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSet()
        {
            Assert.DoesNotThrow(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithEventHandler)));
            Assert.DoesNotThrow(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithTypedEventHandler)));
        }
        
        [Test]
        public void ProxyConnectingToEventWithNormalEventHandler()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            var builder = new NotificationProxyBuilder(logger);
            var proxy = builder.ProxyConnectingTo<IMockNotificationSetWithEventHandler>();

            object sender = null;
            EventArgs receivedArgs = null;
            proxy.OnMyEvent += 
                (s, e) => 
                {
                    sender = s;
                    receivedArgs = e;
                };

            var notificationObj = proxy as NotificationSetProxy;
            Assert.IsNotNull(notificationObj);

            var args = new EventArgs();
            notificationObj.RaiseEvent("OnMyEvent", args);

            Assert.AreSame(proxy, sender);
            Assert.AreSame(args, receivedArgs);
        }

        [Test]
        public void ProxyConnectingToEventWithTypedEventHandler()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            var builder = new NotificationProxyBuilder(logger);
            var proxy = builder.ProxyConnectingTo<IMockNotificationSetWithTypedEventHandler>();

            object sender = null;
            EventArgs receivedArgs = null;
            proxy.OnMyEvent +=
                (s, e) =>
                {
                    sender = s;
                    receivedArgs = e;
                };

            var notificationObj = proxy as NotificationSetProxy;
            Assert.IsNotNull(notificationObj);

            var args = new MySerializableEventArgs();
            notificationObj.RaiseEvent("OnMyEvent", args);

            Assert.AreSame(proxy, sender);
            Assert.AreSame(args, receivedArgs);
        }

        [Test]
        public void ProxyDisconnectFromEventWithNormalEventHandler()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            var builder = new NotificationProxyBuilder(logger);
            var proxy = builder.ProxyConnectingTo<IMockNotificationSetWithEventHandler>();

            object sender = null;
            EventArgs receivedArgs = null;

            EventHandler handler = 
                (s, e) =>
                {
                    sender = s;
                    receivedArgs = e;
                };
            proxy.OnMyEvent += handler;

            var notificationObj = proxy as NotificationSetProxy;
            Assert.IsNotNull(notificationObj);

            var args = new EventArgs();
            notificationObj.RaiseEvent("OnMyEvent", args);

            Assert.AreSame(proxy, sender);
            Assert.AreSame(args, receivedArgs);

            sender = null;
            receivedArgs = null;
            proxy.OnMyEvent -= handler;

            notificationObj.RaiseEvent("OnMyEvent", new EventArgs());
            Assert.IsNull(sender);
            Assert.IsNull(receivedArgs);
        }

        [Test]
        public void ProxyCleanupAttachedEvents()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            var builder = new NotificationProxyBuilder(logger);
            var proxy = builder.ProxyConnectingTo<IMockNotificationSetWithEventHandler>();

            object sender = null;
            EventArgs receivedArgs = null;
            proxy.OnMyEvent +=
                (s, e) =>
                {
                    sender = s;
                    receivedArgs = e;
                };

            var notificationObj = proxy as NotificationSetProxy;
            Assert.IsNotNull(notificationObj);

            var args = new EventArgs();
            notificationObj.RaiseEvent("OnMyEvent", args);

            Assert.AreSame(proxy, sender);
            Assert.AreSame(args, receivedArgs);

            sender = null;
            receivedArgs = null;
            notificationObj.ClearAllEvents();

            notificationObj.RaiseEvent("OnMyEvent", new EventArgs());
            Assert.IsNull(sender);
            Assert.IsNull(receivedArgs);
        }
    }
}
