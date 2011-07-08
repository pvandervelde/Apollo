﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Utilities;
using MbUnit.Framework;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation. Especially not in partial classes.")]
    public sealed partial class CommandProxyBuilderTest
    {
        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonAssignableType()
        { 
            Assert.Throws<TypeIsNotAValidCommandSetException>(() => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(object)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonInterface()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(MockCommandSetNotAnInterface)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithGenericInterface()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithGenericParameters<>)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithProperties()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithProperties)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithAdditionalEvents()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithEvents)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithoutMethods()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithoutMethods)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithGenericMethod()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithGenericMethod)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithIncorrectReturnType()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithIncorrectReturnType)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonSerializableReturnType()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithNonSerializableReturnType)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithOutParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithOutParameter)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithRefParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithRefParameter)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonSerializableParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypetIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithNonSerializableParameter)));
        }

        [Test]
        public void ProxyConnectingToMethodWithTaskReturnWithSuccessFullExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
                {
                    intermediateMsg = m as CommandInvokedMessage;
                    return Task<ICommunicationMessage>.Factory.StartNew(
                        () => new SuccessMessage(remoteEndpoint, new MessageId()),
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var builder = new CommandProxyBuilder(local, sender, logger);
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
        public void ProxyConnectingToMethodWithTaskReturnWithFailedExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
            {
                intermediateMsg = m as CommandInvokedMessage;
                return Task<ICommunicationMessage>.Factory.StartNew(
                    () => new FailureMessage(remoteEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            };

            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var builder = new CommandProxyBuilder(local, sender, logger);
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
        public void ProxyConnectingToMethodWithTypedTaskReturnWithSuccessfullExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
            {
                intermediateMsg = m as CommandInvokedMessage;
                return Task<ICommunicationMessage>.Factory.StartNew(
                    () => new CommandInvokedResponseMessage(remoteEndpoint, new MessageId(), 20),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            };

            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var builder = new CommandProxyBuilder(local, sender, logger);
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
        public void ProxyConnectingToMethodWithTypedTaskReturnWithFailedExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
            {
                intermediateMsg = m as CommandInvokedMessage;
                return Task<ICommunicationMessage>.Factory.StartNew(
                    () => new FailureMessage(remoteEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            };

            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var builder = new CommandProxyBuilder(local, sender, logger);
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
