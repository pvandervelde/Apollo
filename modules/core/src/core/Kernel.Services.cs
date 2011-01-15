﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Messaging;

namespace Apollo.Core
{
    /// <content>
    /// Holds all the service interaction related methods on the 
    /// <c>Kernel</c>.
    /// </content>
    internal sealed partial class Kernel
    {
        /// <summary>
        /// Sends a message to all services to indicate that the start-up process has completed.
        /// </summary>
        private void SendStartupCompleteMessage()
        {
            // Check all services
            var coreProxy = m_Services[typeof(CoreProxy)] as CoreProxy;
            Debug.Assert(coreProxy != null, "Stored an incorrect service under the CoreProxy type.");

            var servicesToNotify = from pair in m_Services
                                   select pair.Value as ISendMessages into nameObject
                                   where (nameObject != null) && (!ReferenceEquals(nameObject, coreProxy))
                                   select nameObject.Name;

            Debug.Assert(coreProxy.Contains(SendMessageForKernelCommand.CommandId), "A command has gone missing.");
            foreach (var service in servicesToNotify)
            {
                var context = new SendMessageForKernelContext(service, new ApplicationStartupCompleteMessage());
                coreProxy.Invoke(SendMessageForKernelCommand.CommandId, context);
            }
        }

        /// <summary>
        /// Determines whether this instance can shutdown.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if this instance can shutdown; otherwise, <see langword="false"/>.
        /// </returns>
        /// <design>
        /// It is possible to cache the last answer and use it the next time. However at the moment
        /// we expect that it will take very little time to perform the message sending given that
        /// a) all messages are send in parallel and b) message sending is essentially method
        /// invokation, not actual networked message sending.
        /// </design>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanShutdown()
        {
            // Check all services
            var coreProxy = m_Services[typeof(CoreProxy)] as CoreProxy;
            Debug.Assert(coreProxy != null, "Stored an incorrect service under the CoreProxy type.");

            var servicesToCheck = from pair in m_Services
                                  select pair.Value as ISendMessages into nameObject
                                  where (nameObject != null) && (!ReferenceEquals(nameObject, coreProxy))
                                  select nameObject.Name;
            var context = new CheckCanServicesShutdownContext(servicesToCheck);

            Debug.Assert(coreProxy.Contains(CheckServicesCanShutdownCommand.CommandId), "A command has gone missing.");
            coreProxy.Invoke(CheckServicesCanShutdownCommand.CommandId, context);
            return context.Result;
        }

        /// <summary>
        /// Shuts the application down.
        /// </summary>
        /// <design>
        /// Note that this method does not check if it is safe to shut the application down. It is assumed that
        /// there are no more objections against shutting down once this method is reached. Under normal circumstances
        /// this method should only be called by the <c>CoreProxy</c> which will perform the necessary checks
        /// to ensure that all objections against shutdown are heard.
        /// </design>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The shutdown must proceede even if a service throws an unknown exception.")]
        public void Shutdown()
        {
            // Indicate that the kernel is stopping.
            m_State = StartupState.Stopping;

            // In order to keep this flexible we will need to sort the services
            // so that the startup order guarantuees that each service will have 
            // its dependencies and requirements running before it does.
            // Obviously this is prone to cyclic loops ...
            var startupOrder = DetermineServiceStartupOrder();

            // Reverse the order so that we move from most dependent 
            // to least dependent
            startupOrder.Reverse();

            // Stop all the services and disconnect them.
            foreach (var service in startupOrder)
            {
                // Grab the actual current service so that we can put it in the
                // lambda expression without having it wiped or replaced on us
                try
                {
                    // Start the service
                    service.Stop();
                }
                catch
                {
                    // An exception occured. Ignore it and move on
                    // we're about to destroy the appdomain the service lives in.
                }
            }

            // @Todo: Fix the fact that we can't nuke the AppDomain on shutdown
            // Cannot remove the services, because the only reason the individual
            // AppDomains exist is that the kernel is holding on to a reference to the
            // services. If we remove the service then the AppDomain unloads. That would
            // normally be fine except that the callstack will very likely have some
            // code that runs through all these services (e.g. the CoreProxy and the 
            // MessagePipeline, which are essential in calling the Kernel.Shutdown method)
            // In that case we'll unload the AppDomain while there is a thread active in
            // that AppDomain. This is not appreciated.
            // foreach (var service in startupOrder)
            // {
            //     Remove the service
            //     Uninstall(service);
            // }
            //
            // If we get here then we have to have finished the
            // startup process, which means we're actually running.
            m_State = StartupState.Stopped;
        }
    }
}