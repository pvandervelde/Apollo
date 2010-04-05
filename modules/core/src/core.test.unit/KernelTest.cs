//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Apollo.Utils.Commands;
using MbUnit.Framework;
using Moq;
using Moq.Protected;

namespace Apollo.Core.Test.Unit
{
    [TestFixture]
    [Description("Tests the Kernel class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class KernelTest
    {
        #region Internal class - KernelService1

        /// <summary>
        /// A mock kernel service. Used to check the start sequencing.
        /// </summary>
        private sealed class KernelService1 : KernelService, IHaveServiceDependencies
        {
            /// <summary>
            /// The action executed on startup.
            /// </summary>
            private readonly Action<KernelService> m_StartupAction;

            /// <summary>
            /// The action executed on startup.
            /// </summary>
            private readonly Action<KernelService> m_StopAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="KernelService1"/> class.
            /// </summary>
            /// <param name="startupAction">The startup action.</param>
            /// <param name="stopAction">The stop action.</param>
            public KernelService1(Action<KernelService> startupAction, Action<KernelService> stopAction)
            {
                m_StartupAction = startupAction;
                m_StopAction = stopAction;
            }

            /// <summary>
            /// Starts the service.
            /// </summary>
            protected override void StartService()
            {
                m_StartupAction(this);
            }

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform shutdown tasks.
            /// </summary>
            protected override void StopService()
            {
                m_StopAction(this);
            }

            /// <summary>
            /// Returns a set of types indicating which services need to be present
            /// for the current service to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of
            /// services which this service requires to be functional.
            /// </returns>
            public IEnumerable<Type> ServicesToBeAvailable()
            {
                return new Type[] { };
            }

            /// <summary>
            /// Returns a set of types indicating which services the current service
            /// needs to be linked to in order to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of services
            /// on which this service depends.
            /// </returns>
            public IEnumerable<Type> ServicesToConnectTo()
            {
                return new[] { typeof(IMessagePipeline) };
            }

            /// <summary>
            /// Provides one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public void ConnectTo(KernelService dependency)
            {
            }

            /// <summary>
            /// Disconnects from one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public void DisconnectFrom(KernelService dependency)
            {
            }

            /// <summary>
            /// Gets a value indicating whether this instance is connected to all dependencies.
            /// </summary>
            /// <value>
            ///     <see langword="true"/> if this instance is connected to all dependencies; otherwise, <see langword="false"/>.
            /// </value>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsConnectedToAllDependencies
            {
                get 
                {
                    return true;
                }
            }
        } 

        #endregion

        #region Internal class - KernelService2

        /// <summary>
        /// A mock kernel service. Used to check the start sequencing.
        /// </summary>
        private sealed class KernelService2 : KernelService, IHaveServiceDependencies
        {
            /// <summary>
            /// The action executed on startup.
            /// </summary>
            private readonly Action<KernelService> m_StartupAction;

            /// <summary>
            /// The action executed on startup.
            /// </summary>
            private readonly Action<KernelService> m_StopAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="KernelService2"/> class.
            /// </summary>
            /// <param name="startupAction">The startup action.</param>
            /// <param name="stopAction">The stop action.</param>
            public KernelService2(Action<KernelService> startupAction, Action<KernelService> stopAction)
            {
                m_StartupAction = startupAction;
                m_StopAction = stopAction;
            }

            /// <summary>
            /// Starts the service.
            /// </summary>
            protected override void StartService()
            {
                m_StartupAction(this);
            }

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform shutdown tasks.
            /// </summary>
            protected override void StopService()
            {
                m_StopAction(this);
            }

            /// <summary>
            /// Returns a set of types indicating which services need to be present
            /// for the current service to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of
            /// services which this service requires to be functional.
            /// </returns>
            public IEnumerable<Type> ServicesToBeAvailable()
            {
                return new[] { typeof(KernelService1) };
            }

            /// <summary>
            /// Returns a set of types indicating which services the current service
            /// needs to be linked to in order to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of services
            /// on which this service depends.
            /// </returns>
            public IEnumerable<Type> ServicesToConnectTo()
            {
                return new[] { typeof(IMessagePipeline) };
            }

            /// <summary>
            /// Provides one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public void ConnectTo(KernelService dependency)
            {
            }

            /// <summary>
            /// Disconnects from one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public void DisconnectFrom(KernelService dependency)
            {
            }

            /// <summary>
            /// Gets a value indicating whether this instance is connected to all dependencies.
            /// </summary>
            /// <value>
            ///     <see langword="true"/> if this instance is connected to all dependencies; otherwise, <see langword="false"/>.
            /// </value>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsConnectedToAllDependencies
            {
                get
                {
                    return true;
                }
            }
        }

        #endregion

        #region Internal class - KernelService3

        /// <summary>
        /// A mock kernel service. Used to check the start sequencing.
        /// </summary>
        private sealed class KernelService3 : KernelService, IHaveServiceDependencies
        {
            /// <summary>
            /// The action executed on startup.
            /// </summary>
            private readonly Action<KernelService> m_StartupAction;

            /// <summary>
            /// The action executed on startup.
            /// </summary>
            private readonly Action<KernelService> m_StopAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="KernelService3"/> class.
            /// </summary>
            /// <param name="startupAction">The startup action.</param>
            /// <param name="stopAction">The stop action.</param>
            public KernelService3(Action<KernelService> startupAction, Action<KernelService> stopAction)
            {
                m_StartupAction = startupAction;
                m_StopAction = stopAction;
            }

            /// <summary>
            /// Starts the service.
            /// </summary>
            protected override void StartService()
            {
                m_StartupAction(this);
            }

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform shutdown tasks.
            /// </summary>
            protected override void StopService()
            {
                m_StopAction(this);
            }

            /// <summary>
            /// Returns a set of types indicating which services need to be present
            /// for the current service to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of
            /// services which this service requires to be functional.
            /// </returns>
            public IEnumerable<Type> ServicesToBeAvailable()
            {
                return new[] { typeof(KernelService2) };
            }

            /// <summary>
            /// Returns a set of types indicating which services the current service
            /// needs to be linked to in order to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of services
            /// on which this service depends.
            /// </returns>
            public IEnumerable<Type> ServicesToConnectTo()
            {
                return new[] { typeof(IMessagePipeline) };
            }

            /// <summary>
            /// Provides one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public void ConnectTo(KernelService dependency)
            {
            }

            /// <summary>
            /// Disconnects from one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public void DisconnectFrom(KernelService dependency)
            {
            }

            /// <summary>
            /// Gets a value indicating whether this instance is connected to all dependencies.
            /// </summary>
            /// <value>
            ///     <see langword="true"/> if this instance is connected to all dependencies; otherwise, <see langword="false"/>.
            /// </value>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsConnectedToAllDependencies
            {
                get
                {
                    return true;
                }
            }
        }

        #endregion

        #region Internal class - KernelService4

        /// <summary>
        /// A mock kernel service. Used to check the start sequencing.
        /// </summary>
        private sealed class KernelService4 : KernelService, IHaveServiceDependencies
        {
            /// <summary>
            /// The action executed on startup.
            /// </summary>
            private readonly Action<KernelService> m_StartupAction;

            /// <summary>
            /// The action executed on startup.
            /// </summary>
            private readonly Action<KernelService> m_StopAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="KernelService4"/> class.
            /// </summary>
            /// <param name="startupAction">The startup action.</param>
            /// <param name="stopAction">The stop action.</param>
            public KernelService4(Action<KernelService> startupAction, Action<KernelService> stopAction)
            {
                m_StartupAction = startupAction;
                m_StopAction = stopAction;
            }

            /// <summary>
            /// Starts the service.
            /// </summary>
            protected override void StartService()
            {
                m_StartupAction(this);
            }

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform shutdown tasks.
            /// </summary>
            protected override void StopService()
            {
                m_StopAction(this);
            }

            /// <summary>
            /// Returns a set of types indicating which services need to be present
            /// for the current service to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of
            /// services which this service requires to be functional.
            /// </returns>
            public IEnumerable<Type> ServicesToBeAvailable()
            {
                return new[] { typeof(KernelService3) };
            }

            /// <summary>
            /// Returns a set of types indicating which services the current service
            /// needs to be linked to in order to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of services
            /// on which this service depends.
            /// </returns>
            public IEnumerable<Type> ServicesToConnectTo()
            {
                return new[] { typeof(IMessagePipeline), typeof(KernelService2) };
            }

            /// <summary>
            /// Provides one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public void ConnectTo(KernelService dependency)
            {
            }

            /// <summary>
            /// Disconnects from one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public void DisconnectFrom(KernelService dependency)
            {
            }

            /// <summary>
            /// Gets a value indicating whether this instance is connected to all dependencies.
            /// </summary>
            /// <value>
            ///     <see langword="true"/> if this instance is connected to all dependencies; otherwise, <see langword="false"/>.
            /// </value>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsConnectedToAllDependencies
            {
                get
                {
                    return true;
                }
            }
        }

        #endregion

        [Test]
        [Description("Checks that a service cannot be installed with a null reference.")]
        public void InstallServiceWithNullObject()
        {
            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            Assert.Throws<ArgumentNullException>(() => kernel.Install(null, AppDomain.CurrentDomain));
        }

        [Test]
        [Description("Checks that a service cannot be installed if there is already a service of the same type installed.")]
        public void InstallServiceWithAlreadyInstalledService()
        {
            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            Assert.Throws<ServiceTypeAlreadyInstalledException>(() => kernel.Install(new CoreProxy(kernel, new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object), AppDomain.CurrentDomain));
        }

        [Test]
        [Description("Checks that a service cannot depend on itself")]
        public void InstallServiceThatDependsOnItself()
        {
            var testMock = new Mock<KernelService>();
            var kernelTestMock = testMock.As<IHaveServiceDependencies>();
            {
                kernelTestMock.Setup(test => test.ServicesToBeAvailable())
                    .Returns(new Type[] { });

                kernelTestMock.Setup(test => test.ServicesToConnectTo())
                    .Returns(new[] { testMock.Object.GetType() });
            }

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            Assert.Throws<ServiceCannotDependOnItselfException>(() => kernel.Install(testMock.Object, AppDomain.CurrentDomain));
        }

        [Test]
        [Description("Checks that a service cannot depend on the generic KernelService class")]
        public void InstallServiceThatDependsOnKernelService()
        {
            var testMock = new Mock<KernelService>();
            var kernelTestMock = testMock.As<IHaveServiceDependencies>();
            {
                kernelTestMock.Setup(test => test.ServicesToBeAvailable())
                    .Returns(new Type[] { });

                kernelTestMock.Setup(test => test.ServicesToConnectTo())
                    .Returns(new[] { typeof(KernelService) });
            }

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            Assert.Throws<ServiceCannotDependOnGenericKernelServiceException>(() => kernel.Install(testMock.Object, AppDomain.CurrentDomain));
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
                    .Returns(new[] { typeof(IMessagePipeline) });

                kernelTestMock.Setup(test => test.ConnectTo(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { dependency = service; });
            }

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            kernel.Install(messageMock.Object, AppDomain.CurrentDomain);
            kernel.Install(testMock.Object, AppDomain.CurrentDomain);

            Assert.AreSame((KernelService)messageKernelMock.Object, dependency);
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
                    .Returns(new[] { typeof(IMessagePipeline) });

                kernelTestMock.Setup(test => test.ConnectTo(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { dependency = service; });
            }

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            kernel.Install(testMock.Object, AppDomain.CurrentDomain);
            kernel.Install(messageMock.Object, AppDomain.CurrentDomain);

            Assert.AreSame((KernelService)messageKernelMock.Object, dependency);
        }

        [Test]
        [Description("Checks that a service cannot be uninstalled if it is not installed.")]
        public void UninstallUnknownServiceType()
        {
            var messageMock = new Mock<KernelService>();

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            Assert.Throws<UnknownKernelServiceTypeException>(() => kernel.Uninstall(messageMock.Object, false));
        }

        [Test]
        [Description("Checks that a service cannot be uninstalled if another object of the same type is installed.")]
        public void UninstallUnknownReference()
        {
            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            var secondProxy = new CoreProxy(kernel, new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            Assert.Throws<CannotUninstallNonequivalentServiceException>(() => kernel.Uninstall(secondProxy, false));
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
                    .Returns(new[] { typeof(IMessagePipeline) });

                kernelTestMock.Setup(test => test.ConnectTo(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { dependency = service; });

                kernelTestMock.Setup(test => test.DisconnectFrom(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { uninstalledDependency = service; });
            }

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            kernel.Install(testMock.Object, AppDomain.CurrentDomain);
            kernel.Install(messageMock.Object, AppDomain.CurrentDomain);

            Assert.AreSame((KernelService)messageKernelMock.Object, dependency);

            kernel.Uninstall(messageMock.Object, false);

            Assert.AreSame((KernelService)messageKernelMock.Object, uninstalledDependency);
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
                    .Returns(new[] { typeof(IMessagePipeline) });

                kernelTestMock.Setup(test => test.ConnectTo(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { dependency = service; });

                kernelTestMock.Setup(test => test.DisconnectFrom(It.IsAny<KernelService>()))
                    .Callback<KernelService>(service => { uninstalledDependency = service; });
            }

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            kernel.Install(testMock.Object, AppDomain.CurrentDomain);
            kernel.Install(messageMock.Object, AppDomain.CurrentDomain);

            Assert.AreSame((KernelService)messageKernelMock.Object, dependency);

            kernel.Uninstall(testMock.Object, false);

            Assert.AreSame((KernelService)messageKernelMock.Object, uninstalledDependency);
        }

        [Test]
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
            var startupOrder = new List<KernelService>();

            // Define the KernelService mock
            var messageMock = new Mock<KernelService>();

            // Now implement IMessagePipeline on that mock
            messageMock.As<IMessagePipeline>();
            {
                messageMock.Protected().Setup("StartService")
                    .Callback(() => startupOrder.Add(messageMock.Object));
            }

            Action<KernelService> storeAction = service => startupOrder.Add(service);
            var testMock1 = new KernelService1(storeAction, service => { return; });
            var testMock2 = new KernelService2(storeAction, service => { return; });
            var testMock3 = new KernelService3(storeAction, service => { return; });
            var testMock4 = new KernelService4(storeAction, service => { return; });

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            kernel.Install(testMock1, AppDomain.CurrentDomain);
            kernel.Install(messageMock.Object, AppDomain.CurrentDomain);
            kernel.Install(testMock2, AppDomain.CurrentDomain);
            kernel.Install(testMock3, AppDomain.CurrentDomain);
            kernel.Install(testMock4, AppDomain.CurrentDomain);

            kernel.Start();

            Assert.AreEqual(5, startupOrder.Count);
            Assert.AreElementsEqual(
                new List<KernelService> 
                    { 
                        messageMock.Object,
                        testMock1,
                        testMock2,
                        testMock3,
                        testMock4
                    },
                startupOrder);
        }

        [Test]
        [Description("Checks that the kernel can be stopped.")]
        public void Stop()
        {
            // Add 4 services:
            // - Service 1: IMessagePipeline, no dependencies
            // - Service 2: Depends on IMessagePipeline
            // - Service 3: Depends on IMessagePipeline
            // - Service 4: Depends on IMessagePipeLine, Service 2 and Service 3
            //
            // startup order should be:
            // Service 1
            // Service 2 / 3
            // Service 4
            var stopOrder = new List<KernelService>();

            // Define the KernelService mock
            var messageMock = new Mock<KernelService>();

            // Now implement IMessagePipeline on that mock
            messageMock.As<IMessagePipeline>();
            {
                messageMock.Protected().Setup("StopService")
                    .Callback(() => stopOrder.Add(messageMock.Object));
            }

            Action<KernelService> storeAction = service => stopOrder.Add(service);
            var testMock1 = new KernelService1(service => { return; }, storeAction);
            var testMock2 = new KernelService2(service => { return; }, storeAction);
            var testMock3 = new KernelService3(service => { return; }, storeAction);
            var testMock4 = new KernelService4(service => { return; }, storeAction);

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object);
            kernel.Install(testMock1, AppDomain.CurrentDomain);
            kernel.Install(messageMock.Object, AppDomain.CurrentDomain);
            kernel.Install(testMock2, AppDomain.CurrentDomain);
            kernel.Install(testMock3, AppDomain.CurrentDomain);
            kernel.Install(testMock4, AppDomain.CurrentDomain);

            kernel.Start();
            kernel.Shutdown();

            Assert.AreEqual(5, stopOrder.Count);
            Assert.AreElementsEqual(
                new List<KernelService> 
                    { 
                        testMock4,
                        testMock3,
                        testMock2,
                        testMock1,
                        messageMock.Object,
                    },
                stopOrder);
        }
    }
}
