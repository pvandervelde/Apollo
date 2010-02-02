//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.UserInterfaces.Application;
using Autofac;
using Lokad;

namespace Apollo.Core.UserInterfaces
{
    /// <summary>
    /// Handles the component registrations for the User Interface part 
    /// of the kernel.
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
        /// <param name="builder">The builder.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new NotificationNameConstants())
                .As<INotificationNameConstants>();

            builder.Register(c => new DnsNameConstants())
                .As<IDnsNameConstants>();

            builder.Register(c => m_Owner)
                .As<IUserInterfaceService>()
                .ExternallyOwned();

            builder.Register(c => new ApplicationFacade(c.Resolve<IUserInterfaceService>()))
                .As<IAbstractApplications>();

            // IInteractWithUsers
            // ILinkToProjects
            // IGiveAdvice
        }
    }
}
