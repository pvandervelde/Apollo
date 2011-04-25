// -----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Projects;
using Apollo.Core.Properties;
using Apollo.Core.UserInterfaces.Projects;
using Apollo.Core.Utils.Licensing;
using Apollo.Utils.Commands;
using Apollo.Utils.Logging;
using Autofac.Core;
using Lokad;

namespace Apollo.Core.UserInterfaces
{
    /// <summary>
    /// Defines the <see cref="KernelService"/> that handles the User Interface interaction with the kernel.
    /// </summary>
    [AutoLoad]
    internal sealed partial class UserInterfaceService : KernelService, IUserInterfaceService
    {
        /// <summary>
        /// The collection of notifications that must be passed on to the user interface.
        /// </summary>
        private readonly Dictionary<NotificationName, Action<INotificationArguments>> m_Notifications =
            new Dictionary<NotificationName, Action<INotificationArguments>>();

        /// <summary>
        /// The logger that logs the debug information.
        /// </summary>
        private readonly ILogger m_Logger;

        /// <summary>
        /// The container that stores all the commands for this service.
        /// </summary>
        private readonly ICommandContainer m_Commands;

        /// <summary>
        /// The collection of NotificationNames.
        /// </summary>
        private readonly INotificationNameConstants m_NotificationNames;

        /// <summary>
        /// The object that stores the validity of the license.
        /// </summary>
        private readonly IValidationResultStorage m_LicenseValidationStorage;

        /// <summary>
        /// The action which is executed when the service is started.
        /// </summary>
        private readonly Action<IModule> m_OnStartService;

        /// <summary>
        /// The service that handles the connection to the <c>Kernel</c>.
        /// </summary>
        private CoreProxy m_Core;

        /// <summary>
        /// The service which handles all the project requests.
        /// </summary>
        private ProjectService m_Projects;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInterfaceService"/> class.
        /// </summary>
        /// <param name="commands">The container that stores all the commands.</param>
        /// <param name="notificationNames">The object that stores all the <see cref="NotificationName"/> objects for the application.</param>
        /// <param name="licenseValidationStorage">The object that stores the validity of the license.</param>
        /// <param name="logger">The <see cref="ILogger"/> that logs the debug information for the current service.</param>
        /// <param name="onStartService">The method that provides the DI module.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="commands"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="notificationNames"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="licenseValidationStorage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="onStartService"/> is <see langword="null"/>.
        /// </exception>
        public UserInterfaceService(
            ICommandContainer commands,
            INotificationNameConstants notificationNames,
            IValidationResultStorage licenseValidationStorage,
            ILogger logger,
            Action<IModule> onStartService)
            : base()
        {
            {
                Enforce.Argument(() => commands);
                Enforce.Argument(() => notificationNames);
                Enforce.Argument(() => licenseValidationStorage);
                Enforce.Argument(() => logger);
                Enforce.Argument(() => onStartService);
            }

            m_NotificationNames = notificationNames;
            m_LicenseValidationStorage = licenseValidationStorage;
            m_OnStartService = onStartService;
            m_Logger = logger;

            m_Commands = commands;
            {
                m_Commands.Add(
                    ShutdownApplicationCommand.CommandId,
                    () => new ShutdownApplicationCommand(() => m_Core.Shutdown()));

                m_Commands.Add(
                    CreateProjectCommand.CommandId,
                    () => new CreateProjectCommand(
                        () => 
                        {
                            m_Projects.CreateNewProject();
                            return m_Projects.Current;
                        }));

                m_Commands.Add(
                   LoadProjectCommand.CommandId,
                   () => new LoadProjectCommand(
                       persistenceInformation =>
                       {
                           m_Projects.LoadProject(persistenceInformation);
                           return m_Projects.Current;
                       }));

                m_Commands.Add(
                   UnloadProjectCommand.CommandId,
                   () => new UnloadProjectCommand(() => m_Projects.UnloadProject()));
            }
        }

        #region Implementation of IHaveCommands

