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
    internal sealed partial class Kernel : INeedStartup
    {
        /// <summary>
        /// The collection of services that are currently known to the kernel.
        /// </summary>
        private readonly Dictionary<Type, KernelService> m_Services = new Dictionary<Type, KernelService>();

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
                    typedCollection.Add(pair.Key, new ServiceVertex(pair.Value));
                }

                // create the edges. They point from a dependency to the
                // dependent vertex
                foreach (var pair in typedCollection)
                {
                    var target = pair.Value;
                    if (target.HasDependencies)
                    {
                        var dependencyHolder = target.ServiceAsDependencyHolder();
                        var dependencies = GetDependencies(dependencyHolder.ServicesToConnectTo());
                        dependencies.ForEach(service => graph.AddEdge(new Edge<ServiceVertex>(typedCollection[service.GetType()], target)));

                        var connections = GetDependencies(dependencyHolder.ServicesToBeAvailable());
                        connections.ForEach(service => graph.AddEdge(new Edge<ServiceVertex>(typedCollection[service.GetType()], target)));
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
                var selectedService = GetInstalledDependencies(dependencyHolder);
                selectedService.ForEach(demandedService => dependencyHolder.ConnectTo(demandedService));
            }

            // Then check all the existing ones and see what we can add 
            // there.
            var selectedServices = GetDependentServices(service.GetType());
            selectedServices.ForEach(selectedService => selectedService.ConnectTo(service));

            // Finally add the service
            m_Services.Add(service.GetType(), service);
        }

        /// <summary>
        /// Gets the dependency services that are currently available.
        /// </summary>
        /// <param name="dependencyHolder">The object for which the dependencies should be determined.</param>
        /// <returns>
        /// A collection of existing services that match the dependency demands of the given object.
        /// </returns>
        private IEnumerable<KernelService> GetInstalledDependencies(IHaveServiceDependencies dependencyHolder)
        {
            var demandedServices = dependencyHolder.ServicesToConnectTo();
            return GetDependencies(demandedServices);
        }

        /// <summary>
        /// Gets the installed services that match the selection of types in the collection.
        /// </summary>
        /// <param name="demandedServices">The collection of demanded service types.</param>
        /// <returns>
        /// A collection of existing services that match the dependency demands of the given object.
        /// </returns>
        private IEnumerable<KernelService> GetDependencies(IEnumerable<Type> demandedServices)
        {
            var selectedService = from storedService in m_Services
                                  from demandedService in demandedServices
                                  where demandedService.IsAssignableFrom(storedService.Key)
                                  select storedService.Value;
            return selectedService;
        }

        /// <summary>
        /// Gets the services that depend on the specified service type.
        /// </summary>
        /// <param name="service">The service type.</param>
        /// <returns>
        /// A collection of existing services that depend on the specified service type.
        /// </returns>
        private IEnumerable<IHaveServiceDependencies> GetDependentServices(Type service)
        {
            Debug.Assert(typeof(KernelService).IsAssignableFrom(service), "Can only search for a KernelService type.");

            var dependencyHolders = from demandedService in m_Services
                                    where (demandedService.Value is IHaveServiceDependencies)
                                    select (demandedService.Value as IHaveServiceDependencies);

            var selectedServices = from demandedService in dependencyHolders
                                   from dependency in demandedService.ServicesToConnectTo()
                                   where dependency.IsAssignableFrom(service)
                                   select demandedService;
            return selectedServices;
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
                throw new CannotUninstallNonEquivalentServiceException();
            }

            // Get all the services that depend on this one.
            var selectedServices = GetDependentServices(service.GetType());
            selectedServices.ForEach(selectedService => selectedService.DisconnectFrom(service));

            // Disconnect the service from all it's dependencies
            var dependencyHolder = service as IHaveServiceDependencies;
            if (dependencyHolder != null)
            {
                var selectedService = GetInstalledDependencies(dependencyHolder);
                selectedService.ForEach(demandedService => dependencyHolder.DisconnectFrom(demandedService));
            }

            // Finally remove the service from the collection
            m_Services.Remove(service.GetType());
        }
    }
}
