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
using System.Security;
using System.Security.Permissions;
using Apollo.Core.Messaging;
using Apollo.Core.Properties;
using Apollo.Core.Utils;
using Apollo.Utils;
using Apollo.Utils.Commands;
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
    /// It is assumed that each service will reside in their own AppDomain and that
    /// services do NOT share AppDomains. Upon uninstalling a service the 
    /// service AppDomain will be removed.
    /// </design>
    internal sealed partial class Kernel : MarshalByRefObject, INeedStartup, IKernel
    {
        /// <summary>
        /// The collection of services that are currently known to the kernel.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The storage requires that we link a type to a map of a service and an AppDomain. Nested generics is an easy way to achieve this.")]
        private readonly Dictionary<Type, KeyValuePair<KernelService, AppDomain>> m_Services = 
            new Dictionary<Type, KeyValuePair<KernelService, AppDomain>>();

        /// <summary>
        /// The collection which tracks the connections between a service and it's dependencies.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The storage requires that we link an object to a list of objects. Nested generics is an easy way to achieve this.")]
        private readonly Dictionary<KernelService, List<ConnectionMap>> m_Connections = 
            new Dictionary<KernelService, List<ConnectionMap>>();

        /// <summary>
        /// The start-up state of the kernel.
        /// </summary>
        private StartupState m_State = StartupState.NotStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="Kernel"/> class.
        /// </summary>
        /// <param name="commandStore">The container that stores all the commands.</param>
        /// <param name="processingHelp">The processing help.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandStore"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="processingHelp"/> is <see langword="null"/>.
        /// </exception>
        public Kernel(ICommandContainer commandStore, IHelpMessageProcessing processingHelp)
        {
            {
                Enforce.Argument(() => commandStore);
                Enforce.Argument(() => processingHelp);
            }

            // Add our own proxy to the collection of services.
            // Do that in the constructor so that this is always loaded.
            // That means that there is no way to install another CoreProxy that
            // we don't control.
            // This also means that there is only one way to uninstall this service, 
            // and that is by getting the reference from the kernel.
            Install(new CoreProxy(this, commandStore, processingHelp), AppDomain.CurrentDomain);
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
            var local = StartupProgress;
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
            // Using full trust permissions here because Quick-Graph needs them.
            var startupOrder = SecurityHelpers.Elevate(new PermissionSet(PermissionState.Unrestricted), () => DetermineServiceStartupOrder());
            foreach (var service in startupOrder)
            {
                // Grab the actual current service so that we can put it in the
                // lambda expression without having it wiped or replaced on us
                var currentService = service;

                // See if the service is complete
                var dependencyHolder = currentService as IHaveServiceDependencies;
                if (dependencyHolder != null)
                {
                    // See if all the dependencies are there.
                    if (!dependencyHolder.IsConnectedToAllDependencies)
                    {
                        throw new KernelStartupFailedException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Exceptions_Messages_KernelStartupFailedDueToMissingServiceDependency_WithService,
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

                // Only start the service if it hasn't already been started
                if (currentService.GetStartupState() != StartupState.Started)
                {
                    var map = m_Services[service.GetType()];
                    var serviceDomain = map.Value;

                    // Assert full trust. This can be done safely
                    // because we will attach to the DomainUnload event but we'll only 
                    // run secure code in the unload event.
                    var set = new PermissionSet(PermissionState.Unrestricted);

                    var progressHandler = SecurityHelpers.Elevate(
                        set,
                        () => Activator.CreateInstanceFrom(
                                    serviceDomain,
                                    typeof(ServiceProgressHandler).Assembly.LocalFilePath(),
                                    typeof(ServiceProgressHandler).FullName)
                                .Unwrap() as ServiceProgressHandler);
                    Debug.Assert(progressHandler != null, "Failed to create the UnloadHandler.");
                    
                    // This doesn't work because we're applying the security permission
                    // in the wrong appdomain ...
                    progressHandler.Attach(this, currentService, startupOrder.Count, startupOrder.IndexOf(currentService));
                    try
                    {
                        // Start the service
                        currentService.Start();

                        // Check that that start was successful
                        if (currentService.GetStartupState() != StartupState.Started)
                        {
                            throw new KernelServiceStartupFailedException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.Exceptions_Messages_KernelServiceStartupFailed_WithService,
                                    currentService.GetType()));
                        }
                    }
                    finally
                    {
                        progressHandler.Detach();
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
            // Define the result collection
            var graph = new AdjacencyGraph<ServiceVertex, Edge<ServiceVertex>>();
            {
                // create the vertices
                var typedCollection = new Dictionary<Type, ServiceVertex>();
                foreach (var pair in m_Services)
                {
                    var vertex = new ServiceVertex(pair.Value.Key);
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

            return graph.TopologicalSort()
                .Select(vertex => vertex.Service)
                .ToList();
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
        /// <param name="service">The service which should be installed.</param>
        /// <param name="serviceDomain">The <see cref="AppDomain"/> in which the service resides.</param>
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
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="serviceDomain"/> is <see langword="null" />.
        /// </exception>
        public void Install(KernelService service, AppDomain serviceDomain)
        {
            // What happens with the lifetime management of all the services
            // Effectively they should live until we lose them or until the
            // app gets shut down 
            // or until their AppDomain gets killed.
            {
                Enforce.Argument(() => service);
                Enforce.Argument(() => serviceDomain);
            }

            // We can only install when we are not started or started. No other 
            // state is condusive to installation.
            if ((m_State != StartupState.NotStarted) && (m_State != StartupState.Started))
            {
                throw new KernelNotInInstallReadyStateException();
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
            
            // Only add an unload event if we're not attaching to the domain that 
            // holds the kernel.
            if (!ReferenceEquals(serviceDomain, AppDomain.CurrentDomain))
            {

                // Assert full trust. This can be done safely
                // because we will attach to the DomainUnload event but we'll only 
                // run secure code in the unload event.
                var set = new PermissionSet(PermissionState.Unrestricted);

                var unloadHandler = SecurityHelpers.Elevate(
                    set, 
                    () => Activator.CreateInstanceFrom(
                                serviceDomain,
                                typeof(AppDomainUnloadHandler).Assembly.LocalFilePath(),
                                typeof(AppDomainUnloadHandler).FullName)
                            .Unwrap() as AppDomainUnloadHandler);

                Debug.Assert(unloadHandler != null, "Failed to create the UnloadHandler.");
                unloadHandler.AttachToUnloadEvent(this);
            }

            // Finally add the service
            m_Services.Add(service.GetType(), new KeyValuePair<KernelService, AppDomain>(service, serviceDomain));
        }

        /// <summary>
        /// Handles the <see cref="AppDomain.DomainUnload"/> event for service AppDomains.
        /// </summary>
        /// <param name="domainToUnload">The <c>AppDomain</c> that is about to be unloaded.</param>
        private void HandleServiceDomainUnloading(AppDomain domainToUnload)
        {
            if (domainToUnload != null)
            {
                // Go find the service(s) that reside in this appdomain.
                var services = (from serviceMap in m_Services
                                where serviceMap.Value.Value.Equals(domainToUnload)
                                select serviceMap.Value.Key).ToList();

                foreach (var service in services)
                {
                    // No need to explicitly unload the domain because that's already happening.
                    Uninstall(service, false);
                }
            }
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
            var available = new List<KernelService>(from service in m_Services select service.Value.Key);

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
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This internal method needs to return a collection of Type - KernelService mappings.")]
        private IEnumerable<KeyValuePair<Type, KernelService>> GetDependentServices(KernelService service)
        {
            var result = new List<KeyValuePair<Type, KernelService>>();
            foreach (var map in m_Services)
            {
                var dependencyHolder = map.Value.Key as IHaveServiceDependencies;
                if (dependencyHolder != null)
                {
                    foreach (var dependency in dependencyHolder.ServicesToConnectTo())
                    {
                        if (dependency.IsAssignableFrom(service.GetType()))
                        {
                            result.Add(new KeyValuePair<Type, KernelService>(dependency, map.Value.Key));
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
        /// <param name="service">The service that needs to be uninstalled.</param>
        /// <param name="shouldUnloadDomain">Indicates if the <c>AppDomain</c> that held the service should be unloaded or not.</param>
        /// <remarks>
        /// Once a service is uninstalled it can no longer be started. It is effectively
        /// removed from the list of known services.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Once we started uninstalling it must finish.")]
        public void Uninstall(KernelService service, bool shouldUnloadDomain)
        {
            // Unlike installation, we can pretty much always uninstall.
            // It may get ugly but we can remove the references.
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
            var installedServiceMap = m_Services[service.GetType()];
            if (!ReferenceEquals(service, installedServiceMap.Key))
            {
                throw new CannotUninstallNonequivalentServiceException();
            }

            // Get all the services that depend on this one.
            var connected = (from pair in m_Connections 
                                let index = pair.Value.FindIndex(map => map.Applied.Equals(service)) 
                             where index > -1 
                             select new KeyValuePair<KernelService, int>(pair.Key, index)).ToList();

            foreach (var map in connected)
            {
                ((IHaveServiceDependencies) map.Key).DisconnectFrom(service);
                m_Connections[map.Key].RemoveAt(map.Value);
            }

            // Disconnect the service from all it's dependencies
            // Note that we need to check that the service still exists because
            // we might be halfway through a shutdown.
            var dependencyHolder = service as IHaveServiceDependencies;
            if ((dependencyHolder != null) && m_Connections.ContainsKey(service))
            {
                var connections = m_Connections[service];
                foreach (var map in connections)
                {
                    try
                    {
                        dependencyHolder.DisconnectFrom(map.Applied);
                    }
                    catch (Exception)
                    {
                        // For now do nothing. Later on we'll log here ...
                    }
                }

                m_Connections.Remove(service);
            }

            // Remove the service. Now that it doesn't have any of the connections anymore.
            m_Services.Remove(service.GetType());
            
            // Unload the appdomain. It shouldn't be needed anymore.
            // We don't need to disconnect from the DomainUnload event, the code
            // that handles that lives inside the AppDomain so it'll be removed
            // automatically
            // only unload if it's not our own appdomain (that would be bad).
            var serviceDomain = installedServiceMap.Value;
            if (shouldUnloadDomain && !serviceDomain.Equals(AppDomain.CurrentDomain))
            {
                try
                {
                    // Assert permission to control the AppDomain. This can be done safely
                    // because we will attach to the AssemblyResolve event but we'll only 
                    // resolve assemblies from a known set of paths or files.
                    var set = new PermissionSet(PermissionState.None);
                    set.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlAppDomain));

                    // This can go wrong if there are still threads executing in this
                    // AppDomain. If it does go wrong we'll bail out semi-gracefully
                    SecurityHelpers.Elevate(set, () => AppDomain.Unload(serviceDomain));
                }
                catch (Exception)
                {
                    // For now do nothing. Later on we should log the problem here.
                    // @Note should this do something nasty to the application?
                }
            }
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet>
        ///     <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/>
        /// </PermissionSet>
        public override object InitializeLifetimeService()
        {
            // We don't really want the system to GC our kernel at random times...
            return null;
        }
    }
}