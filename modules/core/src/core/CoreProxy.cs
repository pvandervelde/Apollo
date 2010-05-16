//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Core.Properties;
using Apollo.Utils.Commands;
using Lokad;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the service that is used by the <c>Kernel</c> to 
    /// interact with the other services on an equal basis.
    /// </summary>
    /// <remarks>
    /// The <c>CoreProxy</c> is automatically loaded by the <see cref="Kernel"/>
    /// so there is no need to request the bootstrapper to load it.
    /// </remarks>
    [AutoLoad]
    internal sealed partial class CoreProxy : MessageEnabledKernelService, IInvokeCommands, IHaveServiceDependencies
    {
        /// <summary>
        /// The <see cref="Kernel"/> that owns this proxy.
        /// </summary>
        private readonly IKernel m_Owner;

        /// <summary>
        /// The collection of DnsNames.
        /// </summary>
        private readonly IDnsNameConstants m_DnsNames;

        /// <summary>
        /// The container that stores all the commands for this service.
        /// </summary>
        private readonly ICommandContainer m_Commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreProxy"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Kernel"/> to which this proxy is linked.</param>
        /// <param name="commands">The container that stores all the commands.</param>
        /// <param name="processor">The object that handles the incoming messages.</param>
        /// <param name="dnsNames">The object that stores all the <see cref="DnsName"/> objects for the application.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="owner"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="commands"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="processor"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dnsNames"/> is <see langword="null"/>.
        /// </exception>
        public CoreProxy(
            IKernel owner, 
            ICommandContainer commands, 
            IHelpMessageProcessing processor,
            IDnsNameConstants dnsNames)
            : base(processor)
        {
            {
                Enforce.Argument(() => owner);
                Enforce.Argument(() => commands);
                Enforce.Argument(() => dnsNames);
            }

            Name = dnsNames.AddressOfKernel;
            m_Owner = owner;
            m_DnsNames = dnsNames;

            m_Commands = commands;
            {
                m_Commands.Add(CheckServicesCanShutdownCommand.CommandId, () => new CheckServicesCanShutdownCommand(SendMessageWithResponse));
                m_Commands.Add(LogMessageForKernelCommand.CommandId, () => new LogMessageForKernelCommand(m_DnsNames.AddressOfLogger, SendMessage));
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
        public void Invoke(CommandId id)
        {
            if (!IsFullyFunctional)
            {
                return;
            }

            m_Commands.Invoke(id);
        }

        /// <summary>
        /// Invokes the command with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <param name="context">The context that will be passed to the command as it is invoked.</param>
        public void Invoke(CommandId id, ICommandContext context)
        {
            if (!IsFullyFunctional)
            {
                return;
            }

            m_Commands.Invoke(id, context);
        }

        #endregion

        #region IHaveServiceDependencies

        /// <summary>
        /// Returns a set of types indicating which services need to be present
        /// for the current service to be functional.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of 
        ///     services which this service requires to be functional. Currently
        ///     there are no services which the <c>CoreProxy</c> requires.
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
        /// An <see cref="IEnumerable{Type}"/> which contains the types of services
        /// on which this service depends. Currently that is only the
        /// <see cref="MessagePipeline"/> service.
        /// </returns>
        public IEnumerable<Type> ServicesToConnectTo()
        {
            return new[] 
                { 
                    typeof(IMessagePipeline) 
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

        #region Overrides of MessageEnabledKernelService

        /// <summary>
        /// Logs the error messages coming from the <see cref="MessageProcessingAssistance"/>.
        /// </summary>
        /// <param name="e">The exception that should be logged.</param>
        protected override void LogErrorMessage(Exception e)
        {
            var message = string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.Kernel_LogMessage_MessageSendExceptionOccurred, e);
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
