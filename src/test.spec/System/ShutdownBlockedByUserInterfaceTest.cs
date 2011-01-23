//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core;
using Apollo.Core.UserInterfaces.Application;
using Autofac;
using Autofac.Core;
using Concordion.Integration;

namespace Test.Spec.System
{
    [ConcordionTest]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Specification tests do not need documentation.")]
    public sealed class ShutdownBlockedByUserInterfaceTest
    {
        /// <summary>
        /// The IOC container that contains the core modules.
        /// </summary>
        private IContainer m_CoreContainer;

        /// <summary>
        /// Indicates if the shut down process has started. Options are: 'not started' while the system hasn't
        /// started the shut down process and 'started' if it has.
        /// </summary>
        private string m_ShutdownStarted = "not started";

        /// <summary>
        /// Indicates if the shut down process was stopped. Options are: 'not stop' if the shut down was not stopped
        /// and 'stop' if it was.
        /// </summary>
        private string m_HasShutdownBeenStopped = "not stop";

        /// <summary>
        /// Indicates if the shut down process has finished successfully. Options are 'failed' while the system
        /// hasn't completed the shut down and 'succesful' if it has.
        /// </summary>
        private string m_ShutdownCompletedSuccessfully = "failed";

        /// <summary>
        /// Indicates if the shut down should be blocked.
        /// </summary>
        private bool m_ShouldBlockShutdown = true;

        /// <summary>
        /// Starts the Apollo core and returns a string indicating if the startup has completed or not.
        /// </summary>
        public void ShutdownApollo()
        {
            // Load the core
            ApolloLoader.Load(ConnectToKernel);

            // Once everything is up and running then we don't need it anymore
            // so dump it.
            var applicationFacade = m_CoreContainer.Resolve<IAbstractApplications>();

            applicationFacade.Shutdown(false, () => { m_HasShutdownBeenStopped = "stop"; });

            m_ShutdownCompletedSuccessfully = "succesfully";
            m_ShouldBlockShutdown = false;
            applicationFacade.Shutdown(false, () => { m_ShutdownCompletedSuccessfully = "failed"; });
        }

        private void ConnectToKernel(IModule kernelUserInterfaceModule)
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(kernelUserInterfaceModule);
            }

            m_CoreContainer = builder.Build();
            RegisterForStartupCompleteEvent(m_CoreContainer);
        }

        private void RegisterForStartupCompleteEvent(IContainer container)
        {
            var notificationNames = container.Resolve<INotificationNameConstants>();
            var applicationFacade = container.Resolve<IAbstractApplications>();
            {
                applicationFacade.RegisterNotification(
                    notificationNames.StartupComplete,
                    obj =>
                    {
                        // Do nothing for now
                    });

                applicationFacade.RegisterNotification(
                    notificationNames.CanSystemShutdown,
                    obj =>
                    {
                        var shutdownArguments = (ShutdownCapabilityArguments)obj;
                        shutdownArguments.CanShutdown = !m_ShouldBlockShutdown;
                    });

                applicationFacade.RegisterNotification(
                    notificationNames.SystemShuttingDown,
                    obj =>
                    {
                        m_ShutdownStarted = "start";
                    });
            }
        }

        public string HasShutdownStarted()
        {
            return m_ShutdownStarted;
        }

        public string HasShutdownBeenStopped()
        {
            return m_HasShutdownBeenStopped;
        }

        public string HasShutdownCompletedSuccessfully()
        {
            return m_ShutdownCompletedSuccessfully;
        }
    }
}
