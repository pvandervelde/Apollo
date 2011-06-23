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

        private static List<DatasetLoadingProposal> OrderProposals(
            ExpectedDatasetLoad load,
            LoadingLocations preferedLocations,
            IEnumerable<Tuple<EndpointId, IDatasetLoaderCommands>> usableNodes,
            CancellationToken token)
        {
            var loadingProposals = new Queue<Task<DatasetLoadingProposal>>();

            // We assume that all loaders which are attached to a command
            // are distributed. The local loader will be communicated with
            // directly.
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

            var orderedProposals = new List<DatasetLoadingProposal>();
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
                    orderedProposals.Add(proposal);
                }
            }

            return orderedProposals;
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
        /// The object that stores the configuration for the current application.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDatasetDistributor"/> class.
        /// </summary>
        /// <param name="commandHub">The object that manages the remote command proxies.</param>
        /// <param name="configuration">The application specific configuration.</param>
        public RemoteDatasetDistributor(ISendCommandsToRemoteEndpoints commandHub, IConfiguration configuration)
        {
            {
                Enforce.Argument(() => commandHub);
                Enforce.Argument(() => configuration);
            }

            m_Configuration = configuration;
            m_CommandHub = commandHub;
            {
                // Note that the events may come in on a different thread than the one
                // we're normally accessed on. This is because adding an enpoint is usually 
                // a result of a WCF message being received, on the WCF message thread.
                m_CommandHub.OnEndpointSignedIn += (s, e) => AddNewEndpoint(e.Endpoint, e.Commands);
                m_CommandHub.OnEndpointSignedOff += (s, e) => RemoveEndpoint(e.Endpoint);

                var knownCommands = m_CommandHub.AvailableCommands();
                foreach (var command in knownCommands)
                {
                    AddNewEndpoint(command.Item1, command.Item2);
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
            // Call IDatasetLoaderCommands.Load to start the loading process
            // - Need to somehow transfer the file
            // - Might need to transfer assemblies (or do we let other parts figure that out)
            // - Returns channel connection information which needs to be passed on somehow
            throw new NotImplementedException();
        }
    }
}
