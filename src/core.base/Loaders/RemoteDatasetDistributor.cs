//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;
using Lokad;

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
            var orderedProposals = OrderProposals(request.ExpectedLoadPerMachine, request.PreferedLocations, usableNodes, token);
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
        /// The object that handles the communication between applications.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// The object that manages the remote command proxies.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_Hub;

        /// <summary>
        /// The object that stores the configuration for the current application.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDatasetDistributor"/> class.
        /// </summary>
        /// <param name="layer">The object that handles the communication between applications.</param>
        /// <param name="commandHub">The object that manages the remote command proxies.</param>
        /// <param name="configuration">The application specific configuration.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandHub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        public RemoteDatasetDistributor(
            ICommunicationLayer layer,
            ISendCommandsToRemoteEndpoints commandHub, 
            IConfiguration configuration,
            TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => layer);
                Enforce.Argument(() => commandHub);
                Enforce.Argument(() => configuration);
            }

            m_Layer = layer;
            m_Configuration = configuration;
            m_Scheduler = scheduler ?? TaskScheduler.Default;
            m_Hub = commandHub;
            {
                // Note that the events may come in on a different thread than the one
                // we're normally accessed on. This is because adding an enpoint is usually 
                // a result of a WCF message being received, on the WCF message thread.
                m_Hub.OnEndpointSignedIn += (s, e) => AddNewEndpoint(e.Endpoint, e.Commands);
                m_Hub.OnEndpointSignedOff += (s, e) => RemoveEndpoint(e.Endpoint);

                var knownCommands = m_Hub.AvailableCommands();
                foreach (var command in knownCommands)
                {
                    AddNewEndpoint(command.Endpoint, command.RegisteredCommands);
                }
            }
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
                            command = m_Hub.CommandsFor<IDatasetLoaderCommands>(endpoint);
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

            var proposals = RetrieveProposals(availableEndpoints, m_Configuration, request, token);
            return from proposal in proposals
                   select new DistributionPlan(
                       (p, t) => ImplementPlan(p, t),
                       request.DatasetToLoad,
                       new NetworkIdentifier(proposal.Endpoint.OriginatesOnMachine()),
                       proposal);
        }

        /// <summary>
        /// Takes the set of distribution plans and loads the given datasets onto the specified machines.
        /// </summary>
        /// <param name="planToImplement">The distribution plan that should be implemented.</param>
        /// <param name="token">The token used to indicate cancellation of the task.</param>
        /// <returns>
        /// A set of objects which allow act as proxies for the loaded datasets.
        /// </returns>
        public Task<DatasetOnlineInformation> ImplementPlan(DistributionPlan planToImplement, CancellationToken token)
        {
            Func<DatasetOnlineInformation> result =
                () =>
                {
                    var connection = (from i in m_Layer.LocalConnectionPoints()
                                      where i.ChannelType.Equals(typeof(NamedPipeChannelType))
                                      select i).First();

                    IDatasetLoaderCommands commands = null;
                    lock (m_Lock)
                    {
                        if (m_LoaderCommands.ContainsKey(planToImplement.Proposal.Endpoint))
                        {
                            throw new EndpointNotContactableException(planToImplement.Proposal.Endpoint);
                        }

                        commands = m_LoaderCommands[planToImplement.Proposal.Endpoint];
                    }

                    var endpointTask = commands.Load(connection, planToImplement.DistributionFor.Id);
                    endpointTask.Wait();

                    // Wait for the application to start up and register itself with our command hub.
                    var endpoint = endpointTask.Result;
                    var resetEvent = new AutoResetEvent(false);
                    Observable.FromEvent<CommandSetAvailabilityEventArgs>(
                            h => m_Hub.OnEndpointSignedIn += h,
                            h => m_Hub.OnEndpointSignedIn -= h)
                        .Where(args => args.EventArgs.Endpoint.Equals(endpoint))
                        .Take(1)
                        .Subscribe(
                            args =>
                            {
                                resetEvent.Set();
                            });

                    if (!m_Hub.HasCommandsFor(endpoint))
                    {
                        resetEvent.WaitOne();
                    }

                    // The commands have been registered, so now wait for the 
                    // dataset to be loaded.
                    //
                    // Now the dataset loading is complete
                    return new DatasetOnlineInformation(
                        planToImplement.DistributionFor.Id,
                        planToImplement.Proposal.Endpoint,
                        planToImplement.MachineToDistributeTo,
                        m_Hub);
                };

            return Task<DatasetOnlineInformation>.Factory.StartNew(
                result, 
                token,
                TaskCreationOptions.LongRunning,
                m_Scheduler);
        }
    }
}
