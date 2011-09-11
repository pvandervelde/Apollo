//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Apollo.Core.Base.Properties;
using Apollo.Utilities;
using Castle.DynamicProxy;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Builds proxy objects for the <see cref="RemoteNotificationHub"/>.
    /// </summary>
    internal sealed class NotificationProxyBuilder
    {
        /// <summary>
        /// Verifies that an interface type will be a correct command set.
        /// </summary>
        /// <remarks>
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
        /// </remarks>
        /// <param name="commandSet">The type that has implemented the command set interface.</param>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     If the given type is not a valid <see cref="ICommandSet"/> interface.
        /// </exception>
        public static void VerifyThatTypetIsACorrectCommandSet(Type commandSet)
        {
            if (!typeof(INotificationSet).IsAssignableFrom(commandSet))
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeIsNotAnINotificationSet,
                        commandSet));
            }

            if (!commandSet.IsInterface)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeIsNotAnInterface,
                        commandSet));
            }

            if (commandSet.ContainsGenericParameters)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeMustBeClosedConstructed,
                        commandSet));
            }

            if (commandSet.GetProperties().Length > 0)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetCannotHaveProperties,
                        commandSet));
            }

            if (commandSet.GetMethods().Length > 0)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetCannotHaveMethods,
                        commandSet));
            }

            var events = commandSet.GetEvents();
            if (events.Length == 0)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetMustHaveEvents,
                        commandSet));
            }

            foreach (var eventInfo in events)
            {
                if (!HasCorrectDelegateType(eventInfo.EventHandlerType))
                {
                    throw new TypeIsNotAValidNotificationSetException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetEventsMustUseEventHandler,
                            commandSet,
                            eventInfo));
                }
            }
        }

        private static bool HasCorrectDelegateType(Type type)
        {
            if (type.Equals(typeof(EventHandler)))
            {
                return true;
            }

            if (type.IsGenericType)
            {
                var baseType = type.GetGenericTypeDefinition();
                if (baseType.Equals(typeof(EventHandler<>)))
                {
                    var genericParameters = type.GetGenericArguments();
                    if (genericParameters[0].ContainsGenericParameters)
                    {
                        return false;
                    }

                    if (IsTypeSerializable(genericParameters[0]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsTypeSerializable(Type type)
        {
            return Attribute.IsDefined(type, typeof(DataContractAttribute)) || typeof(ISerializable).IsAssignableFrom(type) || type.IsSerializable;
        }

        /// <summary>
        /// The generator that will create the proxy objects.
        /// </summary>
        private readonly ProxyGenerator m_Generator = new ProxyGenerator();

        /// <summary>
        /// The ID of the local endpoint.
        /// </summary>
        private readonly EndpointId m_Local;

        /// <summary>
        /// The function which sends the message to the owning endpoint and returns a task that will,
        /// eventually, hold the return message.
        /// </summary>
        private readonly Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> m_SendWithResponse;

        /// <summary>
        /// The function used to write log messages.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationProxyBuilder"/> class.
        /// </summary>
        /// <param name="localEndpoint">The ID number of the local endpoint.</param>
        /// <param name="sendWithResponse">
        ///     The function that sends out a message to the given endpoint and returns a task that will, eventually, hold the return message.
        /// </param>
        /// <param name="logger">The function that is used to log messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendWithResponse"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public NotificationProxyBuilder(
            EndpointId localEndpoint,
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sendWithResponse,
            Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => localEndpoint);
                Enforce.Argument(() => sendWithResponse);
                Enforce.Argument(() => logger);
            }

            m_Local = localEndpoint;
            m_SendWithResponse = sendWithResponse;
            m_Logger = logger;
        }

        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The interface of the commandset for which a proxy must be made.</typeparam>
        /// <param name="endpoint">The endpoint for which a proxy must be made.</param>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public T ProxyConnectingTo<T>(EndpointId endpoint) where T : INotificationSet
        {
            object result = ProxyConnectingTo(endpoint, typeof(T));
            return (T)result;
        }

        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint for which a proxy must be made.</param>
        /// <param name="interfaceType">The interface of the commandset for which a proxy must be made.</param>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public object ProxyConnectingTo(EndpointId endpoint, Type interfaceType)
        {
            {
                Enforce.Argument(() => interfaceType);
                Enforce.With<ArgumentException>(
                    typeof(INotificationSet).IsAssignableFrom(interfaceType), 
                    Resources.Exceptions_Messages_ANotificationSetTypeMustDeriveFromINotificationSet);

                Enforce.Argument(() => endpoint);
            }

            // We assume that the interface lives up to the demands we placed on it, i.e.:
            // - Derives from INotificationSet
            // - Has only events, no properties and no methods
            // - Every event is based on either the EventHandler or the EventHandler<T> delegate.
            // All these checks should have been done when the interface was registered
            // at the remote endpoint.
            var selfReference = new ProxySelfReferenceInterceptor();
            var options = new ProxyGenerationOptions
                {
                    Selector = new NotificationSetInterceptorSelector(),
                    BaseTypeForInterfaceProxy = typeof(NotificationSetProxy),
                };

            var proxy = m_Generator.CreateInterfaceProxyWithoutTarget(
                interfaceType,
                options,
                new IInterceptor[] { selfReference });

            return proxy;
        }
    }
}
