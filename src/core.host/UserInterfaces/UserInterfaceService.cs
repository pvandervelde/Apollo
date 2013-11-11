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
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.Properties;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities.Commands;
using Autofac;
using Lokad;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Apollo.Core.Host.UserInterfaces
{
    /// <summary>
    /// Defines the <see cref="KernelService"/> that handles the User Interface interaction with the kernel.
    /// </summary>
    [AutoLoad]
    internal sealed class UserInterfaceService : KernelService, IUserInterfaceService
    {
        /// <summary>
        /// The collection of notifications that must be passed on to the user interface.
        /// </summary>
        private readonly Dictionary<NotificationName, List<Action<INotificationArguments>>> m_Notifications =
            new Dictionary<NotificationName, List<Action<INotificationArguments>>>();

        /// <summary>
        /// The IOC container for the application.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// The object that provides the diagnostic methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The container that stores all the commands for this service.
        /// </summary>
        private readonly ICommandContainer m_Commands;

        /// <summary>
        /// The collection of NotificationNames.
        /// </summary>
        private readonly INotificationNameConstants m_NotificationNames;

        /// <summary>
        /// The action which is executed when the service is started.
        /// </summary>
        private readonly Action<IContainer> m_OnStartService;

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
        /// <param name="container">The container that stores all the references for the application.</param>
        /// <param name="onStartService">
        ///     The method that stores the IOC container that contains the references for the entire application.
        /// </param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="container"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="onStartService"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public UserInterfaceService(IContainer container, Action<IContainer> onStartService, SystemDiagnostics diagnostics)
            : base(diagnostics)
        {
            {
                Enforce.Argument(() => container);
                Enforce.Argument(() => onStartService);
            }

            m_Container = container;
            m_NotificationNames = container.Resolve<INotificationNameConstants>();
            m_Diagnostics = container.Resolve<SystemDiagnostics>();
            m_OnStartService = onStartService;

            m_Commands = container.Resolve<ICommandContainer>();
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
                Enforce.With<ArgumentException>(
                    IsFullyFunctional, 
                    Resources.Exceptions_Messages_ServicesIsNotFullyFunctional, 
                    StartupState);
            }

            Diagnostics.Log(
                LevelToLog.Trace, 
                HostConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.UserInterface_LogMessage_InvokingCommand_WithId,
                    id));

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
                Enforce.With<ArgumentException>(
                    IsFullyFunctional, 
                    Resources.Exceptions_Messages_ServicesIsNotFullyFunctional, 
                    StartupState);
            }

            Diagnostics.Log(
                LevelToLog.Trace,
                HostConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.UserInterface_LogMessage_InvokingCommand_WithId,
                    id));

            m_Commands.Invoke(id, context);
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
            {
                Enforce.Argument(() => name);
                Enforce.Argument(() => callback);
            }

            if (!m_Notifications.ContainsKey(name))
            {
                m_Notifications.Add(name, new List<Action<INotificationArguments>>());
            }

            var list = m_Notifications[name];
            list.Add(callback);
        }

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
            return new[] 
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

            var list = m_Notifications[m_NotificationNames.StartupComplete];
            foreach (var action in list)
            {
                try
                {
                    action(null);
                }
                catch (Exception e)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Error,
                        HostConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.UserInterrface_LogMessage_StartupCompleteNotificationFailed,
                            e));
                }
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
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new UserInterfaceModule(this));
            }

            builder.Update(m_Container);
            m_OnStartService(m_Container);
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

            var list = m_Notifications[m_NotificationNames.SystemShuttingDown];
            foreach (var action in list)
            {
                try
                {
                    action(null);
                }
                catch (Exception e)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Error,
                        HostConstants.LogPrefix,
                        string.Format(CultureInfo.InvariantCulture, Resources.UserInterrface_LogMessage_DisconnectPreActionFailed, e));
                }
            }
        }
    }
}
