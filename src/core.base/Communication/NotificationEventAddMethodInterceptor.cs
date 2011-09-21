//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Apollo.Utilities;
using Castle.DynamicProxy;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines an <see cref="IInterceptor"/> for the 'add' method of an <see cref="INotificationSet"/> event.
    /// </summary>
    internal sealed class NotificationEventAddMethodInterceptor : IInterceptor
    {
        /// <summary>
        /// The prefix for the method that removes event handlers from the event.
        /// </summary>
        private const string MethodPrefix = "add_";

        private static string MethodToText(MethodInfo method)
        {
            return method.ToString();
        }

        /// <summary>
        /// The function used to write log messages.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventAddMethodInterceptor"/> class.
        /// </summary>
        /// <param name="logger">The function that is used to log messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public NotificationEventAddMethodInterceptor(Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => logger);
            }

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

            proxy.AddToEvent(eventName, handler);
        }
    }
}
