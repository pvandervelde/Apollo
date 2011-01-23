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
    public sealed class CheckIfShutdownIsPossibleTest
    {
        /// <summary>
        /// The IOC container that contains the core modules.
        /// </summary>
        private IContainer m_CoreContainer;

        /// <summary>
        /// Starts the Apollo core and returns a string indicating if the startup has completed or not.
        /// </summary>
        /// <returns>A string indicating if the application can shut down or not.</returns>
        public string VerifyThatTheApplicationCanShutdown()
        {
            // Load the core
            ApolloLoader.Load(ConnectToKernel);

            // Once everything is up and running then we don't need it anymore
            // so dump it.
            var applicationFacade = m_CoreContainer.Resolve<IAbstractApplications>();

            var canShutdown = applicationFacade.CanShutdown();
            applicationFacade.Shutdown(false, () => { });

            return canShutdown ? "shutdown is possible" : "shutdown is not possible";
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
                    notificationNames.SystemShuttingDown,
                    obj =>
                    {
                        // Do nothing for now
                    });
            }
        }
    }
}
