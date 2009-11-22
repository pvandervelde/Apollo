//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messages;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Test.Unit
{
    [TestFixture]
    [Description("Tests the Kernel class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class KernelTest
    {
        [Test]
        [Description("Checks that a service cannot be installed with a null reference.")]
        public void InstallServiceWithNullObject()
        {
            var kernel = new Kernel();
            Assert.Throws<ArgumentNullException>(() => kernel.Install(null));
        }

        [Test]
        [Description("Checks that a service cannot be installed if there is already a service of the same type installed.")]
        public void InstallServiceWithAlreadyInstalledService()
        {
            var kernel = new Kernel();
            Assert.Throws<ServiceTypeAlreadyInstalledException>(() => kernel.Install(new CoreProxy()));
        }

        [Test]
        [Description("Checks that a service is installed properly if dependent services are installed last.")]
        public void InstallServiceAsDependentFirst()
        {
            var messageMock = new Mock<KernelService>();
            var messageKernelMock = messageMock.As<IMessagePipeline>();

            KernelService dependency = null;
            var testMock = new Mock<KernelService>();
            var kernelTestMock = testMock.As<IHaveServiceDependencies>();
            {
                kernelTestMock.Setup(test => test.ServicesToBeAvailable())
                    .Returns(new Type[] { });

                kernelTestMock.Setup(test => test.ServicesToConnectTo())
                    .Returns(new Type[] { typeof(IMessagePipeline) });

                kernelTestMock.Setup(test => test.ConnectTo(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { dependency = service; });
            }

            var kernel = new Kernel();
            kernel.Install(messageMock.Object);
            kernel.Install(testMock.Object);

            Assert.AreSame<KernelService>((KernelService)messageKernelMock.Object, dependency);
        }

        [Test]
        [Description("Checks that a service is installed properly if dependent services are installed first.")]
        public void InstallServiceAsDependentLast()
        {
            var messageMock = new Mock<KernelService>();
            var messageKernelMock = messageMock.As<IMessagePipeline>();

            KernelService dependency = null;
            var testMock = new Mock<KernelService>();
            var kernelTestMock = testMock.As<IHaveServiceDependencies>();
            {
                kernelTestMock.Setup(test => test.ServicesToBeAvailable())
                    .Returns(new Type[] { });

                kernelTestMock.Setup(test => test.ServicesToConnectTo())
                    .Returns(new Type[] { typeof(IMessagePipeline) });

                kernelTestMock.Setup(test => test.ConnectTo(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { dependency = service; });
            }

            var kernel = new Kernel();
            kernel.Install(testMock.Object);
            kernel.Install(messageMock.Object);

            Assert.AreSame<KernelService>((KernelService)messageKernelMock.Object, dependency);
        }

        [Test]
        [Description("Checks that a service cannot be uninstalled if it is not installed.")]
        public void UninstallUnknownServiceType()
        {
            var messageMock = new Mock<KernelService>();

            var kernel = new Kernel();
            Assert.Throws<UnknownKernelServiceTypeException>(() => kernel.Uninstall(messageMock.Object));
        }

        [Test]
        [Description("Checks that a service cannot be uninstalled if another object of the same type is installed.")]
        public void UninstallUnknownReference()
        {
            var proxy = new CoreProxy();

            var kernel = new Kernel();
            Assert.Throws<CannotUninstallNonEquivalentServiceException>(() => kernel.Uninstall(proxy));
        }

        [Test]
        [Description("Checks that a service is uninstalled properly if other services depend on it.")]
        public void UninstallServiceAsDependent()
        {
            var messageMock = new Mock<KernelService>();
            var messageKernelMock = messageMock.As<IMessagePipeline>();

            KernelService dependency = null;
            KernelService uninstalledDependency = null;

            var testMock = new Mock<KernelService>();
            var kernelTestMock = testMock.As<IHaveServiceDependencies>();
            {
                kernelTestMock.Setup(test => test.ServicesToBeAvailable())
                    .Returns(new Type[] { });

                kernelTestMock.Setup(test => test.ServicesToConnectTo())
                    .Returns(new Type[] { typeof(IMessagePipeline) });

                kernelTestMock.Setup(test => test.ConnectTo(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { dependency = service; });

                kernelTestMock.Setup(test => test.DisconnectFrom(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { uninstalledDependency = service; });
            }

            var kernel = new Kernel();
            kernel.Install(testMock.Object);
            kernel.Install(messageMock.Object);

            Assert.AreSame<KernelService>((KernelService)messageKernelMock.Object, dependency);

            kernel.Uninstall(messageMock.Object);

            Assert.AreSame<KernelService>((KernelService)messageKernelMock.Object, uninstalledDependency);
        }

        [Test]
        [Description("Checks that a service is uninstalled properly if it depends on other services.")]
        public void UninstallServiceAsDependency()
        {
            var messageMock = new Mock<KernelService>();
            var messageKernelMock = messageMock.As<IMessagePipeline>();

            KernelService dependency = null;
            KernelService uninstalledDependency = null;

            var testMock = new Mock<KernelService>();
            var kernelTestMock = testMock.As<IHaveServiceDependencies>();
            {
                kernelTestMock.Setup(test => test.ServicesToBeAvailable())
                    .Returns(new Type[] { });

                kernelTestMock.Setup(test => test.ServicesToConnectTo())
                    .Returns(new Type[] { typeof(IMessagePipeline) });

                kernelTestMock.Setup(test => test.ConnectTo(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { dependency = service; });

                kernelTestMock.Setup(test => test.DisconnectFrom(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { uninstalledDependency = service; });
            }

            var kernel = new Kernel();
            kernel.Install(testMock.Object);
            kernel.Install(messageMock.Object);

            Assert.AreSame<KernelService>((KernelService)messageKernelMock.Object, dependency);

            kernel.Uninstall(testMock.Object);

            Assert.AreSame<KernelService>((KernelService)messageKernelMock.Object, uninstalledDependency);
        }

        [Test]
        [Ignore("Not implemented yet")]
        [Description("Checks that the kernel can be started.")]
        public void Start()
        { 
            // Add 4 services:
            // - Service 1: IMessagePipeline, no dependencies
            // - Service 2: Depends on IMessagePipeline
            // - Service 3: Depends on IMessagePipeline
            // - Service 4: Depends on IMessagePipeLine, Service 2 and Service 3
            //
            // Order should be:
            // Service 1
            // Service 2 / 3
            // Service 4

            // Use an array list to track the order of startup ...
        }
    }
}
