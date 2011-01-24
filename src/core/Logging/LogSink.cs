//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Messaging;
using Apollo.Core.Properties;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Defines a <see cref="KernelService"/> that handles logging for the system.
    /// </summary>
    [PrivateBinPathRequirements(PrivateBinPathOption.Log)]
    internal sealed partial class LogSink : MessageEnabledKernelService, ILogSink, IHaveServiceDependencies
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogSink"/> class.
        /// </summary>
        /// <param name="processor">The message processor.</param>
        /// <param name="configuration">The log configuration.</param>
        /// <param name="debugTemplate">The debug template.</param>
        /// <param name="commandTemplate">The command template.</param>
        /// <param name="fileConstants">The object containing constant values describing file and file paths.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="processor"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="configuration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="debugTemplate"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="commandTemplate"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="fileConstants"/> is <see langword="null"/>.
        /// </exception>
        public LogSink(
            IHelpMessageProcessing processor, 
            ILoggerConfiguration configuration, 
            DebugLogTemplate debugTemplate, 
            CommandLogTemplate commandTemplate,
            IFileConstants fileConstants)
            : base(processor)
        {
            {
                Enforce.Argument(() => configuration);
                Enforce.Argument(() => debugTemplate);
                Enforce.Argument(() => commandTemplate);
                Enforce.Argument(() => fileConstants);
            }

            Name = new DnsName(GetType().FullName);

            // Add the loggers to the collection
            {
                m_Loggers.Add(LogType.Debug, new Logger(configuration, debugTemplate, fileConstants));
                m_Loggers.Add(LogType.Command, new Logger(configuration, commandTemplate, fileConstants));
            }
        }

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
            return new Type[0];
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
            return new[] { typeof(IMessagePipeline) };
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

        #region Overrides

        /// <summary>
        /// Performs service startup actions prior to setting up the message handling.
        /// </summary>
        protected override void PreMessageInitializeStartup()
        {
            LogWithoutVerifying(LogType.Debug, new LogMessage(GetType().FullName, LevelToLog.Info, Resources_NonTranslatable.LogSink_LogMessage_LoggersStarted));
        }

        /// <summary>
        /// Performs service shutdown actions after unregistering from the message handling.
        /// </summary>
        protected override void PostMessageUnregisterStopAction()
        {
            LogWithoutVerifying(LogType.Debug, new LogMessage(GetType().FullName, LevelToLog.Info, Resources_NonTranslatable.LogSink_LogMessage_LoggersStopped));

            // Inform all the loggers that the system is being stopped. This allows
            // the loggers to flush all the buffers.
            foreach (var pair in m_Loggers)
            {
                pair.Value.Stop();
            }
        }

        /// <summary>
        /// Logs the error messages coming from the <see cref="MessageProcessingAssistance"/>.
        /// </summary>
        /// <param name="e">The exception that should be logged.</param>
        protected override void LogErrorMessage(Exception e)
        {
            if (IsFullyFunctional)
            {
                var message = string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.LogSink_LogMessage_MessageSendExceptionOccurred, e);
                LogWithoutVerifying(LogType.Debug, new LogMessage(Name.ToString(), LevelToLog.Info, message));
            }
        }

        #endregion
    }
}
