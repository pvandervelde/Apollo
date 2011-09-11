//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Forms the base for remote <see cref="INotificationSet"/> proxy objects.
    /// </summary>
    /// <remarks>
    /// This type is not really meant to be used except by the DynamicProxy2 framework, hence
    /// it should be an open type which is not abstract.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    internal class NotificationSetProxy : INotificationSet
    {
        /// <summary>
        /// Gets method that will be invoked the event is raised.
        /// </summary>
        /// <param name="obj">Object that contains the event.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <returns>
        /// The method that is used to invoke the event.
        /// </returns>
        private static MethodInfo GetEventInvoker(object obj, string eventName)
        {
            {
                Debug.Assert(obj != null, "The input object should not be null.");
                Debug.Assert(!string.IsNullOrEmpty(eventName), "The event name should not be null or empty.");
            }

            // prepare current processing type
            var currentType = obj.GetType();

            // try to get special event decleration
            while (true)
            {
                var fieldInfo = currentType.GetField(
                    eventName, 
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField);
                if (fieldInfo == null)
                {
                    if (currentType.BaseType != null)
                    {
                        // move deeper
                        currentType = currentType.BaseType;
                        continue;
                    }

                    Debug.Fail(string.Format("Not found event named {0} in object type {1}", eventName, obj));
                    return null;
                }

                return ((MulticastDelegate)fieldInfo.GetValue(obj)).Method;
            }
        }

        /// <summary>
        /// Returns the reference to the 'current' object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is created so that dynamic proxies can override the method and return a 
        /// reference to the proxy. This should prevent the 'leaking' of the this reference to the
        /// outside world. For more information see:
        /// http://kozmic.pl/2009/10/30/castle-dynamic-proxy-tutorial-part-xv-patterns-and-antipatterns
        /// </para>
        /// <para>
        /// Note that this method is <c>protected internal</c> so that the <see cref="CommandSetInterceptorSelector"/>
        /// can get access to the method through an expression tree.
        /// </para>
        /// </remarks>
        /// <returns>
        /// The current object.
        /// </returns>
        protected internal virtual object SelfReference()
        {
            return this;
        }

        /// <summary>
        /// Raises the event given by the <paramref name="eventName"/> parameter.
        /// </summary>
        /// <param name="eventName">The name of the event that should be raised.</param>
        /// <param name="args">The event arguments with which the event should be raised.</param>
        protected internal void RaiseEvent(string eventName, EventArgs args)
        {
            var obj = SelfReference();
            var eventInvoker = GetEventInvoker(obj, eventName);
            eventInvoker.Invoke(obj, new object[] { obj, args });
        }
    }
}
