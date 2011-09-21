//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
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
        private static string MethodInfoToString(IEnumerable<MethodInfo> invalidMethods)
        {
            var methodsText = new StringBuilder();
            foreach (var methodInfo in invalidMethods)
            {
                if (methodsText.Length > 0)
                {
                    methodsText.Append("; ");
                }

                var parametersText = new StringBuilder();
                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    if (parametersText.Length > 0)
                    {
                        parametersText.Append(", ");
                    }

                    parametersText.Append(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}{1}{2} {3}",
                            parameterInfo.IsOut ? "out " : string.Empty,
                            parameterInfo.IsRetval ? "ref " : string.Empty,
                            parameterInfo.ParameterType.Name,
                            parameterInfo.Name));
                }

                methodsText.Append(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}({2})",
                        methodInfo.DeclaringType.Name,
                        methodInfo.Name,
                        parametersText.ToString()));
            }

            return methodsText.ToString();
        }

        private static string PropertyInfoToString(IEnumerable<PropertyInfo> invalidProperties)
        {
            var propertiesText = new StringBuilder();
            foreach (var propertyInfo in invalidProperties)
            {
                if (propertiesText.Length > 0)
                {
                    propertiesText.Append("; ");
                }

                propertiesText.Append(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        propertyInfo.DeclaringType.Name,
                        propertyInfo.Name));
            }

            return propertiesText.ToString();
        }

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
        /// <param name="notificationSet">The type that has implemented the command set interface.</param>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     If the given type is not a valid <see cref="ICommandSet"/> interface.
        /// </exception>
        public static void VerifyThatTypeIsACorrectNotificationSet(Type notificationSet)
        {
            if (!typeof(INotificationSet).IsAssignableFrom(notificationSet))
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeIsNotAnINotificationSet,
                        notificationSet));
            }

            if (!notificationSet.IsInterface)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeIsNotAnInterface,
                        notificationSet));
            }

            if (notificationSet.ContainsGenericParameters)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeMustBeClosedConstructed,
                        notificationSet));
            }

            if (notificationSet.GetProperties().Length > 0)
            {
                var propertiesText = PropertyInfoToString(notificationSet.GetProperties());
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetCannotHaveProperties,
                        notificationSet,
                        propertiesText));
            }

            // Event methods get to stay, anything else is evil ...
            var invalidMethods = from methodInfo in notificationSet.GetMethods()
                                 where (!methodInfo.Name.StartsWith("add_") 
                                        && !methodInfo.Name.StartsWith("remove_"))
                                 select methodInfo;
            if (invalidMethods.Any())
            {
                var methodsText = MethodInfoToString(invalidMethods);
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetCannotHaveMethods,
                        notificationSet,
                        methodsText));
            }

            var events = notificationSet.GetEvents();
            if (events.Length == 0)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetMustHaveEvents,
                        notificationSet));
            }

            foreach (var eventInfo in events)
            {
                if (!HasCorrectDelegateType(eventInfo.EventHandlerType))
                {
                    throw new TypeIsNotAValidNotificationSetException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetEventsMustUseEventHandler,
                            notificationSet,
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
        /// The function used to write log messages.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationProxyBuilder"/> class.
        /// </summary>
        /// <param name="logger">The function that is used to log messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public NotificationProxyBuilder(Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => logger);
            }

            m_Logger = logger;
        }

        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The interface of the commandset for which a proxy must be made.</typeparam>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public T ProxyConnectingTo<T>() where T : INotificationSet
        {
            object result = ProxyConnectingTo(typeof(T));
            return (T)result;
        }

        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <param name="interfaceType">The interface of the commandset for which a proxy must be made.</param>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public INotificationSet ProxyConnectingTo(Type interfaceType)
        {
            {
                Enforce.Argument(() => interfaceType);
                Enforce.With<ArgumentException>(
                    typeof(INotificationSet).IsAssignableFrom(interfaceType), 
                    Resources.Exceptions_Messages_ANotificationSetTypeMustDeriveFromINotificationSet);
            }

            // We assume that the interface lives up to the demands we placed on it, i.e.:
            // - Derives from INotificationSet
            // - Has only events, no properties and no methods
            // - Every event is based on either the EventHandler or the EventHandler<T> delegate.
            // All these checks should have been done when the interface was registered
            // at the remote endpoint.
            var selfReference = new ProxySelfReferenceInterceptor();
            var addEventHandler = new NotificationEventAddMethodInterceptor(m_Logger);
            var removeEventHandler = new NotificationEventRemoveMethodInterceptor(m_Logger);

            var options = new ProxyGenerationOptions
                {
                    Selector = new NotificationSetInterceptorSelector(),
                    BaseTypeForInterfaceProxy = typeof(NotificationSetProxy),
                };

            var proxy = m_Generator.CreateInterfaceProxyWithoutTarget(
                interfaceType,
                options,
                new IInterceptor[] { selfReference, addEventHandler, removeEventHandler });

            return (INotificationSet)proxy;
        }
    }
}
