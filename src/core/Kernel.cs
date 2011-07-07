// -----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Apollo.Core.Properties;
using Apollo.Utilities;
using Lokad;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the core class that controls the kernel of the Apollo application.
    /// </summary>
    /// <design>
    /// The kernel will determine the order in which the services must be 
    /// started based on their dependencies. 
    /// </design>
    internal sealed partial class Kernel : INeedStartup, IKernel
    {
        /// <summary>
        /// The collection of services that are currently known to the kernel.
        /// </summary>
        private readonly Dictionary<Type, KernelService> m_Services = new Dictionary<Type, KernelService>();

        /// <summary>
        /// The collection which tracks the connections between a service and it's dependencies.
        /// </summary>
        private readonly Dictionary<KernelService, List<ConnectionMap>> m_Connections = 
            new Dictionary<KernelService, List<ConnectionMap>>();

        /// <summary>
        /// The action that should be executed just before the entire system shuts down.
        /// </summary>
        private readonly Action m_ShutdownAction;

        /// <summary>
        /// The start-up state of the kernel.
        /// </summary>
        private StartupState m_State = StartupState.NotStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="Kernel"/> class.
        /// </summary>
        /// <param name="shutdownAction">The action that should be executed just before shut down.</param>
        public Kernel(Action shutdownAction = null)
        {
            m_ShutdownAction = shutdownAction;

            // Add our own proxy to the collection of services.
            // Do that in the constructor so that this is always loaded.
            // That means that there is no way to install another CoreProxy that
            // we don't control.
            // This also means that there is only one way to uninstall this service, 
            // and that is by getting the reference from the kernel which is only 
            // possible if we start poking around in the data structures of the kernel.
            // In other words there shouldn't be any (legal) way of removing the
            // coreproxy object.
            Install(new CoreProxy(this));
        }

        /// <summary>
        /// The event that is fired when there is an update in the startup process.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnStartupProgress;

        /// <summary>
        /// Raises the startup progress event.
        /// </summary>
        /// <param name="progress">The progress percentage.</param>
        /// <param name="mark">The progress mark.</param>
        private void RaiseOnStartupProgress(int progress, IProgressMark mark)
        {
            var local = OnStartupProgress;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, mark));
            }
        }

        /// <summary>
        /// Initialized the kernel by allowing all the kernel services to 
        /// go through their initialization processes.
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
            m_State = StartupState.Starting;

            // In order to keep this flexible we will need to sort the services
            // so that the startup order guarantuees that each service will have 
            // its dependencies and requirements running before it does.
            // Obviously this is prone to cyclic loops ...
            var startupOrder = DetermineServiceStartupOrder();
            foreach (var service in startupOrder)
            {
                // Grab the actual current service so that we can put it in the
                // lambda expression without having it wiped or replaced on us
                var currentService = service;

                var dependencyHolder = currentService as IHaveServiceDependencies;
                if (dependencyHolder != null)
                {
                    if (!dependencyHolder.IsConnectedToAllDependencies)
                    {
                        throw new KernelStartupFailedException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources_NonTranslatable.Exception_Messages_KernelStartupFailedDueToMissingServiceDependency_WithService,
                                currentService.GetType()));
                    }

                    // There is no need to check that all the dependencies are up and
                    // running because:
                    // - We know that all the services started prior to the current one
                    //   are running (otherwise we would have thrown).
                    // - We know that all services are sorted by dependency, the dependent
                    //   services last and the independent services first.
                    // So this means that all dependencies must be running.
                }

                if (currentService.StartupState != StartupState.Started)
                {
                    EventHandler<ProgressEventArgs> handler = (s, e) =>
                        {
                            var finishedPercentage = (double)startupOrder.IndexOf(currentService) / startupOrder.Count;
                            var currentPercentage = e.Progress / (100.0 * startupOrder.Count);
                            var total = finishedPercentage + currentPercentage;

                            RaiseOnStartupProgress((int)Math.Floor(total * 100), e.CurrentlyProcessing);
                        };

                    currentService.OnStartupProgress += handler;
                    try
                    {
                        currentService.Start();
                        if (currentService.StartupState != StartupState.Started)
                        {
                            throw new KernelServiceStartupFailedException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources_NonTranslatable.Exception_Messages_KernelServiceStartupFailed_WithService,
                                    currentService.GetType()));
                        }
                    }
                    finally
                    {
                        currentService.OnStartupProgress -= handler;
                    }
                }
            }

            m_State = StartupState.Started;
            SendStartupCompleteMessage();
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
                var typedCollection = new Dictionary<Type, ServiceVertex>();
                foreach (var pair in m_Services)
                {
                    var vertex = new ServiceVertex(pair.Value);
                    graph.AddVertex(vertex);
                    typedCollection.Add(pair.Key, vertex);
                }

                // The edges point from a dependency to the dependent vertex
                foreach (var pair in typedCollection)
                {
                    var target = pair.Value;
                    if (target.HasDependencies)
                    {
                        var dependencies = m_Connections[target.Service];
                        foreach (var dependent in dependencies)
                        {
                            Debug.Assert(typedCollection.ContainsKey(dependent.Applied.GetType()), "Missing a service type.");
                            var source = typedCollection[dependent.Applied.GetType()];

                            graph.AddEdge(new Edge<ServiceVertex>(source, target));
                        }
                    }
                }
            }

            return graph.TopologicalSort()
                .Select(vertex => vertex.Service)
                .ToList();
        }

        /// <summary>
        /// Sends a message to all services to indicate that the start-up process has completed.
        /// </summary>
        private void SendStartupCompleteMessage()
        {
            var coreProxy = m_Services[typeof(CoreProxy)] as CoreProxy;
            Debug.Assert(coreProxy != null, "Stored an incorrect service under the CoreProxy type.");

            coreProxy.NotifyServicesOfStartupCompletion();
        }

        /// <summary>
        /// Gets a value indicating what the state of the object is regarding
        /// the startup process.
        /// </summary>
        public StartupState StartupState
        {
            get
            {
                return m_State;
            }
        }

        /// <summary>
        /// Shuts the application down.
        /// </summary>
        /// <design>
        /// Note that this method does not check if it is safe to shut the application down. It is assumed that
        /// there are no more objections against shutting down once this method is reached. 
        /// </design>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The shutdown must proceede even if a service throws an unknown exception.")]
        public void Shutdown()
        {
            m_State = StartupState.Stopping;

            // In order to keep this flexible we will need to sort the services
            // so that the startup order guarantuees that each service will have 
            // its dependencies and requirements running before it does.
            // Obviously this is prone to cyclic loops ...
            var startupOrder = DetermineServiceStartupOrder();

            // Reverse the order so that we move from most dependent 
            // to least dependent
            startupOrder.Reverse();

            foreach (var service in startupOrder)
            {
                try
                {
                    service.Stop();
                }
                catch
                {
                    // An exception occured. Ignore it and move on
                    // we're about to destroy the appdomain the service lives in.
                }
            }

            // Run the final shut down action. This will normally dispose the 
            // IOC container etc.
            if (m_ShutdownAction != null)
            {
                m_ShutdownAction();
            }

            m_State = StartupState.Stopped;
        }

        /// <summary>
        /// Installs the specified service.
        /// </summary>
        /// <param name="service">The service which should be installed.</param>
        /// <remarks>
        /// <para>
        /// Only services that are 'installed' can be used by the service manager.
        /// Services that have not been installed are simply unknown to the service
        /// manager.
        /// </para>
        /// <para>
        /// Note that only one instance for each <c>Type</c> can be provided to
        /// the service manager.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="service"/> is <see langword="null" />.
        /// </exception>
        public void Install(KernelService service)
        {
            {
                Enforce.Argument(() => service);
            }

            if ((m_State != StartupState.NotStarted) && (m_State != StartupState.Started))
            {
                throw new KernelNotInInstallReadyStateException();
            }

            if (m_Services.ContainsKey(service.GetType()))
            {
                throw new ServiceTypeAlreadyInstalledException();
            }

            var dependencyHolder = service as IHaveServiceDependencies;
            if (dependencyHolder != null)
            {
                if (dependencyHolder.ServicesToConnectTo().Contains(dependencyHolder.GetType()))
                {
                    throw new ServiceCannotDependOnItselfException();
                }

                if (dependencyHolder.ServicesToConnectTo().Contains(typeof(KernelService)))
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
    }
}
