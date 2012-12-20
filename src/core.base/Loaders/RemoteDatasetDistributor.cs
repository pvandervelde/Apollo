//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;
using NManto;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Provides methods to get dataset loading proposals from the known remote endpoints.
    /// </summary>
    internal sealed class RemoteDatasetDistributor : IGenerateDistributionProposals, ILoadDatasets
    {
        private static IEnumerable<DatasetLoadingProposal> RetrieveProposals(
            IEnumerable<Tuple<EndpointId, IDatasetLoaderCommands>> availableEndpoints,
            IConfiguration configuration,
            DatasetRequest request,
            CancellationToken token)
        {
            var usableNodes = DetermineUsableEndpoints(availableEndpoints, configuration);
            var orderedProposals = OrderProposals(request.ExpectedLoadPerMachine, request.PreferredLocations, usableNodes, token);
            return orderedProposals;
        }

        private static IEnumerable<Tuple<EndpointId, IDatasetLoaderCommands>> DetermineUsableEndpoints(
            IEnumerable<Tuple<EndpointId, IDatasetLoaderCommands>> availableEndpoints,
            IConfiguration configuration)
        {
            IEnumerable<Tuple<EndpointId, IDatasetLoaderCommands>> usableNodes = availableEndpoints;

            var key = LoaderConfigurationKeys.OffLimitsEndpoints;
            if (configuration.HasValueFor(key))
            {
                var offlimitEndpoints = configuration.Value<IDictionary<string, object>>(key);
                var nonUsableNodes = from node in availableEndpoints
                                     from string machine in offlimitEndpoints.Values
                                     where node.Item1.IsOnMachine(machine)
                                     select node.Item1;

                usableNodes = from node in availableEndpoints
                              where (!nonUsableNodes.Contains(node.Item1))
                              select node;
            }

            return usableNodes;
        }

        private static IEnumerable<DatasetLoadingProposal> OrderProposals(
            ExpectedDatasetLoad load,
            LoadingLocations preferedLocations,
            IEnumerable<Tuple<EndpointId, IDatasetLoaderCommands>> usableNodes,
            CancellationToken token)
        {
            var loadingProposals = new Queue<Task<DatasetLoadingProposal>>();

            bool shouldLoad = ShouldLoadDistributed(preferedLocations);
            foreach (var pair in usableNodes)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (shouldLoad)
                {
                    try
                    {
                        var result = pair.Item2.ProposeFor(load);
                        loadingProposals.Enqueue(result);
                    }
                    catch (CommandInvocationFailedException)
                    {
                        // Chances are the endpoint just disappeared
                        // so we just ignore it and move on.
                    }
                }
            }

            while (loadingProposals.Count > 0)
            {
                if (token.IsCancellationRequested)
                {
                    // @todo: How do we deal with the tasks that are running?
                    //        do we just abadon them or ...???
                    token.ThrowIfCancellationRequested();
                }

                var task = loadingProposals.Dequeue();
                if (!task.IsCompleted)
                {
                    if (loadingProposals.Count > 1)
                    {
                        loadingProposals.Enqueue(task);
                        continue;
                    }
                    else
                    {
                        try
                        {
                            task.Wait();
                        }
                        catch (AggregateException)
                        {
                            continue;
                        }
                    }
                }

                if (task.IsCanceled || task.IsFaulted)
                {
                    // Get the exception so that the task doesn't throw in
                    // the finalizer. Don't do anything with this though
                    // because we don't really care.
                    var exception = task.Exception;
                    continue;
                }

                var proposal = task.Result;
                if (proposal.IsAvailable)
                {
                    yield return proposal;
                }
            }
        }

        private static bool ShouldLoadDistributed(LoadingLocations preferedLocations)
        {
            return ((preferedLocations & LoadingLocations.DistributedOnCluster) == LoadingLocations.DistributedOnCluster)
                || ((preferedLocations & LoadingLocations.DistributedOnPeerToPeer) == LoadingLocations.DistributedOnPeerToPeer);
        }

        /// <summary>
        /// The object used to take out locks on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The object that provides the commands to load datasets onto other machines.
        /// </summary>
        private readonly Dictionary<EndpointId, IDatasetLoaderCommands> m_LoaderCommands =
            new Dictionary<EndpointId, IDatasetLoaderCommands>();

        /// <summary>
        /// The object that manages the remote command proxies.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_CommandHub;

        /// <summary>
        /// The object that receives notifications from remote endpoints.
        /// </summary>
        private readonly INotifyOfRemoteEndpointEvents m_NotificationHub;

        /// <summary>
        /// The object that stores the configuration for the current application.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// The collection that stores all the uploads waiting to be
        /// started.
        /// </summary>
        private readonly IStoreUploads m_Uploads;

        /// <summary>
        /// The function that returns a <see cref="DatasetOnlineInformation"/>.
        /// </summary>
        private readonly Func<DatasetId, EndpointId, NetworkIdentifier, DatasetOnlineInformation> m_DatasetInformationBuilder;

        /// <summary>
        /// The object that handles the communication for the application.
        /// </summary>
        private readonly ICommunicationLayer m_CommunicationLayer;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDatasetDistributor"/> class.
        /// </summary>
        /// <param name="commandHub">The object that manages the remote command proxies.</param>
        /// <param name="notificationHub">The object that receives notifications from remote endpoints.</param>
        /// <param name="configuration">The application specific configuration.</param>
        /// <param name="uploads">The object that stores all the uploads waiting to be started.</param>
        /// <param name="datasetInformationBuilder">The function that builds <see cref="DatasetOnlineInformation"/> objects.</param>
        /// <param name="communicationLayer">The object that handles the communication for the application.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandHub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationHub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="datasetInformationBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="communicationLayer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public RemoteDatasetDistributor(
            ISendCommandsToRemoteEndpoints commandHub,
            INotifyOfRemoteEndpointEvents notificationHub,
            IConfiguration configuration,
            IStoreUploads uploads,
            Func<DatasetId, EndpointId, NetworkIdentifier, DatasetOnlineInformation> datasetInformationBuilder,
            ICommunicationLayer communicationLayer,
            SystemDiagnostics systemDiagnostics,
            TaskScheduler scheduler = null)
        {
            {
                Lokad.Enforce.Argument(() => commandHub);
                Lokad.Enforce.Argument(() => notificationHub);
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => datasetInformationBuilder);
                Lokad.Enforce.Argument(() => communicationLayer);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Configuration = configuration;
            m_Uploads = uploads;
            m_DatasetInformationBuilder = datasetInformationBuilder;
            m_CommunicationLayer = communicationLayer;
            m_Diagnostics = systemDiagnostics;
            m_Scheduler = scheduler ?? TaskScheduler.Default;
            m_CommandHub = commandHub;
            {
                // Set up the events so that we can see the loaders come online.
                //
                // Note that the events may come in on a different thread than the one
                // we're normally accessed on. This is because adding an enpoint is usually 
                // a result of a WCF message being received, on the WCF message thread.
                m_CommandHub.OnEndpointSignedIn += (s, e) => AddNewEndpoint(e.Endpoint, e.Commands);
                m_CommandHub.OnEndpointSignedOff += (s, e) => RemoveEndpoint(e.Endpoint);

                var knownCommands = m_CommandHub.AvailableCommands();
                foreach (var command in knownCommands)
                {
                    AddNewEndpoint(command.Endpoint, command.RegisteredCommands);
                }
            }

            m_NotificationHub = notificationHub;
        }

        private void AddNewEndpoint(EndpointId endpoint, IEnumerable<Type> commandTypes)
        {
            if (commandTypes.Contains(typeof(IDatasetLoaderCommands)))
            {
                lock (m_Lock)
                {
                    if (!m_LoaderCommands.ContainsKey(endpoint))
                    {
                        IDatasetLoaderCommands command = null;
                        try
                        {
                            command = m_CommandHub.CommandsFor<IDatasetLoaderCommands>(endpoint);
                        }
                        catch (CommandNotSupportedException)
                        {
                            // Secretly we actually don't have the command so just ignore this
                            // endpoint. This could be caused by the endpoint disappearing or
                            // some other networky problem.
                        }

                        if (command != null)
                        {
                            m_LoaderCommands.Add(endpoint, command);
                        }
                    }
                }
            }
        }

        private void RemoveEndpoint(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                if (m_LoaderCommands.ContainsKey(endpoint))
                {
                    m_LoaderCommands.Remove(endpoint);
                }
            }
        }

        /// <summary>
        /// Processes the dataset request and returns a collection of distribution plans.
        /// </summary>
        /// <param name="request">
        /// The request that describes the characteristics of the dataset that 
        /// should be loaded.
        /// </param>
        /// <param name="token">The cancellation token that can be used to terminate the proposal.</param>
        /// <returns>
        /// The collection containing all the distribution plans.
        /// </returns>
        public IEnumerable<DistributionPlan> ProposeDistributionFor(DatasetRequest request, CancellationToken token)
        {
            var availableEndpoints = new List<Tuple<EndpointId, IDatasetLoaderCommands>>();
            lock (m_Lock)
            {
                foreach (var pair in m_LoaderCommands)
                {
                    availableEndpoints.Add(new Tuple<EndpointId, IDatasetLoaderCommands>(pair.Key, pair.Value));
                }
            }

            using (var interval = m_Diagnostics.Profiler.Measure("Generating remote proposal"))
            {
                var proposals = RetrieveProposals(availableEndpoints, m_Configuration, request, token);
                return from proposal in proposals
                       select new DistributionPlan(
                           (p, t, r) => ImplementPlan(p, t, r),
                           request.DatasetToLoad,
                           new NetworkIdentifier(proposal.Endpoint.OriginatesOnMachine()),
                           proposal);
            }
        }

        /// <summary>
        /// Takes the set of distribution plans and loads the given datasets onto the specified machines.
        /// </summary>
        /// <param name="planToImplement">The distribution plan that should be implemented.</param>
        /// <param name="token">The token used to indicate cancellation of the task.</param>
        /// <param name="progressReporter">The action that handles the reporting of progress.</param>
        /// <returns>
        /// A set of objects which allow act as proxies for the loaded datasets.
        /// </returns>
        public Task<DatasetOnlineInformation> ImplementPlan(
            DistributionPlan planToImplement, 
            CancellationToken token,
            Action<int, IProgressMark, TimeSpan> progressReporter)
        {
            Func<DatasetOnlineInformation> result =
                () =>
                {
                    IDatasetLoaderCommands loaderCommands = null;
                    lock (m_Lock)
                    {
                        if (!m_LoaderCommands.ContainsKey(planToImplement.Proposal.Endpoint))
                        {
                            throw new EndpointNotContactableException(planToImplement.Proposal.Endpoint);
                        }

                        loaderCommands = m_LoaderCommands[planToImplement.Proposal.Endpoint];
                    }

                    // We shouldn't have to load the TCP channel at this point because that channel would have
                    // been loaded when the loaders broadcast their message indicating that they exist.
                    var info = (from connection in m_CommunicationLayer.LocalConnectionPoints()
                                where connection.ChannelType.Equals(typeof(TcpChannelType))
                                select connection).First();

                    var endpointTask = loaderCommands.Load(info, planToImplement.DistributionFor.Id);
                    endpointTask.Wait();

                    var endpoint = endpointTask.Result;
                    var resetEvent = new AutoResetEvent(false);
                    var commandAvailabilityNotifier =
                        Observable.FromEventPattern<CommandSetAvailabilityEventArgs>(
                            h => m_CommandHub.OnEndpointSignedIn += h,
                            h => m_CommandHub.OnEndpointSignedIn -= h)
                        .Where(args => args.EventArgs.Endpoint.Equals(endpoint))
                        .Take(1)
                        .Subscribe(
                            args =>
                            {
                                resetEvent.Set();
                            });

                    using (commandAvailabilityNotifier)
                    {
                        if (!m_CommandHub.HasCommandsFor(endpoint) || !m_NotificationHub.HasNotificationsFor(endpoint))
                        {
                            resetEvent.WaitOne();
                        }
                    }

                    EventHandler<ProgressEventArgs> progressHandler = 
                        (s, e) => progressReporter(e.Progress, e.CurrentlyProcessing, e.EstimatedFinishingTime);
                    var notifications = m_NotificationHub.NotificationsFor<IDatasetApplicationNotifications>(endpoint);
                    notifications.OnProgress += progressHandler;
                    try
                    {
                        // Store the file 
                        var file = planToImplement.DistributionFor.StoredAt.AsFile();
                        var uploadToken = m_Uploads.Register(file.FullName);

                        // The commands have been registered, so now load the dataset
                        Debug.Assert(
                            m_CommandHub.HasCommandFor(endpoint, typeof(IDatasetApplicationCommands)), "No application commands registered.");
                        var applicationCommands = m_CommandHub.CommandsFor<IDatasetApplicationCommands>(endpoint);
                        var task = applicationCommands.Load(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            uploadToken);
                        task.Wait();

                        return m_DatasetInformationBuilder(
                            planToImplement.DistributionFor.Id,
                            endpoint,
                            planToImplement.MachineToDistributeTo);
                    }
                    finally
                    {
                        notifications.OnProgress -= progressHandler;
                    }
                };

            return Task<DatasetOnlineInformation>.Factory.StartNew(
                result, 
                token,
                TaskCreationOptions.LongRunning,
                m_Scheduler);
        }
    }
}
