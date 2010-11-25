//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Core.Properties;

namespace Apollo.Core.UserInterfaces
{
    /// <content>
    /// Defines the methods used for working with messages.
    /// </content>
    internal sealed partial class UserInterfaceService
    {
        /// <summary>
        /// Stores the different message types and their connected actions.
        /// </summary>
        /// <param name="processor">The processor.</param>
        protected override void StoreMessageActions(IHelpMessageProcessing processor)
        {
            // Define the response to a shutdown capability request
            processor.RegisterAction(
                typeof(ServiceShutdownCapabilityRequestMessage),
                message =>
                {
                    var request = message.Body as ServiceShutdownCapabilityRequestMessage;
                    Debug.Assert(request != null, "Message type mapping failed for ServiceShutdownCapabilityRequestMessage");

                    HandleShutdownCapabilityRequest(message.Header.Sender, message.Header.Id);
                });

            // Define the response to a shutdown capability request
            processor.RegisterAction(
                typeof(ApplicationStartupCompleteMessage),
                message =>
                {
                    HandleStartupCompleteMessage();
                });
        }

        private void HandleShutdownCapabilityRequest(DnsName originalSender, MessageId id)
        {
            Debug.Assert(
                IsFullyFunctional,
                string.Format("The service tried to perform an action but wasn't in the correct startup state. The actual state was: {0}", GetStartupState()));

            // @todo: Check with the UI if we can shutdown. This should only be a UI value, not the system value.
            // For now just send a message saying that we can shutdown.
            SendMessage(originalSender, new ServiceShutdownCapabilityResponseMessage(CanUserInterfaceShutDown()), id);
        }

        private bool CanUserInterfaceShutDown()
        {
            if (!m_Notifications.ContainsKey(m_NotificationNames.CanSystemShutdown))
            {
                return true;
            }

            var action = m_Notifications[m_NotificationNames.CanSystemShutdown];
            try
            {
                var arg = new ShutdownCapabilityArguments();
                action(arg);
                
                return arg.CanShutdown;
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
                            string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.UserInterrface_LogMessage_StartupCompleteNotificationFailed, e)),
                        LogType.Debug),
                    MessageId.None);

                // Now get the hell out of here.
                throw;
            }
        }

        private void HandleStartupCompleteMessage()
        {
            // @todo: We can store the start-up time here. Effectively we're not started until we get this
            //        message anyway so storing it in the UI service sounds reasonable
            Debug.Assert(
                IsFullyFunctional,
                string.Format("The service tried to perform an action but wasn't in the correct startup state. The actual state was: {0}", GetStartupState()));

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
                // Log the fact that we failed
                SendMessage(
                    m_DnsNames.AddressOfLogger,
                    new LogEntryRequestMessage(
                        new LogMessage(
                            Name.ToString(),
                            LevelToLog.Error,
                            string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.UserInterrface_LogMessage_StartupCompleteNotificationFailed, e)),
                        LogType.Debug),
                    MessageId.None);

                // Now get the hell out of here.
                throw;
            }
        }
    }
}
