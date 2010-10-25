//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
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
    public sealed class StartupTest
    {
        /// <summary>
        /// The IOC container that contains the core modules.
        /// </summary>
        private IContainer m_CoreContainer;

        private string m_IsLicenseValid = "invalid";

        /// <summary>
        /// Indicates if the application has started. Options are: 'not finished' while the system isn't
        /// started and 'finished' if it has.
        /// </summary>
        private string m_HasStartupCompleted = "not finished";

        public string StartApollo()
        {
            ApolloLoader.Load(ConnectToKernel);
            var applicationFacade = m_CoreContainer.Resolve<IAbstractApplications>();
            applicationFacade.Shutdown(true, () => { });

            return m_HasStartupCompleted;
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
                        m_HasStartupCompleted = "finished";
                    });

                applicationFacade.RegisterNotification(
                    notificationNames.SystemShuttingDown,
                    obj =>
                    {
                        // No nothing at the moment.
                    });
            }
        }

        public string IsLicenseValid()
        {
            return m_IsLicenseValid;
        }

        public string HasStartupCompleted()
        {
            return m_HasStartupCompleted;
        }
    }
}
