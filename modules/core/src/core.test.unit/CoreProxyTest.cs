//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Messaging;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Test.Unit
{
    [TestFixture]
    [Description("Tests the CoreProxy class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CoreProxyTest
    {
        [Test]
        [Description("Checks that the object returns the correct names for services that should be available.")]
        public void ServicesToBeAvailable()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<IHelpMessageProcessing>().Object);
            Assert.AreEqual(0, service.ServicesToBeAvailable().Count());
        }

        [Test]
        [Description("Checks that the object returns the correct names for services to which it should be connected.")]
        public void ServicesToConnectTo()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<IHelpMessageProcessing>().Object);
            Assert.AreElementsEqual(new[] { typeof(IMessagePipeline) }, service.ServicesToConnectTo());
        }

        [Test]
        [Description("Checks that the object can be connected to the dependencies.")]
        public void ConnectTo()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<IHelpMessageProcessing>().Object);
            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var pipeline = new MessagePipeline();
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from an unknown dependency.")]
        public void DisconnectFromWithNonMatchingServiceType()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<IHelpMessageProcessing>().Object);
            var pipeline = new MessagePipeline();
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new Mock<KernelService>().Object);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from a non-matching reference.")]
        public void DisconnectFromWithNonMatchingObjectReference()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<IHelpMessageProcessing>().Object);
            var pipeline = new MessagePipeline();
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MessagePipeline());
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object can be disconnected from the dependencies.")]
        public void DisconnectFrom()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<IHelpMessageProcessing>().Object);
            var pipeline = new MessagePipeline();
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);

            service.DisconnectFrom(pipeline);
            Assert.IsFalse(service.IsConnectedToAllDependencies);
        }
    }
}
