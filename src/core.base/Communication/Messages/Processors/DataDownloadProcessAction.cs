//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
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
        /// Initializes a new instance of the <see cref="DataDownloadProcessAction"/> class.
        /// </summary>
        /// <param name="uploads">The object that stores the files that need uploading.</param>
        /// <param name="layer">The object that handles the communication with remote endpoints.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        public DataDownloadProcessAction(WaitingUploads uploads, ICommunicationLayer layer)
        {
            {
                Enforce.Argument(() => uploads);
                Enforce.Argument(() => layer);
            }

            m_Uploads = uploads;
            m_Layer = layer;
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
                m_Layer.SendMessageTo(msg.OriginatingEndpoint, new FailureMessage(m_Layer.Id, msg.Id));
                return;
            }

            var filePath = m_Uploads.Deregister(msg.Token);
            var tokenSource = new CancellationTokenSource();
            var task = m_Layer.UploadData(filePath, msg.TransferInformation, tokenSource.Token);

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

            m_Layer.SendMessageTo(msg.OriginatingEndpoint, returnMsg);
        }
    }
}
