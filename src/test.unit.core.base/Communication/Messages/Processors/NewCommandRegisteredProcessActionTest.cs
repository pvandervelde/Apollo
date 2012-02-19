﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class NewCommandRegisteredProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var registrator = new Mock<IAcceptExternalProxyInformation>();
            var action = new NewCommandRegisteredProcessAction(registrator.Object);
            Assert.AreEqual(typeof(NewCommandRegisteredMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            var registrator = new Mock<IAcceptExternalProxyInformation>();
            {
                registrator.Setup(r => r.RecentlyRegisteredProxy(It.IsAny<EndpointId>(), It.IsAny<ISerializedType>()))
                    .Verifiable();
            }

            var action = new NewCommandRegisteredProcessAction(registrator.Object);

            var msg = new NewCommandRegisteredMessage(
                EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                typeof(LocalCommandCollectionTest.IMockCommandSetWithTaskReturn));
            action.Invoke(msg);

            registrator.Verify(r => r.RecentlyRegisteredProxy(It.IsAny<EndpointId>(), It.IsAny<ISerializedType>()), Times.Once());
        }
    }
}