//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="DataDownloadRequestMessage"/>.
    /// </summary>
    internal sealed class DataDownloadProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The collection that holds all the uploads.
        /// </summary>
        private readonly WaitingUploads m_Uploads;

        /// <summary>
        /// The object that handles communication with remote endpoints.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// The function that logs messages.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDownloadProcessAction"/> class.
        /// </summary>
        /// <param name="uploads">The object that stores the files that need uploading.</param>
        /// <param name="layer">The object that handles the communication with remote endpoints.</param>
        /// <param name="logger">The function that handles the logging of messages.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks on.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public DataDownloadProcessAction(
            WaitingUploads uploads,
            ICommunicationLayer layer,
            Action<LogSeverityProxy, string> logger,
            TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => uploads);
                Enforce.Argument(() => layer);
                Enforce.Argument(() => logger);
            }

            m_Uploads = uploads;
            m_Layer = layer;
            m_Logger = logger;
            m_Scheduler = scheduler;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(DataDownloadRequestMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as DataDownloadRequestMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            if (!m_Uploads.HasRegistration(msg.Token))
            {
                m_Logger(
                   LogSeverityProxy.Warning,
                   string.Format(
                       CultureInfo.InvariantCulture,
                       "No file was registered for uploading with token {0}",
                       msg.Token));
                
                SendMessage(msg, new FailureMessage(m_Layer.Id, msg.Id));
                return;
            }

            var filePath = m_Uploads.Deregister(msg.Token);
            var tokenSource = new CancellationTokenSource();
            var task = m_Layer.UploadData(filePath, msg.TransferInformation, null, tokenSource.Token, m_Scheduler);

            ICommunicationMessage returnMsg = null;
            try
            {
                task.Wait();
                returnMsg = new SuccessMessage(m_Layer.Id, msg.Id);
            }
            catch (AggregateException)
            {
                returnMsg = new FailureMessage(m_Layer.Id, msg.Id);
                m_Uploads.Reregister(msg.Token, filePath);
            }

            SendMessage(msg, returnMsg);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "This method should not kill the application, it should notify the other side of the connection of failure if possible")]
        private void SendMessage(ICommunicationMessage msg, ICommunicationMessage returnMsg)
        {
            try
            {
                m_Layer.SendMessageTo(msg.OriginatingEndpoint, returnMsg);
            }
            catch (Exception e)
            {
                HandleCommandExecutionFailure(msg, e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "There is no point crashing the current app without being able to notify the other side of the channel.")]
        private void HandleCommandExecutionFailure(ICommunicationMessage msg, Exception e)
        {
            try
            {
                m_Logger(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Error while sending endpoint information. Exception is: {0}",
                        e));
                m_Layer.SendMessageTo(msg.OriginatingEndpoint, new FailureMessage(m_Layer.Id, msg.Id));
            }
            catch (Exception errorSendingException)
            {
                m_Logger(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Error while trying to send process failure. Exception is: {0}",
                        errorSendingException));
            }
        }
    }
}
