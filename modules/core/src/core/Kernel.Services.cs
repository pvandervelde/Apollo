//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Apollo.Core
{
    /// <content>
    /// Holds all the service interaction related methods on the 
    /// <c>Kernel</c>.
    /// </content>
    internal sealed partial class Kernel
    {
        // Send ping messages and check the responses
        // System information
        // Create appdomain
        // Restart service

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
                                  let service = pair.Value
                                  where !ReferenceEquals(service, coreProxy)
                                  select service;

            return coreProxy.CanShutdownService(servicesToCheck);
        }

        /// <summary>
        /// Shuts the application down.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The shutdown must proceede even if a service throws an unknown exception.")]
        public void Shutdown()
        {
            // Indicate that the kernel is booting.
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

                // Remove the connections
                if (m_Connections.ContainsKey(service))
                {
                    var dependencyHolder = service as IHaveServiceDependencies;
                    Debug.Assert(dependencyHolder != null, "Found dependencies for non-dependent service.");

                    var dependencies = m_Connections[service];
                    foreach (var map in dependencies)
                    {
                        try
                        {
                            dependencyHolder.DisconnectFrom(map.Applied);
                        }
                        catch
                        {
                            // An exception occured. Ignore it. Try to
                            // disconnect the next service.
                        }
                    }

                    m_Connections.Remove(service);
                }

                m_Services.Remove(service.GetType());
            }

            // If we get here then we have to have finished the
            // startup process, which means we're actually running.
            m_State = StartupState.Stopped;
        }
    }
}