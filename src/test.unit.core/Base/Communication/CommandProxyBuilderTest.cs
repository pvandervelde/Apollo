//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using MbUnit.Framework;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the ChannelClosedEventArgs class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation. Especially not in partial classes.")]
    public sealed partial class CommandProxyBuilderTest
    {
        [Test]
        [Description("Checks that the type must be assignable from ICommandSet.")]
        public void VerifyThatTypeIsACorrectCommandSetWithNonAssignableType()
        { 
            Assert.Throws<TypeIsNotAValidCommandSetException>(() => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(object)));
        }

        [Test]
        [Description("Checks that the type must be an interface.")]
        public void VerifyThatTypeIsACorrectCommandSetWithNonInterface()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(MockCommandSetNotAnInterface)));
        }

        [Test]
        [Description("Checks that the type cannot be an open generic type.")]
        public void VerifyThatTypeIsACorrectCommandSetWithGenericInterface()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithGenericParameters<>)));
        }

        [Test]
        [Description("Checks that the type cannot have properties.")]
        public void VerifyThatTypeIsACorrectCommandSetWithProperties()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithProperties)));
        }

        [Test]
        [Description("Checks that the type cannot have additional events.")]
        public void VerifyThatTypeIsACorrectCommandSetWithAdditionalEvents()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithEvents)));
        }

        [Test]
        [Description("Checks that the type should have at least one method.")]
        public void VerifyThatTypeIsACorrectCommandSetWithoutMethods()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithoutMethods)));
        }

        [Test]
        [Description("Checks that the type cannot have any open generic methods.")]
        public void VerifyThatTypeIsACorrectCommandSetWithGenericMethod()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithGenericMethod)));
        }

        [Test]
        [Description("Checks that the type cannot have methods with an incorrect return type.")]
        public void VerifyThatTypeIsACorrectCommandSetWithIncorrectReturnType()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithIncorrectReturnType)));
        }

        [Test]
        [Description("Checks that the type cannot have methods with a non-serializable return type.")]
        public void VerifyThatTypeIsACorrectCommandSetWithNonSerializableReturnType()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithNonSerializableReturnType)));
        }

        [Test]
        [Description("Checks that the type cannot have methods with an out parameter.")]
        public void VerifyThatTypeIsACorrectCommandSetWithOutParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithOutParameter)));
        }

        [Test]
        [Description("Checks that the type cannot have methods with an ref parameter.")]
        public void VerifyThatTypeIsACorrectCommandSetWithRefParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithRefParameter)));
        }

        [Test]
        [Description("Checks that the type cannot have methods with a non-serializable parameter.")]
        public void VerifyThatTypeIsACorrectCommandSetWithNonSerializableParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithNonSerializableParameter)));
        }

        [Test]
        [Description("Checks that a proxy returning a Task can report on the success of the remote operation.")]
        public void ProxyConnectingToMethodWithTaskReturnWithSuccessFullExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
                {
                    intermediateMsg = m as CommandInvokedMessage;
                    return Task<ICommunicationMessage>.Factory.StartNew(() => new SuccessMessage(remoteEndpoint, new MessageId()));
                };

            var builder = new CommandProxyBuilder(local, sender);
            var proxy = builder.ProxyConnectingTo<IMockCommandSetWithTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            result.Wait();

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsFalse(result.IsFaulted);

            Assert.AreEqual(CommandSetProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn)), intermediateMsg.Invocation.CommandSet);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Count);
            Assert.AreEqual(CommandSetProxyExtensions.FromType(typeof(int)), intermediateMsg.Invocation.Parameters[0].Item1);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Item2);
        }

        [Test]
        [Description("Checks that a proxy returning a Task can report on the failure of the remote operation.")]
        public void ProxyConnectingToMethodWithTaskReturnWithFailedExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
            {
                intermediateMsg = m as CommandInvokedMessage;
                return Task<ICommunicationMessage>.Factory.StartNew(() => new FailureMessage(remoteEndpoint, new MessageId()));
            };

            var builder = new CommandProxyBuilder(local, sender);
            var proxy = builder.ProxyConnectingTo<IMockCommandSetWithTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            Assert.Throws<AggregateException>(() => result.Wait());

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsTrue(result.IsFaulted);
            Assert.IsAssignableFrom(typeof(CommandInvocationFailedException), ((AggregateException)result.Exception).InnerExceptions[0].GetType());

            Assert.AreEqual(CommandSetProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn)), intermediateMsg.Invocation.CommandSet);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Count);
            Assert.AreEqual(CommandSetProxyExtensions.FromType(typeof(int)), intermediateMsg.Invocation.Parameters[0].Item1);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Item2);
        }

        [Test]
        [Description("Checks that a proxy returning a Task<T> can report on the success of the remote operation.")]
        public void ProxyConnectingToMethodWithTypedTaskReturnWithSuccessfullExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
            {
                intermediateMsg = m as CommandInvokedMessage;
                return Task<ICommunicationMessage>.Factory.StartNew(() => new CommandInvokedResponseMessage(remoteEndpoint, new MessageId(), 20));
            };

            var builder = new CommandProxyBuilder(local, sender);
            var proxy = builder.ProxyConnectingTo<IMockCommandSetWithTypedTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            result.Wait();

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsFalse(result.IsFaulted);
            Assert.AreEqual(20, result.Result);

            Assert.AreEqual(CommandSetProxyExtensions.FromType(typeof(IMockCommandSetWithTypedTaskReturn)), intermediateMsg.Invocation.CommandSet);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Count);
            Assert.AreEqual(CommandSetProxyExtensions.FromType(typeof(int)), intermediateMsg.Invocation.Parameters[0].Item1);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Item2);
        }

        [Test]
        [Description("Checks that a proxy returning a Task<T> can report on the failure of the remote operation.")]
        public void ProxyConnectingToMethodWithTypedTaskReturnWithFailedExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
            {
                intermediateMsg = m as CommandInvokedMessage;
                return Task<ICommunicationMessage>.Factory.StartNew(() => new FailureMessage(remoteEndpoint, new MessageId()));
            };

            var builder = new CommandProxyBuilder(local, sender);
            var proxy = builder.ProxyConnectingTo<IMockCommandSetWithTypedTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            Assert.Throws<AggregateException>(() => result.Wait());

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsTrue(result.IsFaulted);
            Assert.IsAssignableFrom(typeof(CommandInvocationFailedException), ((AggregateException)result.Exception).InnerExceptions[0].GetType());

            Assert.AreEqual(CommandSetProxyExtensions.FromType(typeof(IMockCommandSetWithTypedTaskReturn)), intermediateMsg.Invocation.CommandSet);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Count);
            Assert.AreEqual(CommandSetProxyExtensions.FromType(typeof(int)), intermediateMsg.Invocation.Parameters[0].Item1);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Item2);
        }
    }
}
