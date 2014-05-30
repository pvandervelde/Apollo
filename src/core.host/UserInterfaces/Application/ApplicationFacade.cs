//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Apollo.Core.Host.Properties;
using Lokad;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

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
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// A flag that indicates if the application is shutting down.
        /// </summary>
        private volatile bool m_IsApplicationShuttingDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFacade"/> class.
        /// </summary>
        /// <param name="service">
        /// The user interface service that handles the communication with the rest of the system.
        /// </param>
        /// <param name="notificationNames">The object that defines the application level notification names.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="service"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationNames"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        internal ApplicationFacade(IUserInterfaceService service, INotificationNameConstants notificationNames, SystemDiagnostics diagnostics)
        {
            {
                Enforce.Argument(() => service);
            }

            m_Service = service;
            m_Service.RegisterNotification(notificationNames.SystemShuttingDown, HandleApplicationShutdown);

            // Initialize the system information object.
            m_SystemInformation = new SystemInformation(
                () => DateTimeOffset.Now,
                () => new SystemInformationStorage() { StartupTime = DateTimeOffset.Now });

            m_Diagnostics = diagnostics;
        }

        private void HandleApplicationShutdown(INotificationArguments arguments)
        {
            m_IsApplicationShuttingDown = true;
        }

        /// <summary>
        /// Shuts the application down.
        /// </summary>
        public void Shutdown()
        {
            if (m_IsApplicationShuttingDown)
            {
                return;
            }

            m_Diagnostics.Log(
                LevelToLog.Trace, 
                HostConstants.LogPrefix,
                Resources.UserInterface_LogMessage_ShuttingDown);

            var context = new ShutdownApplicationContext();

            try
            {
                Debug.Assert(m_Service.Contains(ShutdownApplicationCommand.CommandId), "A command has gone missing.");
                m_Service.Invoke(ShutdownApplicationCommand.CommandId, context);
            }
            catch (ArgumentException e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    HostConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.UserInterface_LogMessage_ShutDownFailed_WithError,
                        e));
            }
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
            if (m_IsApplicationShuttingDown)
            {
                return;
            }

            m_Service.RegisterNotification(name, callback);
        }
    }
}
