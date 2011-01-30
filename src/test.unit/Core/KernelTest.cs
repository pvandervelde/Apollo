//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the Kernel class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class KernelTest
    {
        #region Internal class - AdaptableKernelService

        /// <summary>
        /// A mock implementation of <see cref="KernelService"/>.
        /// </summary>
        private sealed class AdaptableKernelService : KernelService, IHaveServiceDependencies
        {
            /// <summary>
            /// Stores the types of the services to which this service should be connected.
            /// </summary>
            private readonly Type[] m_ConnectingServices;

            /// <summary>
            /// Initializes a new instance of the <see cref="AdaptableKernelService"/> class.
            /// </summary>
            /// <param name="connectingServices">The connecting services.</param>
            public AdaptableKernelService(Type[] connectingServices)
            {
                m_ConnectingServices = connectingServices;
            }

            #region Implementation of IHaveServiceDependencies

            /// <summary>
            /// Returns a set of types indicating which services the current service
            /// needs to be linked to in order to be functional.
            /// </summary>
            /// <returns>
            ///     An <see cref="IEnumerable{Type}"/> which contains the types of services
            ///     on which this service depends.
            /// </returns>
            public override IEnumerable<Type> ServicesToConnectTo()
            {
                return m_ConnectingServices;
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
            /// The service to which a connection was made.
            /// </summary>
            private KernelService m_Connection;

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
            /// Returns a set of types indicating which services the current service
            /// needs to be linked to in order to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of services
            /// on which this service depends.
            /// </returns>
            public override IEnumerable<Type> ServicesToConnectTo()
            {
                return new[] { typeof(KernelService1) };
            }

            /// <summary>
            /// Provides one of the services on which the current service depends.
            /// </summary>
            /// <param name="dependency">The dependency service.</param>
            public override void ConnectTo(KernelService dependency)
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
            public override bool IsConnectedToAllDependencies
            {
                get
                {
                    return m_Connection != null;
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
            /// Returns a set of types indicating which services the current service
            /// needs to be linked to in order to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of services
            /// on which this service depends.
            /// </returns>
            public override IEnumerable<Type> ServicesToConnectTo()
            {
                return new[] { typeof(KernelService2) };
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
            /// Returns a set of types indicating which services the current service
            /// needs to be linked to in order to be functional.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerable{Type}"/> which contains the types of services
            /// on which this service depends.
            /// </returns>
            public override IEnumerable<Type> ServicesToConnectTo()
            {
                return new[] { typeof(KernelService2), typeof(KernelService3) };
            }
        }

        #endregion

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
            Assert.Throws<ServiceTypeAlreadyInstalledException>(() => kernel.Install(new CoreProxy(kernel)));
        }

        [Test]
        [Description("Checks that a service cannot depend on itself")]
        public void InstallServiceThatDependsOnItself()
        {
            var testMock = new AdaptableKernelService(
                new Type[] { typeof(AdaptableKernelService) });

            var kernel = new Kernel();
            Assert.Throws<ServiceCannotDependOnItselfException>(() => kernel.Install(testMock));
        }

        [Test]
        [Description("Checks that a service cannot depend on the generic KernelService class")]
        public void InstallServiceThatDependsOnKernelService()
        {
            var testMock = new AdaptableKernelService(
                new Type[] { typeof(KernelService) });

            var kernel = new Kernel();
            Assert.Throws<ServiceCannotDependOnGenericKernelServiceException>(() => kernel.Install(testMock));
        }

        [Test]
        [Description("Checks that a service is installed properly if dependent services are installed last.")]
        public void InstallServiceAsDependentFirst()
        {
            var kernelTestMock1 = new KernelService1(
                service =>
                    {
                        return;
                    },
                service =>
                    {
                       return;
                    });

            var kernelTestMock2 = new KernelService2(
                service =>
                {
                    return;
                },
                service =>
                {
                    return;
                });

            var kernel = new Kernel();
            kernel.Install(kernelTestMock1);
            kernel.Install(kernelTestMock2);

            Assert.AreSame(kernelTestMock1, kernelTestMock2.Connection);
        }

        [Test]
        [Description("Checks that a service is installed properly if dependent services are installed first.")]
        public void InstallServiceAsDependentLast()
        {
            var kernelTestMock1 = new KernelService1(
                service =>
                {
                    return;
                },
                service =>
                {
                    return;
                });

            var kernelTestMock2 = new KernelService2(
                service =>
                {
                    return;
                },
                service =>
                {
                    return;
                });

            var kernel = new Kernel();
            kernel.Install(kernelTestMock2);
            kernel.Install(kernelTestMock1);

            Assert.AreSame(kernelTestMock1, kernelTestMock2.Connection);
        }

        [Test]
        [Description("Checks that the kernel can be started.")]
        public void Start()
        {
            // Add 4 services:
            // - Service 1: no dependencies
            // - Service 2: Depends on Service 1
            // - Service 3: Depends on Service 1
            // - Service 4: Depends on Service 2 and Service 3
            //
            // Order should be:
            // Service 1
            // Service 2 / 3
            // Service 4
            var startupOrder = new List<KernelService>();

            Action<KernelService> storeAction = service => startupOrder.Add(service);
            var testMock1 = new KernelService1(storeAction, service => { return; });
            var testMock2 = new KernelService2(storeAction, service => { return; });
            var testMock3 = new KernelService3(storeAction, service => { return; });
            var testMock4 = new KernelService4(storeAction, service => { return; });

            var kernel = new Kernel();
            kernel.Install(testMock1);
            kernel.Install(testMock2);
            kernel.Install(testMock3);
            kernel.Install(testMock4);

            kernel.Start();

            Assert.AreEqual(4, startupOrder.Count);
            Assert.AreElementsEqual(
                new List<KernelService> 
                    { 
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
            // - Service 1:  no dependencies
            // - Service 2: Depends on Service 1
            // - Service 3: Depends on Service 1
            // - Service 4: Depends on Service 2 and Service 3
            //
            // startup order should be:
            // Service 1
            // Service 2 / 3
            // Service 4
            var stopOrder = new List<KernelService>();

            Action<KernelService> storeAction = service => stopOrder.Add(service);
            var testMock1 = new KernelService1(service => { return; }, storeAction);
            var testMock2 = new KernelService2(service => { return; }, storeAction);
            var testMock3 = new KernelService3(service => { return; }, storeAction);
            var testMock4 = new KernelService4(service => { return; }, storeAction);

            var kernel = new Kernel();
            kernel.Install(testMock1);
            kernel.Install(testMock2);
            kernel.Install(testMock3);
            kernel.Install(testMock4);

            kernel.Start();
            kernel.Shutdown();

            Assert.AreEqual(4, stopOrder.Count);
            Assert.AreElementsEqual(
                new List<KernelService> 
                    { 
                        testMock4,
                        testMock3,
                        testMock2,
                        testMock1,
                    },
                stopOrder);
        }
    }
}
