//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Messaging;
using Apollo.Utils.Commands;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the CoreProxy class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CoreProxyTest
    {
        #region Internal class - MockIKernel

        /// <summary>
        /// Defines a mock implementation of the <see cref="IKernel"/> interface.
        /// </summary>
        /// <design>
        /// This class is defined because Moq is unable to create Mock objects for 
        /// interfaces that are not publicly available (like the IKernel interface),
        /// even if those interfaces are reachable through an InternalsVisibleToAttribute.
        /// </design>
        private sealed class MockIKernel : IKernel
        {
            #region Implementation of IKernel

            /// <summary>
            /// Installs the specified service.
            /// </summary>
            /// <param name="service">The service which should be installed.</param>
            /// <param name="serviceDomain">The <see cref="AppDomain"/> in which the service resides.</param>
            /// <remarks>
            /// <para>
            /// Only services that are 'installed' can be used by the service manager.
            /// Services that have not been installed are simply unknown to the service
            /// manager.
            /// </para>
            /// <para>
            /// Note that only one instance for each <c>Type</c> can be provided to
            /// the service manager.
            /// </para>
            /// </remarks>
            public void Install(KernelService service, AppDomain serviceDomain)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Uninstalls the specified service.
            /// </summary>
            /// <remarks>
            ///     Once a service is uninstalled it can no longer be started. It is effectively
            ///     removed from the list of known services.
            /// </remarks>
            /// <param name="service">
            ///     The service that needs to be uninstalled.
            /// </param>
            /// <param name="shouldUnloadDomain">
            /// Indicates if the <c>AppDomain</c> that held the service should be unloaded or not.
            /// </param>
            public void Uninstall(KernelService service, bool shouldUnloadDomain)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Determines whether the application can shutdown cleanly.
            /// </summary>
            /// <returns>
            ///     <see langword="true"/> if the application can shutdown cleanly; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool CanShutdown()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Shuts the application down.
            /// </summary>
            public void Shutdown()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        #region Internal class - MockKernelService

        /// <summary>
        /// Defines a mock implementation of the <see cref="KernelService"/> abstract class.
        /// </summary>
        /// <design>
        /// This class is defined because Moq is unable to create Mock objects for 
        /// classes that are not publicly available (like the KernelService class),
        /// even if those interfaces are reachable through an InternalsVisibleToAttribute.
        /// </design>
        private sealed class MockKernelService : KernelService
        {
            #region Overrides of KernelService

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform startup tasks.
            /// </summary>
            protected override void StartService()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform shutdown tasks.
            /// </summary>
            protected override void StopService()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        [Test]
        [Description("Checks that the object returns the correct names for services that should be available.")]
        public void ServicesToBeAvailable()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.AreEqual(0, service.ServicesToBeAvailable().Count());
        }

        [Test]
        [Description("Checks that the object returns the correct names for services to which it should be connected.")]
        public void ServicesToConnectTo()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.AreElementsEqual(new[] { typeof(IMessagePipeline) }, service.ServicesToConnectTo());
        }

        [Test]
        [Description("Checks that the object can be connected to the dependencies.")]
        public void ConnectTo()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from an unknown dependency.")]
        public void DisconnectFromWithNonMatchingServiceType()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MockKernelService());
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from a non-matching reference.")]
        public void DisconnectFromWithNonMatchingObjectReference()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MessagePipeline(new DnsNameConstants()));
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object can be disconnected from the dependencies.")]
        public void DisconnectFrom()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);

            service.DisconnectFrom(pipeline);
            Assert.IsFalse(service.IsConnectedToAllDependencies);
        }
    }
}
