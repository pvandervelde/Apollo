//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Utilities;
using Castle.DynamicProxy;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines an <see cref="IInterceptor"/> for the 'remove' method of an <see cref="INotificationSet"/> event.
    /// </summary>
    internal sealed class NotificationEventRemoveMethodInterceptor : IInterceptor
    {
        /// <summary>
        /// The prefix for the method that removes event handlers from the event.
        /// </summary>
        private const string MethodPrefix = "remove_";

        private static string MethodToText(MethodInfo method)
        {
            return method.ToString();
        }

        /// <summary>
        /// The type of the interface that is being proxied.
        /// </summary>
        private readonly Type m_InterfaceType;

        /// <summary>
        /// The function which sends the <see cref="RegisterForNotificationMessage"/> to the owning endpoint.
        /// </summary>
        private readonly Action<ISerializedEventRegistration> m_SendMessageWithoutResponse;

        /// <summary>
        /// The function used to write log messages.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventRemoveMethodInterceptor"/> class.
        /// </summary>
        /// <param name="proxyInterfaceType">The type of the interface that is being proxied.</param>
        /// <param name="sendMessageWithoutResponse">
        ///     The function used to send the information about the event registration to the owning endpoint.
        /// </param>
        /// <param name="logger">The function that is used to log messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="proxyInterfaceType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessageWithoutResponse"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public NotificationEventRemoveMethodInterceptor(
            Type proxyInterfaceType,
            Action<ISerializedEventRegistration> sendMessageWithoutResponse,
            Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => proxyInterfaceType);
                Enforce.Argument(() => sendMessageWithoutResponse);
                Enforce.Argument(() => logger);
            }

            m_InterfaceType = proxyInterfaceType;
            m_SendMessageWithoutResponse = sendMessageWithoutResponse;
            m_Logger = logger;
        }

        /// <summary>
        /// Called when a method or property call is intercepted.
        /// </summary>
        /// <param name="invocation">Information about the call that was intercepted.</param>
        public void Intercept(IInvocation invocation)
        {
            {
                Debug.Assert(invocation.Method.Name.StartsWith(MethodPrefix), "Intercepted an incorrect method.");
                Debug.Assert(invocation.Arguments.Length == 1, "There should only be one argument.");
                Debug.Assert(invocation.Arguments[0] is Delegate, "The argument should be a delegate.");
            }

            m_Logger(
                LogSeverityProxy.Trace,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Invoking {0}",
                    MethodToText(invocation.Method)));
            
            var methodToInvoke = invocation.Method.Name;
            var eventName = methodToInvoke.Substring(MethodPrefix.Length);

            var handler = invocation.Arguments[0] as Delegate;
            var proxy = invocation.Proxy as NotificationSetProxy;
            proxy.RemoveFromEvent(eventName, handler);

            if (!proxy.HasSubscribers(eventName))
            {
                m_SendMessageWithoutResponse(new SerializedEvent(ProxyExtensions.FromType(m_InterfaceType), eventName));
            }
        }
    }
}
