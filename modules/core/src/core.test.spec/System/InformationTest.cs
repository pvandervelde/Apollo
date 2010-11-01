//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.UserInterfaces.Application;
using Autofac;
using Autofac.Core;
using Concordion.Integration;

namespace Apollo.Core.Test.Spec.System
{
    [ConcordionTest]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Specification tests do not need documentation.")]
    public sealed class InformationTest
    {
        /// <summary>
        /// The IOC container that contains the core modules.
        /// </summary>
        private IContainer m_CoreContainer;

        /// <summary>
        /// Starts the Apollo core and returns a string indicating if the startup has completed or not.
        /// </summary>
        /// <returns>
        /// Returns a value indicating if gathering of the application information was successful or not. The options
        /// are 'information' if it was, or 'no information' if it wasn't.
        /// </returns>
        public string GetInformationFromApollo()
        {
            // Load the core
            ApolloLoader.Load(ConnectToKernel);

            IAbstractApplications applicationFacade = null;
            try
            {
                // Once everything is up and running then we don't need it anymore
                // so dump it.
                applicationFacade = m_CoreContainer.Resolve<IAbstractApplications>();
                var status = applicationFacade.ApplicationStatus;
                if (status.CoreVersion != typeof(IAbstractApplications).Assembly.GetName().Version)
                {
                    return "no information";
                }

                return "information";
            }
            finally
            {
                if (applicationFacade != null)
                {
                    applicationFacade.Shutdown(false, () => { });
                }
            }
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
                    notificationNames.CanSystemShutDown,
                    obj =>
                    {
                        var shutdownArguments = (ShutdownCapabilityArguments)obj;
                        shutdownArguments.CanShutDown = true;
                    });

                applicationFacade.RegisterNotification(
                    notificationNames.SystemShuttingDown,
                    obj =>
                    {
                        // Do nothing
                    });
            }
        }
    }
}