        /// <summary>
        /// Determines whether a command with the specified Id is stored.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <returns>
        ///     <see langword="true"/> if a command with the specified ID is stored; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Contains(CommandId id)
        {
            return m_Commands.Contains(id);
        }

        #endregion

        #region Implementation of IInvokeCommands

        /// <summary>
        /// Invokes the command with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the service is not fully functional.
        /// </exception>
        public void Invoke(CommandId id)
        {
            {
                Enforce.With<ArgumentException>(IsFullyFunctional, Resources_NonTranslatable.Exception_Messages_ServicesIsNotFullyFunctional, StartupState);
            }

            m_Commands.Invoke(id);
        }

        /// <summary>
        /// Invokes the command with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <param name="context">The context that will be passed to the command as it is invoked.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the service is not fully functional.
        /// </exception>
        public void Invoke(CommandId id, ICommandContext context)
        {
            {
                Enforce.With<ArgumentException>(IsFullyFunctional, Resources_NonTranslatable.Exception_Messages_ServicesIsNotFullyFunctional, StartupState);
            }

            m_Commands.Invoke(id, context);
        }

        #endregion

        #region Implementation of IUserInterfaceService

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
            {
                Enforce.Argument(() => name);
                Enforce.Argument(() => callback);
            }

            if (m_Notifications.ContainsKey(name))
            {
                throw new DuplicateNotificationException(name);
            }

            m_Notifications.Add(name, callback);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a set of types indicating which services the current service
        /// needs to be linked to in order to be functional.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of services
        ///     on which this service depends.
        /// </returns>
        public override IEnumerable<Type> ServicesToConnectTo()
        {
            return new Type[] 
                { 
                    typeof(CoreProxy),
                    typeof(ProjectService),
                };
        }

        /// <summary>
        /// Provides one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        public override void ConnectTo(KernelService dependency)
        {
            var core = dependency as CoreProxy;
            if (core != null)
            {
                m_Core = core;
                m_Core.OnStartupComplete += OnStartupComplete;
            }

            var projects = dependency as ProjectService;
            if (projects != null)
            {
                m_Projects = projects;
            }
        }

        private void OnStartupComplete(object sender, ApplicationStartupEventArgs args)
        {
            // @todo: We can store the start-up time here. Effectively we're not started until we get this
            //        message anyway so storing it in the UI service sounds reasonable
            Debug.Assert(
                IsFullyFunctional,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "The service tried to perform an action but wasn't in the correct startup state. The actual state was: {0}",
                    StartupState));

            if (!m_Notifications.ContainsKey(m_NotificationNames.StartupComplete))
            {
                return;
            }

            var action = m_Notifications[m_NotificationNames.StartupComplete];
            try
            {
                action(null);
            }
            catch (Exception e)
            {
                LogMessage(
                    LevelToLog.Error,
                    string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.UserInterrface_LogMessage_StartupCompleteNotificationFailed, e));

                throw;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected to all dependencies.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this instance is connected to all dependencies; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool IsConnectedToAllDependencies
        {
            get
            {
                return m_Projects != null;
            }
        }

        /// <summary>
        /// Starts the User Interface service.
        /// </summary>
        protected override void StartService()
        {
            m_OnStartService(new UserInterfaceModule(this));
        }

        /// <summary>
        /// Stops the user interface service.
        /// </summary>
        protected override void StopService()
        {
            if (!m_Notifications.ContainsKey(m_NotificationNames.SystemShuttingDown))
            {
                throw new MissingNotificationActionException(m_NotificationNames.SystemShuttingDown);
            }

            var action = m_Notifications[m_NotificationNames.SystemShuttingDown];
            try
            {
                action(null);
            }
            catch (Exception e)
            {
                LogMessage(
                    LevelToLog.Error,
                    string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.UserInterrface_LogMessage_DisconnectPreActionFailed, e));

                throw;
            }
        }

        #endregion

        /// <summary>
        /// Logs the messages coming from the service.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message that should be logged.</param>
        private void LogMessage(LevelToLog level, string message)
        {
            m_Logger.Log(
                new LogMessage(
                    GetType().FullName,
                    level,
                    message));
        }
    }
}