// -----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Core.Properties;
using Apollo.Core.Utils.Licensing;
using Apollo.Utils.Commands;
using Autofac.Core;
using Lokad;

namespace Apollo.Core.UserInterfaces
{
    /// <summary>
    /// Defines the <see cref="KernelService"/> that handles the User Interface interaction with the kernel.
    /// </summary>
    [AutoLoad]
    internal sealed partial class UserInterfaceService : MessageEnabledKernelService, IHaveServiceDependencies, IUserInterfaceService
    {
        /// <summary>
        /// The collection of notifications that must be passed on to the user interface.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to store the action somehow ...")]
        private readonly Dictionary<NotificationName, Action<INotificationArguments>> m_Notifications = new Dictionary<NotificationName, Action<INotificationArguments>>();

        /// <summary>
        /// The container that stores all the commands for this service.
        /// </summary>
        private readonly ICommandContainer m_Commands;

        /// <summary>
        /// The collection of DnsNames.
        /// </summary>
        private readonly IDnsNameConstants m_DnsNames;

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
        /// Initializes a new instance of the <see cref="UserInterfaceService"/> class.
        /// </summary>
        /// <param name="commands">The container that stores all the commands.</param>
        /// <param name="dnsNames">The object that stores all the <see cref="DnsName"/> objects for the application.</param>
        /// <param name="notificationNames">The object that stores all the <see cref="NotificationName"/> objects for the application.</param>
        /// <param name="processor">The object that handles the incoming messages.</param>
        /// <param name="licenseValidationStorage">The object that stores the validity of the license.</param>
        /// <param name="onStartService">The method that provides the DI module.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="commands"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dnsNames"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="notificationNames"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="processor"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="licenseValidationStorage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="onStartService"/> is <see langword="null"/>.
        /// </exception>
        public UserInterfaceService(
            ICommandContainer commands, 
            IDnsNameConstants dnsNames, 
            INotificationNameConstants notificationNames, 
            IHelpMessageProcessing processor, 
            IValidationResultStorage licenseValidationStorage,
            Action<IModule> onStartService)
            : base(processor)
        {
            {
                Enforce.Argument(() => commands);
                Enforce.Argument(() => dnsNames);
                Enforce.Argument(() => notificationNames);
                Enforce.Argument(() => licenseValidationStorage);
                Enforce.Argument(() => onStartService);
            }

            Name = dnsNames.AddressOfUserInterface;

            m_DnsNames = dnsNames;
            m_NotificationNames = notificationNames;
            m_LicenseValidationStorage = licenseValidationStorage;
            m_OnStartService = onStartService;

            m_Commands = commands;
            {
                m_Commands.Add(
                    CheckApplicationCanShutdownCommand.CommandId, 
                    () => new CheckApplicationCanShutdownCommand(m_DnsNames.AddressOfKernel, SendMessageWithResponse));

                m_Commands.Add(
                    ShutdownApplicationCommand.CommandId,
                    () => new ShutdownApplicationCommand(m_DnsNames.AddressOfKernel, SendMessageWithResponse));
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
                Enforce.With<ArgumentException>(IsFullyFunctional, Resources_NonTranslatable.Exceptions_Messages_ServicesIsNotFullyFunctional, StartupState);
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
                Enforce.With<ArgumentException>(IsFullyFunctional, Resources_NonTranslatable.Exceptions_Messages_ServicesIsNotFullyFunctional, StartupState);
            }

            m_Commands.Invoke(id, context);
        }

        #endregion

        #region Implementation of IHaveServiceDependencies

        /// <summary>
        /// Returns a set of types indicating which services need to be present
        /// for the current service to be functional.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of 
        ///     services which this service requires to be functional.
        /// </returns>
        public IEnumerable<Type> ServicesToBeAvailable()
        {
            return new Type[] 
                { 
                    typeof(LogSink),
                };
        }

        /// <summary>
        /// Returns a set of types indicating which services the current service
        /// needs to be linked to in order to be functional.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of services
        ///     on which this service depends.
        /// </returns>
        public IEnumerable<Type> ServicesToConnectTo()
        {
            return new[] 
                { 
                    typeof(IMessagePipeline),
                };
        }

        /// <summary>
        /// Provides one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        public void ConnectTo(KernelService dependency)
        {
            var pipeline = dependency as IMessagePipeline;
            if (pipeline != null)
            {
                ConnectToMessageSink(pipeline);
            }
        }

        /// <summary>
        /// Disconnects from one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        public void DisconnectFrom(KernelService dependency)
        {
            var pipeline = dependency as IMessagePipeline;
            if (pipeline != null)
            {
                DisconnectFromMessageSink(pipeline);
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
        public bool IsConnectedToAllDependencies
        {
            get
            {
                return IsConnectedToPipeline;
            }
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
        /// Performs initialization prior to setting up the message handling.
        /// </summary>
        protected override void PreMessageInitializeStartup()
        {
            InitializeDependencyInjectionContainer();
        }

        private void InitializeDependencyInjectionContainer()
        {
            m_OnStartService(new UserInterfaceModule(this));
        }

        /// <summary>
        /// Performs un-initialization prior to unregistering from the message handling.
        /// </summary>
        protected override void PreMessageUnregisterStopAction()
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
                // Log the fact that we failed
                SendMessage(
                    m_DnsNames.AddressOfLogger, 
                    new LogEntryRequestMessage(
                        new LogMessage(
                            Name.ToString(),
                            LevelToLog.Error,
                            string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.UserInterrface_LogMessage_DisconnectPreActionFailed, e)),
                        LogType.Debug), 
                    MessageId.None);

                // Now get the hell out of here.
                throw;
            }
        }

        /// <summary>
        /// Logs the error messages coming from the <see cref="MessageProcessingAssistance"/>.
        /// </summary>
        /// <param name="e">The exception that should be logged.</param>
        protected override void LogErrorMessage(Exception e)
        {
            var message = string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.UserInterface_LogMessage_MessageSendExceptionOccurred, e);
            SendMessage(
                m_DnsNames.AddressOfLogger,
                new LogEntryRequestMessage(
                    new LogMessage(
                        Name.ToString(),
                        LevelToLog.Info,
                        message),
                    LogType.Debug),
                MessageId.None);
        }

        #endregion
    }
}