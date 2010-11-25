//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Apollo.Utils;
using Apollo.Utils.Commands;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the Kernel class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class KernelTest
    {
        #region Internal class - MockPipeline

        /// <summary>
        /// A mock implementation of <see cref="KernelService"/> and <see cref="IMessagePipeline"/>.
        /// </summary>
        private sealed class MockPipeline : KernelService, IMessagePipeline
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
            /// Initializes a new instance of the <see cref="MockPipeline"/> class.
            /// </summary>
            public MockPipeline() : this(null, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MockPipeline"/> class.
            /// </summary>
            /// <param name="startupAction">The startup action.</param>
            /// <param name="stopAction">The stop action.</param>
            public MockPipeline(Action<KernelService> startupAction, Action<KernelService> stopAction)
            {
                m_StartupAction = startupAction;
                m_StopAction = stopAction;
            }

            #region Overrides of KernelService

            /// <summary>
            /// Starts the service.
            /// </summary>
            protected override void StartService()
            {
                if (m_StartupAction != null)
                {
                    m_StartupAction(this);
                }
            }

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform shutdown tasks.
            /// </summary>
            protected override void StopService()
            {
                if (m_StopAction != null)
                {
                    m_StopAction(this);
                }
            }

            #endregion

            #region Implementation of IMessagePipeline

            /// <summary>
            /// Determines whether a service with the specified name is registered.
            /// </summary>
            /// <param name="name">The name of the service.</param>
            /// <returns>
            ///     <see langword="true"/> if a service with the specified name is registered; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsRegistered(DnsName name)
            {
                return false;
            }

            /// <summary>
            /// Determines whether the specified service is registered.
            /// </summary>
            /// <param name="service">The service.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified service is registered; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsRegistered(IProcessMessages service)
            {
                return false;
            }

            /// <summary>
            /// Determines whether the specified service is registered.
            /// </summary>
            /// <param name="service">The service.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified service is registered; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsRegistered(ISendMessages service)
            {
                return false;
            }

            /// <summary>
            /// Registers as listener.
            /// </summary>
            /// <param name="service">The service.</param>
            public void RegisterAsListener(IProcessMessages service)
            {
            }

            /// <summary>
            /// Registers as sender.
            /// </summary>
            /// <param name="service">The service.</param>
            public void RegisterAsSender(ISendMessages service)
            {
            }

            /// <summary>
            /// Registers the specified service.
            /// </summary>
            /// <param name="service">The service.</param>
            public void Register(object service)
            {
            }

            /// <summary>
            /// Unregisters the specified service.
            /// </summary>
            /// <param name="service">The service.</param>
            public void Unregister(object service)
            {
            }

            /// <summary>
            /// Unregisters as listener.
            /// </summary>
            /// <param name="service">The service.</param>
            public void UnregisterAsListener(IProcessMessages service)
            {
            }

            /// <summary>
            /// Unregisters as sender.
            /// </summary>
            /// <param name="service">The service.</param>
            public void UnregisterAsSender(ISendMessages service)
            {
            }

            /// <summary>
            /// Sends the specified sender.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="recipient">The recipient.</param>
            /// <param name="information">The information.</param>
            /// <returns>The ID number of the newly send message.</returns>
            public MessageId Send(DnsName sender, DnsName recipient, MessageBody information)
            {
                return MessageId.Next();
            }

            /// <summary>
            /// Sends the specified sender.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="recipient">The recipient.</param>
            /// <param name="information">The information.</param>
            /// <param name="inReplyTo">The in reply to.</param>
            /// <returns>The ID number of the newly send message.</returns>
            public MessageId Send(DnsName sender, DnsName recipient, MessageBody information, MessageId inReplyTo)
            {
                return MessageId.Next();
            }

            #endregion
        }

        #endregion

        #region Internal class - AdaptableKernelService

        /// <summary>
        /// A mock implementation of <see cref="KernelService"/>.
        /// </summary>
        private sealed class AdaptableKernelService : KernelService, IHaveServiceDependencies
        {
            /// <summary>
            /// Stores the types of the services that should be available.
            /// </summary>
            private readonly Type[] m_AvailableServices;

            /// <summary>
            /// Stores the types of the services to which this service should be connected.
            /// </summary>
            private readonly Type[] m_ConnectingServices;

            /// <summary>
            /// Initializes a new instance of the <see cref="AdaptableKernelService"/> class.
            /// </summary>
            /// <param name="availableServices">The available services.</param>
            /// <param name="connectingServices">The connecting services.</param>
            public AdaptableKernelService(Type[] availableServices, Type[] connectingServices)
            {
                m_AvailableServices = availableServices;
                m_ConnectingServices = connectingServices;
            }

            #region Overrides of KernelService

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform startup tasks.
            /// </summary>
            protected override void StartService()
            {
                // Do nothing
            }

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform shutdown tasks.
            /// </summary>
            protected override void StopService()
            {
                // Do nothing
            }

            #endregion

            #region Implementation of IHaveServiceDependencies

            /// <summary>
            /// Returns a set of types indicating which services need to be present
            /// for the current service to be functional.
            /// </summary>
            /// <returns>
            ///     An <see cref="IEnumerable{Type}"/> which contains the types of 
            ///     services which this service requires to be functional.
            /// </returns>
            public IEnumerable<Type> ServicesToBeAvailable()
            {
                return m_AvailableServices;
            }

            /// <summary>
            /// Returns a set of types indicating which services the current service
            /// needs to be linked to in order to be functional.
            /// </summary>
            /// <returns>
            ///     An <see cref="IEnumerable{Type}"/> which contains the types of services
            ///     on which this service depends.
            /// </returns>
            public IEnumerable<Type> ServicesToConnectTo()
            {
                return m_ConnectingServices;
            }

            /// <summary>
            /// Provides one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public void ConnectTo(KernelService dependency)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Disconnects from one of the services on which the current service depends.
            /// </summary> 
            /// <param name="dependency">The dependency service.</param>
            public void DisconnectFrom(KernelService dependency)
            {
                throw new NotImplementedException();
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
                    throw new NotImplementedException();
                }
            }

            #endregion
        }

        #endregion

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
            /// The service to which a connection was made.
            /// </summary>
            private KernelService m_Connection;

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
                m_Connection = dependency;
            }

            /// <summary>
            /// Disconnects from one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public void DisconnectFrom(KernelService dependency)
            {
                m_Connection = dependency;
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

            /// <summary>
            /// Gets the connection.
            /// </summary>
            /// <value>The connection.</value>
            public KernelService Connection
            {
                get
                {
                    return m_Connection;
                }
            }

            /// <summary>
            /// Resets this instance.
            /// </summary>
            internal void Reset()
            {
                m_Connection = null;
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
            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.Throws<ArgumentNullException>(() => kernel.Install(null, AppDomain.CurrentDomain));
        }

        [Test]
        [Description("Checks that a service cannot be installed if there is already a service of the same type installed.")]
        public void InstallServiceWithAlreadyInstalledService()
        {
            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.Throws<ServiceTypeAlreadyInstalledException>(() => kernel.Install(new CoreProxy(kernel, new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants()), AppDomain.CurrentDomain));
        }

        [Test]
        [Description("Checks that a service cannot depend on itself")]
        public void InstallServiceThatDependsOnItself()
        {
            var testMock = new AdaptableKernelService(
                new Type[0], 
                new Type[] { typeof(AdaptableKernelService) });

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.Throws<ServiceCannotDependOnItselfException>(() => kernel.Install(testMock, AppDomain.CurrentDomain));
        }

        [Test]
        [Description("Checks that a service cannot depend on the generic KernelService class")]
        public void InstallServiceThatDependsOnKernelService()
        {
            var testMock = new AdaptableKernelService(
                new Type[0], 
                new Type[] { typeof(KernelService) });

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.Throws<ServiceCannotDependOnGenericKernelServiceException>(() => kernel.Install(testMock, AppDomain.CurrentDomain));
        }

        [Test]
        [Description("Checks that a service is installed properly if dependent services are installed last.")]
        public void InstallServiceAsDependentFirst()
        {
            var messageKernelMock = new MockPipeline();

            var kernelTestMock = new KernelService1(
                service =>
                    {
                        return;
                    },
                service =>
                    {
                       return;
                    });

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            kernel.Install(messageKernelMock, AppDomain.CurrentDomain);
            kernel.Install(kernelTestMock, AppDomain.CurrentDomain);

            Assert.AreSame(messageKernelMock, kernelTestMock.Connection);
        }

        [Test]
        [Description("Checks that a service is installed properly if dependent services are installed first.")]
        public void InstallServiceAsDependentLast()
        {
            var messageKernelMock = new MockPipeline();

            var kernelTestMock = new KernelService1(
                service =>
                    {
                        return;
                    },
                service =>
                    {
                       return;
                    });

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            kernel.Install(kernelTestMock, AppDomain.CurrentDomain);
            kernel.Install(messageKernelMock, AppDomain.CurrentDomain);

            Assert.AreSame(messageKernelMock, kernelTestMock.Connection);
        }

        [Test]
        [Description("Checks that a service cannot be uninstalled if it is not installed.")]
        public void UninstallUnknownServiceType()
        {
            var messageMock = new MockPipeline();

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.Throws<UnknownKernelServiceTypeException>(() => kernel.Uninstall(messageMock, false));
        }

        [Test]
        [Description("Checks that a service cannot be uninstalled if another object of the same type is installed.")]
        public void UninstallUnknownReference()
        {
            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            var service1 = new KernelService1(
                s =>
                    {
                    }, 
                s =>
                    {
                    });

            kernel.Install(service1, AppDomain.CurrentDomain);

            var service2 = new KernelService1(
                s =>
                    {
                    }, 
                s =>
                    {
                    });

            Assert.Throws<CannotUninstallNonequivalentServiceException>(() => kernel.Uninstall(service2, false));
        }

        [Test]
        [Description("Checks that the CoreProxy service cannot be uninstalled.")]
        public void UninstallCoreProxy()
        {
            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            var secondProxy = new CoreProxy(kernel, new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            kernel.Uninstall(secondProxy, false);
            Assert.Throws<ServiceTypeAlreadyInstalledException>(() => kernel.Install(secondProxy, AppDomain.CurrentDomain));
        }

        [Test]
        [Description("Checks that a service is uninstalled properly if other services depend on it.")]
        public void UninstallServiceAsDependent()
        {
            var messageMock = new MockPipeline();

            var kernelTestMock = new KernelService1(
                service =>
                    {
                        return;
                    },
                service =>
                    {
                       return;
                    });

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            kernel.Install(kernelTestMock, AppDomain.CurrentDomain);
            kernel.Install(messageMock, AppDomain.CurrentDomain);

            Assert.AreSame(messageMock, kernelTestMock.Connection);
            kernelTestMock.Reset();

            kernel.Uninstall(messageMock, false);

            Assert.AreSame(messageMock, kernelTestMock.Connection);
        }

        [Test]
        [Description("Checks that a service is uninstalled properly if it depends on other services.")]
        public void UninstallServiceAsDependency()
        {
            var messageMock = new MockPipeline();

            var kernelTestMock = new KernelService1(
                service =>
                    {
                        return;
                    },
                service =>
                    {
                       return;
                    });

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            kernel.Install(kernelTestMock, AppDomain.CurrentDomain);
            kernel.Install(messageMock, AppDomain.CurrentDomain);

            Assert.AreSame(messageMock, kernelTestMock.Connection);
            kernelTestMock.Reset();

            kernel.Uninstall(kernelTestMock, false);

            Assert.AreSame(messageMock, kernelTestMock.Connection);
        }

        [Test]
        [Description("Checks that services from a different AppDomain react properly if the service gets unloaded.")]
        public void UninstallServiceFromDifferentAppDomain()
        {
            var kernelTestMock = new KernelService1(
                service =>
                    {
                        return;
                    },
                service =>
                    {
                       return;
                    });
            
            var domainSetup = new AppDomainSetup();
            {
                domainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            }

            var domain = AppDomain.CreateDomain("testDomain", null, domainSetup);
            var messageMock = domain.CreateInstanceAndUnwrap(
                typeof(MockPipeline).Assembly.FullName, 
                typeof(MockPipeline).FullName) as MockPipeline;

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            kernel.Install(messageMock, domain);
            kernel.Install(kernelTestMock, AppDomain.CurrentDomain);

            Assert.AreSame(messageMock, kernelTestMock.Connection);
            kernelTestMock.Reset();

            kernel.Uninstall(messageMock, true);

            Assert.AreSame(messageMock, kernelTestMock.Connection);
        }

        [Test]
        [Description("Checks that services from a different AppDomain react properly if the domain gets unloaded.")]
        public void UnloadServiceAppDomain()
        {
            var kernelTestMock = new KernelService1(
                service =>
                    {
                        return;
                    },
                service =>
                    {
                       return;
                    });
            
            var domainSetup = new AppDomainSetup();
            {
                domainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            }

            var domain = AppDomain.CreateDomain("testDomain", null, domainSetup);
            var messageMock = domain.CreateInstanceAndUnwrap(
                typeof(MockPipeline).Assembly.FullName, 
                typeof(MockPipeline).FullName) as MockPipeline;

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            kernel.Install(messageMock, domain);
            kernel.Install(kernelTestMock, AppDomain.CurrentDomain);

            Assert.AreSame(messageMock, kernelTestMock.Connection);
            kernelTestMock.Reset();

            AppDomain.Unload(domain);

            Assert.AreSame(messageMock, kernelTestMock.Connection);
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

            Action<KernelService> storeAction = service => startupOrder.Add(service);
            var messageMock = new MockPipeline(storeAction, service => { return; });
            var testMock1 = new KernelService1(storeAction, service => { return; });
            var testMock2 = new KernelService2(storeAction, service => { return; });
            var testMock3 = new KernelService3(storeAction, service => { return; });
            var testMock4 = new KernelService4(storeAction, service => { return; });

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            kernel.Install(testMock1, AppDomain.CurrentDomain);
            kernel.Install(messageMock, AppDomain.CurrentDomain);
            kernel.Install(testMock2, AppDomain.CurrentDomain);
            kernel.Install(testMock3, AppDomain.CurrentDomain);
            kernel.Install(testMock4, AppDomain.CurrentDomain);

            kernel.Start();

            Assert.AreEqual(5, startupOrder.Count);
            Assert.AreElementsEqual(
                new List<KernelService> 
                    { 
                        messageMock,
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

            Action<KernelService> storeAction = service => stopOrder.Add(service);
            var messageMock = new MockPipeline(service => { return; }, storeAction);
            var testMock1 = new KernelService1(service => { return; }, storeAction);
            var testMock2 = new KernelService2(service => { return; }, storeAction);
            var testMock3 = new KernelService3(service => { return; }, storeAction);
            var testMock4 = new KernelService4(service => { return; }, storeAction);

            var kernel = new Kernel(new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            kernel.Install(testMock1, AppDomain.CurrentDomain);
            kernel.Install(messageMock, AppDomain.CurrentDomain);
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
                        messageMock,
                    },
                stopOrder);
        }
    }
}
