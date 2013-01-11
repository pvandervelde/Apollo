//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Lokad;

namespace Apollo.Core.Host.UserInterfaces.Application
{
    /// <summary>
    /// Defines methods for general interaction with the system kernel.
    /// </summary>
    internal sealed class ApplicationFacade : IAbstractApplications
    {
        /// <summary>
        /// The UI service that handles the communication with the rest of the system.
        /// </summary>
        private readonly IUserInterfaceService m_Service;

        /// <summary>
        /// The object that holds general information about the Apollo core.
        /// </summary>
        private readonly IHoldSystemInformation m_SystemInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFacade"/> class.
        /// </summary>
        /// <param name="service">
        /// The user interface service that handles the communication with the rest of the system.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="service"/> is <see langword="null" />.
        /// </exception>
        internal ApplicationFacade(IUserInterfaceService service)
        {
            {
                Enforce.Argument(() => service);
            }

            m_Service = service;

            // Initialize the system information object.
            // @todo: Setup a proper way to get the system information. At the moment the
            //        startup time is incorrect.
            m_SystemInformation = new SystemInformation(
                () => DateTimeOffset.Now,
                () => new SystemInformationStorage() { StartupTime = DateTimeOffset.Now });
        }

        /// <summary>
        /// Shuts the application down.
        /// </summary>
        public void Shutdown()
        {
            var context = new ShutdownApplicationContext();

            Debug.Assert(m_Service.Contains(ShutdownApplicationCommand.CommandId), "A command has gone missing.");
            m_Service.Invoke(ShutdownApplicationCommand.CommandId, context);
        }

        /// <summary>
        /// Gets the object that provides information about the application status.
        /// </summary>
        /// <value>The object that provides information about the application status.</value>
        public IHoldSystemInformation ApplicationStatus
        {
            get
            {
                return m_SystemInformation;
            }
        }

        /// <summary>
        /// Registers the notification.
        /// </summary>
        /// <param name="name">The name of the notification.</param>
        /// <param name="callback">The callback method that is called when the notification is activated.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="callback"/> is <see langword="null" />.
        /// </exception>
        public void RegisterNotification(NotificationName name, Action<INotificationArguments> callback)
        {
            m_Service.RegisterNotification(name, callback);
        }
    }
}
