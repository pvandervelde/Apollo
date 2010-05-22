﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.UserInterfaces.Application;
using Apollo.Utils;
using Autofac;
using Lokad;

namespace Apollo.Core.UserInterfaces
{
    /// <summary>
    /// Handles the component registrations for the User Interface part 
    /// of the core.
    /// </summary>
    [ExcludeFromCoverage("Modules are used for dependency injection purposes. Testing is done through integration testing.")]
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
        /// <param name="moduleBuilder">The builder.</param>
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            base.Load(moduleBuilder);

            moduleBuilder.Register(c => new NotificationNameConstants())
                .As<INotificationNameConstants>();

            moduleBuilder.Register(c => new DnsNameConstants())
                .As<IDnsNameConstants>();

            moduleBuilder.Register(c => m_Owner)
                .As<IUserInterfaceService>()
                .ExternallyOwned();

            moduleBuilder.Register(c => new ApplicationFacade(c.Resolve<IUserInterfaceService>()))
                .As<IAbstractApplications>();

            // IInteractWithUsers
            // ILinkToProjects
            // IGiveAdvice
        }
    }
}