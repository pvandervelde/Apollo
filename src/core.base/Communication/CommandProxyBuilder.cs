//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Properties;
using Castle.DynamicProxy;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Builds proxy objects for the <see cref="RemoteCommandHub"/>.
    /// </summary>
    internal sealed class CommandProxyBuilder
    {
        /// <summary>
        /// Verifies that an interface type will be a correct command set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A proper command set class has the following characteristics:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The interface must derrive from <see cref="ICommandSet"/>.</description>
        ///     </item>
        ///     <item>
        ///         <description>The interface must only have methods, no properties or events.</description>
        ///     </item>
        ///     <item>
        ///         <description>Each method must return either <see cref="Task"/> or <see cref="Task{T}"/>.</description>
        ///     </item>
        ///     <item>
        ///         <description>If a method returns a <see cref="Task{T}"/> then <c>T</c> must be a closed constructed type.</description>
        ///     </item>
        ///     <item>
        ///         <description>If a method returns a <see cref="Task{T}"/> then <c>T</c> must be serializable.</description>
        ///     </item>
        ///     <item>
        ///         <description>All method parameters must be serializable.</description>
        ///     </item>
        ///     <item>
        ///         <description>None of the method parameters may be <c>ref</c> or <c>out</c> parameters.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="commandSet">The object that has implemented the command set.</param>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     If the given type is not a valid <see cref="ICommandSet"/> interface.
        /// </exception>
        public static void VerifyThatObjectIsACorrectCommandSet(Type commandSet)
        {
            if (!typeof(ICommandSet).IsAssignableFrom(commandSet))
            {
                throw new TypeIsNotAValidCommandSetException(Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_TypeIsNotAnICommandSet);
            }

            if (!commandSet.IsInterface)
            {
                throw new TypeIsNotAValidCommandSetException(Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_TypeIsNotAnInterface);
            }

            if (commandSet.ContainsGenericParameters)
            {
                throw new TypeIsNotAValidCommandSetException(Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_TypeMustBeClosedConstructed);
            }

            if (commandSet.GetProperties().Length > 0)
            {
                throw new TypeIsNotAValidCommandSetException(Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetCannotHaveProperties);
            }

            if (commandSet.GetEvents().Length > typeof(ICommandSet).GetEvents().Length)
            {
                throw new TypeIsNotAValidCommandSetException(Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetCannotHaveEvents);
            }

            var methods = commandSet.GetMethods();
            foreach (var method in methods)
            {
                if (method.IsGenericMethodDefinition)
                {
                    throw new TypeIsNotAValidCommandSetException(Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMethodsCannotBeGeneric);
                }

                if (!HasCorrectReturnType(method.ReturnType))
                {
                    throw new TypeIsNotAValidCommandSetException(Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMethodsMustHaveCorrectReturnType);
                }

                var parameters = method.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (!IsParameterValid(parameter))
                    {
                        throw new TypeIsNotAValidCommandSetException(Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetParametersMustBeValid);
                    }
                }
            }
        }

        private static bool IsParameterValid(ParameterInfo parameter)
        {
            if (parameter.ParameterType.ContainsGenericParameters)
            {
                return false;
            }

            if (parameter.IsOut || parameter.ParameterType.IsByRef)
            {
                return false;
            }

            if (!IsTypeSerializable(parameter.ParameterType))
            {
                return false;
            }

            return true;
        }

        private static bool HasCorrectReturnType(Type type)
        {
            if (type.Equals(typeof(void)))
            {
                return true;
            }

            if (type.Equals(typeof(Task)))
            {
                return true;
            }

            if (type.IsGenericType)
            { 
                var baseType = type.GetGenericTypeDefinition();
                if (baseType.Equals(typeof(Task<>)))
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
        /// Initializes a new instance of the <see cref="CommandProxyBuilder"/> class.
        /// </summary>
        /// <param name="localEndpoint">The ID number of the local endpoint.</param>
        /// <param name="sendWithResponse">
        ///     The function that sends out a message to the given endpoint and returns a task that will, eventually, hold the return message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendWithResponse"/> is <see langword="null" />.
        /// </exception>
        public CommandProxyBuilder(
            EndpointId localEndpoint,
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sendWithResponse)
        {
            {
                Enforce.Argument(() => localEndpoint);
                Enforce.Argument(() => sendWithResponse);
            }

            m_Local = localEndpoint;
            m_SendWithResponse = sendWithResponse;
        }

        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The interface of the commandset for which a proxy must be made.</typeparam>
        /// <param name="endpoint">The endpoint for which a proxy must be made.</param>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public T ProxyConnectingTo<T>(EndpointId endpoint) where T : ICommandSet
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
                    typeof(ICommandSet).IsAssignableFrom(interfaceType), 
                    Resources.Exceptions_Messages_ACommandSetTypeMustDeriveFromICommandSet);

                Enforce.Argument(() => endpoint);
            }

            // We assume that the interface lives up to the demands we placed on it, i.e.:
            // - Derives from ICommandSet
            // - Has only methods, no properties and no events, other than those defined by
            //   ICommandSet
            // - Every method either returns nothing (void) or returns a Task<T> object.
            // All these checks should have been done when the interface was registered
            // at the remote endpoint.
            var baseObject = new CommandSetProxy();

            var selfReference = new CommandSetProxySelfReferenceInterceptor();
            var methodWithoutResult = new CommandSetMethodWithoutResultInterceptor(
                methodInvocation =>
                {
                    var msg = new CommandInvokedMessage(m_Local, methodInvocation);
                    return m_SendWithResponse(endpoint, msg);
                });
            var methodWithResult = new CommandSetMethodWithResultInterceptor(
                methodInvocation =>
                {
                    var msg = new CommandInvokedMessage(m_Local, methodInvocation);
                    return m_SendWithResponse(endpoint, msg);
                });

            var options = new ProxyGenerationOptions
                {
                    Selector = new CommandSetInterceptorSelector(),
                };

            var proxy = m_Generator.CreateInterfaceProxyWithTarget(
                interfaceType,
                baseObject,
                options,
                new IInterceptor[] { selfReference, methodWithoutResult, methodWithResult });

            return proxy;
        }
    }
}
