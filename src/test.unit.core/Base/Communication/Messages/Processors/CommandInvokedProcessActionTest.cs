//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Communication.Messages.Processors;
using Apollo.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication.Messages.Processors
{
    [TestFixture]
    [Description("Tests the CommandInvokedProcessAction class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class CommandInvokedProcessActionTest
    {
        // A fake command set interface to invoke methods on
        public interface IMockCommandSet : ICommandSet
        {
            Task MethodWithoutReturnValue(int someNumber);

            Task<int> MethodWithReturnValue(int someNumber);
        }

        [Test]
        [Description("Checks that the object processes the correct message type.")]
        public void MessageTypeToProcess()
        {
            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { };
            var commands = new Mock<ICommandCollection>();
            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, logger);
            Assert.AreEqual(typeof(CommandInvokedMessage), action.MessageTypeToProcess);
        }

        [Test]
        [Description("Checks that the message is processed correctly if there is a Task return value.")]
        public void InvokeWithTaskReturn()
        {
            var actionObject = new Mock<IMockCommandSet>();
            {
                actionObject.Setup(a => a.MethodWithoutReturnValue(It.IsAny<int>()))
                    .Returns(Task.Factory.StartNew(() => { }))
                    .Verifiable();
            }

            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(IMockCommandSet), actionObject.Object)
                };

            var endpoint = new EndpointId("id");

            EndpointId storedEndpoint = null;
            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
                {
                    storedEndpoint = e;
                    storedMsg = m;
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandsFor(It.IsAny<Type>()))
                    .Returns(commandSets[0].Value);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, logger);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    CommandSetProxyExtensions.FromMethodInfo(typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue"), new object[] { 1 })));

            actionObject.Verify(a => a.MethodWithoutReturnValue(It.IsAny<int>()), Times.Once());
            Assert.IsInstanceOfType(typeof(SuccessMessage), storedMsg);
        }

        [Test]
        [Description("Checks that the message is processed correctly if there is a Task<T> method return value.")]
        public void InvokeWithTypedTaskReturn()
        {
            var actionObject = new Mock<IMockCommandSet>();
            {
                actionObject.Setup(a => a.MethodWithReturnValue(It.IsAny<int>()))
                    .Returns(() => Task<int>.Factory.StartNew(() => 1))
                    .Verifiable();
            }

            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(IMockCommandSet), actionObject.Object)
                };

            var endpoint = new EndpointId("id");

            AutoResetEvent resetEvent = new AutoResetEvent(false);
            EndpointId storedEndpoint = null;
            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
                {
                    storedEndpoint = e;
                    storedMsg = m;
                    resetEvent.Set();
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandsFor(It.IsAny<Type>()))
                    .Returns(commandSets[0].Value);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, logger);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    CommandSetProxyExtensions.FromMethodInfo(typeof(IMockCommandSet).GetMethod("MethodWithReturnValue"), new object[] { 2 })));

            actionObject.Verify(a => a.MethodWithReturnValue(It.IsAny<int>()), Times.Once());

            // For some reason the processing of a task with a return value takes a non-trivial amount
            // of time, so we put in a wait event and wait for it to finish.
            resetEvent.WaitOne();
            Assert.IsInstanceOfType(typeof(CommandInvokedResponseMessage), storedMsg);
            
            var responseMsg = storedMsg as CommandInvokedResponseMessage;
            Assert.IsInstanceOfType(typeof(int), responseMsg.Result);
            Assert.AreEqual(1, (int)responseMsg.Result);
        }

        [Test]
        [Description("Checks that the message is processed correctly.")]
        public void InvokeWithFailingResponse()
        {
            var actionObject = new Mock<IMockCommandSet>();
            {
                actionObject.Setup(a => a.MethodWithoutReturnValue(It.IsAny<int>()))
                    .Returns(Task.Factory.StartNew(() => { }))
                    .Verifiable();
            }

            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(IMockCommandSet), actionObject.Object)
                };

            var endpoint = new EndpointId("id");

            AutoResetEvent resetEvent = new AutoResetEvent(false);
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
                        resetEvent.Set();
                    }
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandsFor(It.IsAny<Type>()))
                    .Returns(commandSets[0].Value);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, logger);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    CommandSetProxyExtensions.FromMethodInfo(typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue"), new object[] { 1 })));

            actionObject.Verify(a => a.MethodWithoutReturnValue(It.IsAny<int>()), Times.Once());

            // For some reason the processing of a task with a return value takes a non-trivial amount
            // of time, so we put in a wait event and wait for it to finish.
            resetEvent.WaitOne();
            Assert.AreEqual(2, count);
            Assert.IsInstanceOfType(typeof(FailureMessage), storedMsg);
        }

        [Test]
        [Description("Checks that the message is processed correctly.")]
        public void InvokeWithFailedChannel()
        {
            var actionObject = new Mock<IMockCommandSet>();
            {
                actionObject.Setup(a => a.MethodWithoutReturnValue(It.IsAny<int>()))
                    .Returns(Task.Factory.StartNew(() => { }))
                    .Verifiable();
            }

            var commandSets = new List<KeyValuePair<Type, ICommandSet>> 
                { 
                    new KeyValuePair<Type, ICommandSet>(typeof(IMockCommandSet), actionObject.Object)
                };

            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { throw new Exception(); };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandsFor(It.IsAny<Type>()))
                    .Returns(commandSets[0].Value);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            int count = 0;
            Action<LogSeverityProxy, string> logger = (p, t) => { count++; };

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, logger);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    CommandSetProxyExtensions.FromMethodInfo(typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue"), new object[] { 1 })));

            // This is obviously pure evil but we need to wait for the tasks that get created by the Invoke method
            // Unfortunately we can't get to those tasks so we'll have to sleep the thread.
            // And because we are throwing exceptions we can't really define a good place to put a reset event either :(
            Thread.Sleep(100);

            Assert.AreEqual(2, count);
            actionObject.Verify(a => a.MethodWithoutReturnValue(It.IsAny<int>()), Times.Once());
        }
    }
}
