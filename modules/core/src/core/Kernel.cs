//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Apollo.Core.Properties;
using Apollo.Utils;
using Lokad;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the core class that controls the kernel of the Apollo application.
    /// </summary>
    /// <design>
    /// The kernel will start the services in the following order:
    /// <list type="bullet">
    /// <serviceType>Message service</serviceType>
    /// <serviceType>Core</serviceType>
    /// <serviceType>User interface service</serviceType>
    /// <serviceType>License service</serviceType>
    /// <serviceType>Persistence service</serviceType>
    /// <serviceType>Log sink</serviceType>
    /// <serviceType>Timeline service</serviceType>
    /// <serviceType>Plug-in service</serviceType>
    /// <serviceType>Project service</serviceType>
    /// </list>
    /// </design>
    internal sealed partial class Kernel : MarshalByRefObject, INeedStartup
    {
        /// <summary>
        /// The collection of services that are currently known to the kernel.
        /// </summary>
        private readonly Dictionary<Type, KernelService> m_Services = new Dictionary<Type, KernelService>();

        /// <summary>
        /// The collection which tracks the connections between a service and it's dependencies.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
                Justification = "The storage requires that we link an object to a list of objects. Nested generics is an easy way to achieve this.")]
        private readonly Dictionary<KernelService, List<ConnectionMap>> m_Connections = new Dictionary<KernelService, List<ConnectionMap>>();

        /// <summary>
        /// The start-up state of the kernel.
        /// </summary>
        private StartupState m_State = StartupState.NotStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="Kernel"/> class.
        /// </summary>
        public Kernel()
        {
            // Add our own proxy to the collection of services.
            Install(new CoreProxy());
        }

        /// <summary>
        /// The event that is fired when there is an update in the startup process.
        /// </summary>
        public event EventHandler<StartupProgressEventArgs> StartupProgress;

        /// <summary>
        /// Raises the startup progress event.
        /// </summary>
        /// <param name="progress">The progress percentage.</param>
        /// <param name="mark">The progress mark.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to fire an event.")]
        private void RaiseStartupProgress(int progress, IProgressMark mark)
        {
            EventHandler<StartupProgressEventArgs> local = StartupProgress;
            if (local != null)
            { 
                local(this, new StartupProgressEventArgs(progress, mark));
            }
        }

        /// <summary>
        /// Starts the startup process.
        /// </summary>
        /// <design>
        /// <para>
        /// This method ensures that all the services are operational and
        /// have been started. Once all the services are capable of
        /// running then we return focus to the object that started the 
        /// kernel.
        /// </para>
        /// <para>
        /// The <c>Kernel.Start</c> method does not specifically start a
        /// new thread for the services to run on (although individual
        /// services may start new threads). This is done specifically because
        /// eventually the focus needs to return to the UI thread. After all
        /// this is where the user interaction will take place on. Thus there
        /// is no reason for the kernel to have it's own thread.
        /// </para>
        /// </design>
        public void Start()
        {
            // Indicate that the kernel is booting.
            m_State = StartupState.Starting;

            // In order to keep this flexible we will need to sort the services
            // so that the startup order guarantuees that each service will have 
            // its dependencies and requirements running before it does.
            // Obviously this is prone to cyclic loops ...
            var startupOrder = DetermineServiceStartupOrder();
            foreach (var service in startupOrder)
            {
                // See if the service is complete
                var dependencyHolder = service as IHaveServiceDependencies;
                if (dependencyHolder != null)
                {
                    // See if all the dependencies are there.
                    if (!dependencyHolder.IsConnectedToAllDependencies)
                    {
                        throw new KernelStartupFailedException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Exceptions_Messages_KernelStartupFailedDueToMissingServiceDependency_WithService,
                                service.GetType()));
                    }

                    // There is no need to check that all the dependencies are up and
                    // running because:
                    // - We know that all the services started prior to the current one
                    //   are running (otherwise we would have thrown).
                    // - We know that all services are sorted by dependency, the dependent
                    //   services last and the independent services first.
                    // So this means that all dependencies must be running.
                }

                // Only start the service if it hasn't already been started
                if (service.GetStartupState() != StartupState.Started)
                {
                    EventHandler<StartupProgressEventArgs> progressHandler = (s, e) => ProcessServiceStartup(startupOrder.IndexOf(service), e.Progress, e.CurrentlyProcessing);
                    service.StartupProgress += progressHandler;
                    try
                    {
                        // Start the service
                        service.Start();

                        // Check that that start was successful
                        if (service.GetStartupState() != StartupState.Started)
                        {
                            throw new KernelServiceStartupFailedException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.Exceptions_Messages_KernelServiceStartupFailed_WithService,
                                    service.GetType()));
                        }
                    }
                    finally
                    {
                        service.StartupProgress -= progressHandler;
                    }
                }
            }

            // If we get here then we have to have finished the
            // startup process, which means we're actually running.
            m_State = StartupState.Started;
        }

        /// <summary>
        /// Determines the ideal service startup order of the services.
        /// </summary>
        /// <returns>
        /// An ordered collection of services. Where the order indicates the ideal startup order.
        /// </returns>
        private List<KernelService> DetermineServiceStartupOrder()
        {
            var graph = new AdjacencyGraph<ServiceVertex, Edge<ServiceVertex>>();
            {
                // create the vertices
                var typedCollection = new Dictionary<Type, ServiceVertex>();
                foreach (var pair in m_Services)
                {
                    var vertex = new ServiceVertex(pair.Value);
                    graph.AddVertex(vertex);
                    typedCollection.Add(pair.Key, vertex);
                }

                // create the edges. They point from a dependency to the
                // dependent vertex
                foreach (var pair in typedCollection)
                {
                    var target = pair.Value;
                    if (target.HasDependencies)
                    {
                        // Handle the connections
                        var dependencies = m_Connections[target.Service];
                        foreach (var dependent in dependencies)
                        {
                            Debug.Assert(typedCollection.ContainsKey(dependent.Applied.GetType()), "Missing a service type.");
                            var source = typedCollection[dependent.Applied.GetType()];

                            graph.AddEdge(new Edge<ServiceVertex>(source, target));
                        }

                        // Handle the available demands
                        var connections = GetInstalledDependencies(target.ServiceAsDependencyHolder().ServicesToBeAvailable());
                        foreach (var connection in connections)
                        {
                            Debug.Assert(typedCollection.ContainsKey(connection.Value.GetType()), "Missing a service type.");
                            var source = typedCollection[connection.Value.GetType()];

                            // Remove any duplicate edges before we create them
                            if (!graph.ContainsEdge(source, target))
                            {
                                graph.AddEdge(new Edge<ServiceVertex>(source, target));
                            }
                        }
                    }
                }
            }

            var startupOrder = new List<KernelService>();
            foreach (var vertex in graph.TopologicalSort())
            {
                startupOrder.Add(vertex.Service);
            }

            return startupOrder;
        }

        /// <summary>
        /// Processes the service startup event values.
        /// </summary>
        /// <param name="serviceIndex">Index of the service in the list of services that need to be started.</param>
        /// <param name="progress">The progress percentage for the startup process of the service.</param>
        /// <param name="currentlyProcessing">The mark which indiates what the currently status of the service is.</param>
        private void ProcessServiceStartup(int serviceIndex, int progress, IProgressMark currentlyProcessing)
        {
            Debug.Assert(m_Services.Count > 0, "Cannot calculate the startup progress if there are no services.");
            
            // Calculate the number of services that we have.
            var serviceCount = m_Services.Count;

            // There are serviceIndex number of services that have finished
            // their startup process.
            var finishedPercentage = (double)serviceIndex / serviceCount;

            // The current service is progress percentage finished. That
            // translates to:
            //   (percentage quantity for one service) * (progress in current service)
            //   which is: (1 / serviceCount) * progress / 100
            var currentPercentage = progress / (100.0 * serviceCount);
            var total = finishedPercentage + currentPercentage;

            RaiseStartupProgress((int)Math.Floor(total * 100), currentlyProcessing);
        }

        /// <summary>
        /// Returns a value indicating what the state of the object is regarding
        /// the startup process.
        /// </summary>
        /// <returns>
        ///   The current startup state for the object.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "See: Framework Design Guidelines; Section 5.1, page 136.")]
        public StartupState GetStartupState()
        {
            return m_State;
        }

        /// <summary>
        /// Installs the specified service.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     Only services that are 'installed' can be used by the service manager.
        ///     Services that have not been installed are simply unknown to the service
        ///     manager.
        /// </para>
        /// <para>
        ///     Note that only one instance for each <c>Type</c> can be provided to
        ///     the service manager.
        /// </para>
        /// </remarks>
        /// <param name="service">
        ///     The service which should be installed.
        /// </param>
        public void Install(KernelService service)
        {
            // What happens with the lifetime management of all the services
            // Effectively they should live until we lose them or until the
            // app gets shut down 
            // or until their AppDomain gets killed.
            {
                Enforce.Argument(() => service);
            }

            if (m_Services.ContainsKey(service.GetType()))
            {
                // The service already exists, or an equivalent
                // service already exists. So stop now.
                throw new ServiceTypeAlreadyInstalledException();
            }

            // Check for dependencies
            var dependencyHolder = service as IHaveServiceDependencies;
            if (dependencyHolder != null)
            {
                if (dependencyHolder.ServicesToBeAvailable().Contains(dependencyHolder.GetType()) ||
                    dependencyHolder.ServicesToConnectTo().Contains(dependencyHolder.GetType()))
                {
                    throw new ServiceCannotDependOnItselfException();
                }

                if (dependencyHolder.ServicesToBeAvailable().Contains(typeof(KernelService)) ||
                    dependencyHolder.ServicesToConnectTo().Contains(typeof(KernelService)))
                {
                    throw new ServiceCannotDependOnGenericKernelServiceException();
                }

                var selectedService = GetInstalledDependencies(dependencyHolder.ServicesToConnectTo());
                foreach (var map in selectedService)
                {
                    if (!m_Connections.ContainsKey(service))
                    {
                        m_Connections.Add(service, new List<ConnectionMap>());
                    }

                    var connection = m_Connections[service];
                    connection.Add(new ConnectionMap(map.Key, map.Value));
                    dependencyHolder.ConnectTo(map.Value);
                }
            }

            // Then check all the existing ones and see what we can add 
            // there.
            var selectedServices = GetDependentServices(service);
            foreach (var map in selectedServices)
            {
                if (!m_Connections.ContainsKey(map.Value))
                {
                    m_Connections.Add(map.Value, new List<ConnectionMap>());
                }

                var connection = m_Connections[map.Value];
                connection.Add(new ConnectionMap(map.Key, service));

                var dependent = map.Value as IHaveServiceDependencies;
                Debug.Assert(dependent != null, "The service should be able to handle dependencies.");

                dependent.ConnectTo(service);
            }

            // Finally add the service
            m_Services.Add(service.GetType(), service);
        }

        /// <summary>
        /// Gets the dependency services that are currently available based on the given collection of dependencies.
        /// </summary>
        /// <param name="demandedServices">The collection of types which should be matched up with the installed services.</param>
        /// <returns>
        /// A collection containing a mapping between the demanded dependency and the service that would
        /// serve for this dependency.
        /// </returns>
        /// <design>
        /// Note that the current connection strategy only works if there are no demands for duplicates OR 
        /// services with the same interfaces.
        /// </design>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
                Justification = "We need to return a mapping between a Type and a KernelService. The KeyValuePair<T,K> class is the simplest solution.")]
        private IEnumerable<KeyValuePair<Type, KernelService>> GetInstalledDependencies(IEnumerable<Type> demandedServices)
        {
            // Create a copy of the available service list. Then when we 
            // select a service we'll remove it from this list.
            var available = new List<KernelService>(from service in m_Services select service.Value);

            // Dependencies have just gotten a whole lot more complex
            // We want to be able to match up interfaces & abstract classes too
            // The problem is that we only want to pass each service only once
            var result = new List<KeyValuePair<Type, KernelService>>();
            foreach (var dependency in demandedServices)
            {
                KernelService selected = null;
                foreach (var storedService in available)
                {
                    if (dependency.IsAssignableFrom(storedService.GetType()))
                    {
                        selected = storedService;
                        break;
                    }
                }

                if (selected != null)
                {
                    available.Remove(selected);
                    result.Add(new KeyValuePair<Type, KernelService>(dependency, selected));
                }

                if (available.Count == 0)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the services that depend on the specified service type.
        /// </summary>
        /// <param name="service">The service type.</param>
        /// <returns>
        /// A collection containing a mapping between the demanded dependency and the service that would
        /// serve for this dependency.
        /// </returns>
        private IEnumerable<KeyValuePair<Type, KernelService>> GetDependentServices(KernelService service)
        {
            var result = new List<KeyValuePair<Type, KernelService>>();
            foreach (var map in m_Services)
            {
                var dependencyHolder = map.Value as IHaveServiceDependencies;
                if (dependencyHolder != null)
                {
                    foreach (var dependency in dependencyHolder.ServicesToConnectTo())
                    {
                        if (dependency.IsAssignableFrom(service.GetType()))
                        {
                            result.Add(new KeyValuePair<Type, KernelService>(dependency, map.Value));
                            break;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Uninstalls the specified service.
        /// </summary>
        /// <remarks>
        ///     Once a service is uninstalled it can no longer be started. It is effectively
        ///     removed from the list of known services.
        /// </remarks>
        /// <param name="service">
        ///     The service that needs to be uninstalled.
        /// </param>
        public void Uninstall(KernelService service)
        {
            {
                Enforce.Argument(() => service);
            }

            // Check if we have the service
            if (!m_Services.ContainsKey(service.GetType()))
            {
                throw new UnknownKernelServiceTypeException();
            }

            // Can only uninstall the service if we have the same
            // reference as the object that was installed
            var installedService = m_Services[service.GetType()];
            if (!ReferenceEquals(service, installedService))
            {
                throw new CannotUninstallNonequivalentServiceException();
            }

            // Get all the services that depend on this one.
            var connected = new List<KeyValuePair<KernelService, int>>();
            foreach (var pair in m_Connections)
            {
                int index = pair.Value.FindIndex(map => map.Applied.Equals(service));
                if (index > -1)
                {
                    connected.Add(new KeyValuePair<KernelService, int>(pair.Key, index));
                }
            }

            foreach (var map in connected)
            {
                ((IHaveServiceDependencies)map.Key).DisconnectFrom(service);
                m_Connections[map.Key].RemoveAt(map.Value);
            }

            // Disconnect the service from all it's dependencies
            var dependencyHolder = service as IHaveServiceDependencies;
            if (dependencyHolder != null)
            {
                var connections = m_Connections[service];
                foreach (var map in connections)
                {
                    dependencyHolder.DisconnectFrom(map.Applied);
                }

                m_Connections.Remove(service);
            }

            // Finally remove the service from the collection
            m_Services.Remove(service.GetType());
        }
    }
}
