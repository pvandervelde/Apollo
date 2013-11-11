//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Host.Scripting;
using Apollo.Core.Host.UserInterfaces.Application;
using Apollo.Core.Host.UserInterfaces.Projects;
using Autofac;
using Lokad;
using Nuclei.Diagnostics;

namespace Apollo.Core.Host.UserInterfaces
{
    /// <summary>
    /// Handles the component registrations for the User Interface part 
    /// of the core.
    /// </summary>
    internal sealed class UserInterfaceModule : Module
    {
        /// <summary>
        /// The service that owns the user interface connection to the
        /// kernel.
        /// </summary>
        private readonly IUserInterfaceService m_Owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInterfaceModule"/> class.
        /// </summary>
        /// <param name="ownerService">The owner service.</param>
        public UserInterfaceModule(IUserInterfaceService ownerService)
        {
            {
                Enforce.Argument(() => ownerService);
            }

            m_Owner = ownerService;
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterModule(new ScriptingModule());

            builder.Register(c => new NotificationNameConstants())
                .As<INotificationNameConstants>();

            builder.Register(c => m_Owner)
                .As<IUserInterfaceService>()
                .ExternallyOwned();

            builder.Register(c => new ApplicationFacade(
                    c.Resolve<IUserInterfaceService>(),
                    c.Resolve<INotificationNameConstants>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IAbstractApplications>()
                .SingleInstance();

            builder.Register(c => new ProjectHub(c.Resolve<IUserInterfaceService>()))
                .As<ILinkToProjects>()
                .SingleInstance();
        }
    }
}
