﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.UserInterfaces.Application;
using Apollo.Core.UserInterfaces.Projects;
using Autofac;
using Lokad;

namespace Apollo.Core.UserInterfaces
{
    /// <summary>
    /// Handles the component registrations for the User Interface part 
    /// of the core.
    /// </summary>
    [ExcludeFromCodeCoverage]
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

            builder.Register(c => new NotificationNameConstants())
                .As<INotificationNameConstants>();

            builder.Register(c => m_Owner)
                .As<IUserInterfaceService>()
                .ExternallyOwned();

            builder.Register(c => new ApplicationFacade(c.Resolve<IUserInterfaceService>()))
                .As<IAbstractApplications>()
                .SingleInstance();

            builder.Register(c => new ProjectServiceFacade(c.Resolve<IUserInterfaceService>()))
                .As<ILinkToProjects>()
                .SingleInstance();

            // IInteractWithUsers
            // IGiveAdvice
        }
    }
}
