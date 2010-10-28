//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using Apollo.Core.Messaging;

namespace Apollo.Core.Logging
{
    /// <content>
    /// Defines the methods for the <c>LogSink</c> relating to the
    /// handling of messages from the message pipeline.
    /// </content>
    internal sealed partial class LogSink
    {
        /// <summary>
        /// Stores the different message types and their connected actions.
        /// </summary>
        /// <param name="processor">The processor.</param>
        protected override void StoreMessageActions(IHelpMessageProcessing processor)
        {
            // Define the response to an incoming log message
            processor.RegisterAction(
                typeof(LogEntryRequestMessage),
                message =>
                    {
                        var request = message.Body as LogEntryRequestMessage;
                        Debug.Assert(request != null, "Message type mapping failed for LogEntryRequestMessage");

                        Log(request.LogType, request.Message);
                    });

            // Define the response to an incoming log level change request
            processor.RegisterAction(
                typeof(LogLevelChangeRequestMessage),
                message =>
                    {
                        var request = message.Body as LogLevelChangeRequestMessage;
                        Debug.Assert(request != null, "Message type mapping failed for LogLevelChangeRequestMessage");

                        HandleLogLevelChangeRequest(request.Level);
                    });

            // Define the response to a shutdown request
            processor.RegisterAction(
                typeof(ServiceShutdownCapabilityRequestMessage),
                message =>
                    {
                        var request = message.Body as ServiceShutdownCapabilityRequestMessage;
                        Debug.Assert(request != null, "Message type mapping failed for ServiceShutdownCapabilityRequestMessage");

                        HandleShutdownCapabilityRequest(message.Header.Sender, message.Header.Id);
                    });
        }

        private void HandleLogLevelChangeRequest(LevelToLog newLevel)
        {
            Debug.Assert(
                IsFullyFunctional,
                string.Format("The service tried to perform an action but wasn't in the correct startup state. The actual state was: {0}", GetStartupState()));

            foreach (var pair in m_Loggers)
            {
                pair.Value.ChangeLevel(newLevel);
            }
        }

        private void HandleShutdownCapabilityRequest(DnsName originalSender, MessageId id)
        {
            Debug.Assert(
                IsFullyFunctional,
                string.Format("The service tried to perform an action but wasn't in the correct startup state. The actual state was: {0}", GetStartupState()));

            // Send a message saying that we can shutdown.
            SendMessage(originalSender, new ServiceShutdownCapabilityResponseMessage(true), id);
        }
    }
}
