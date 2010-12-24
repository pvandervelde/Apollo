//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using Apollo.Core.Messaging;
using Apollo.Utils;

namespace Apollo.Core.Projects
{
    /// <content>
    /// Defines the methods used for working with messages.
    /// </content>
    internal sealed partial class ProjectService
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

            processor.RegisterAction(
                typeof(CreateNewProjectMessage),
                message =>
                {
                    var request = message.Body as CreateNewProjectMessage;
                    Debug.Assert(request != null, "Message type mapping failed for CreateNewProjectMessage");

                    HandleCreateNew(message.Header.Sender, message.Header.Id);
                });

            processor.RegisterAction(
                typeof(LoadProjectMessage),
                message =>
                {
                    var request = message.Body as LoadProjectMessage;
                    Debug.Assert(request != null, "Message type mapping failed for LoadProjectMessage");

                    HandleLoadProject(message.Header.Sender, message.Header.Id, request.PersistedProject);
                });

            processor.RegisterAction(
                typeof(UnloadProjectMessage),
                message =>
                {
                    var request = message.Body as UnloadProjectMessage;
                    Debug.Assert(request != null, "Message type mapping failed for UnloadProjectMessage");

                    HandleUnload();
                });

            processor.RegisterAction(
                typeof(ProjectRequestMessage),
                message =>
                {
                    var request = message.Body as ProjectRequestMessage;
                    Debug.Assert(request != null, "Message type mapping failed for ProjectRequestMessage");

                    HandleProjectRequest(message.Header.Sender, message.Header.Id);
                });
        }

        private void HandleShutdownCapabilityRequest(DnsName originalSender, MessageId id)
        {
            Debug.Assert(
                IsFullyFunctional,
                string.Format("The service tried to perform an action but wasn't in the correct startup state. The actual state was: {0}", GetStartupState()));

            // @todo: Check with the project if we can shutdown. This should only be a project value, not the system value.
            // For now just send a message saying that we can shutdown.
            SendMessage(originalSender, new ServiceShutdownCapabilityResponseMessage(true), id);
        }

        private void HandleCreateNew(DnsName originalSender, MessageId messageId)
        {
            CreateNewProject();
            HandleProjectRequest(originalSender, messageId);
        }

        private void HandleLoadProject(DnsName originalSender, MessageId messageId, IPersistenceInformation persistedProject)
        {
            LoadProject(persistedProject);
            HandleProjectRequest(originalSender, messageId);
        }

        private void HandleUnload()
        {
            UnloadProject();
        }

        private void HandleProjectRequest(DnsName originalSender, MessageId messageId)
        {
            ObjRef projectProxy = null;

            var obj = m_Current as MarshalByRefObject;
            if (obj != null)
            {
                projectProxy = RemotingServices.Marshal(obj);
            }

            SendMessage(originalSender, new ProjectRequestResponseMessage(projectProxy), messageId);
        }
    }
}
