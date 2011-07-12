//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;
using Lokad;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Provide dataset loading proposals for the local machine.
    /// </summary>
    internal sealed class LocalDatasetDistributor : IGenerateDistributionProposals, ILoadDatasets
    {
        /// <summary>
        /// The object that handles the distribution proposals for the local machine.
        /// </summary>
        private readonly ICalculateDistributionParameters m_LocalDistributor;

        /// <summary>
        /// The object that handles the actual starting of the dataset application.
        /// </summary>
        private readonly IApplicationLoader m_Loader;

        /// <summary>
        /// The object that sends commands to remote endpoints.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_Hub;

        /// <summary>
        /// The collection that stores all the uploads waiting to be
        /// started.
        /// </summary>
        private readonly WaitingUploads m_Uploads;

        /// <summary>
        /// The function that returns a <see cref="DatasetOnlineInformation"/>.
        /// </summary>
        private readonly Func<DatasetId, EndpointId, NetworkIdentifier, DatasetOnlineInformation> m_DatasetInformationBuilder;

        /// <summary>
        /// The function that returns information about the channel on which the connection should be made.
        /// </summary>
        private readonly Func<ChannelConnectionInformation> m_ChannelInformation;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDatasetDistributor"/> class.
        /// </summary>
        /// <param name="localDistributor">The object that handles distribution proposals for the local machine.</param>
        /// <param name="loader">The object that handles the actual starting of the dataset application.</param>
        /// <param name="hub">The object that sends commands to remote endpoints.</param>
        /// <param name="uploads">The object that stores all the uploads waiting to be started.</param>
        /// <param name="datasetInformationBuilder">The function that builds <see cref="DatasetOnlineInformation"/> objects.</param>
        /// <param name="channelInformation">The function that returns information about the correct channel to use for communication.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks on.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localDistributor"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="loader"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="hub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="datasetInformationBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelInformation"/> is <see langword="null" />.
        /// </exception>
        public LocalDatasetDistributor(
            ICalculateDistributionParameters localDistributor,
            IApplicationLoader loader,
            ISendCommandsToRemoteEndpoints hub,
            WaitingUploads uploads,
            Func<DatasetId, EndpointId, NetworkIdentifier, DatasetOnlineInformation> datasetInformationBuilder,
            Func<ChannelConnectionInformation> channelInformation,
            TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => localDistributor);
                Enforce.Argument(() => loader);
                Enforce.Argument(() => hub);
                Enforce.Argument(() => datasetInformationBuilder);
                Enforce.Argument(() => channelInformation);
            }

            m_LocalDistributor = localDistributor;
            m_Loader = loader;
            m_Hub = hub;
            m_Uploads = uploads;
            m_DatasetInformationBuilder = datasetInformationBuilder;
            m_ChannelInformation = channelInformation;
            m_Scheduler = scheduler ?? TaskScheduler.Default;
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
            var proposal = m_LocalDistributor.ProposeForLocalMachine(request.ExpectedLoadPerMachine);
            var plan = new DistributionPlan(
                (p, t) => ImplementPlan(p, t),
                request.DatasetToLoad,
                new NetworkIdentifier(proposal.Endpoint.OriginatesOnMachine()),
                proposal);
            return new DistributionPlan[] { plan };
        }

        /// <summary>
        /// Takes the set of distribution plans and loads the given datasets onto the specified machines.
        /// </summary>
        /// <param name="planToImplement">The distribution plan that should be implemented.</param>
        /// <param name="token">The token used to indicate cancellation of the task.</param>
        /// <returns>
        /// A set of objects which allow act as proxies for the loaded datasets.
        /// </returns>
        public Task<DatasetOnlineInformation> ImplementPlan(
            DistributionPlan planToImplement, 
            CancellationToken token)
        {
            Func<DatasetOnlineInformation> result =
                () =>
                {
                    var endpoint = m_Loader.LoadDataset(m_ChannelInformation());
                    var resetEvent = new AutoResetEvent(false);
                    var commandAvailabilityNotifier = 
                        Observable.FromEvent<EventHandler<CommandSetAvailabilityEventArgs>, CommandSetAvailabilityEventArgs>(
                            h => m_Hub.OnEndpointSignedIn += h,
                            h => m_Hub.OnEndpointSignedIn -= h)
                        .Where(args => args.Endpoint.Equals(endpoint))
                        .Take(1)
                        .Subscribe(
                            args =>
                            {
                                resetEvent.Set();
                            });

                    using (commandAvailabilityNotifier)
                    {
                        if (!m_Hub.HasCommandsFor(endpoint))
                        {
                            resetEvent.WaitOne();
                        }
                    }

                    // Store the file 
                    var file = planToImplement.DistributionFor.StoredAt.AsFile();
                    var uploadToken = m_Uploads.Register(file.FullName);

                    // The commands have been registered, so now load the dataset
                    Debug.Assert(m_Hub.HasCommandFor(endpoint, typeof(IDatasetApplicationCommands)), "No application commands registered.");
                    var commands = m_Hub.CommandsFor<IDatasetApplicationCommands>(endpoint);
                    var task = commands.Load(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(), 
                        uploadToken);
                    task.Wait();

                    // Now the dataset loading is complete
                    return m_DatasetInformationBuilder(
                        planToImplement.DistributionFor.Id, 
                        endpoint,
                        planToImplement.MachineToDistributeTo);
                };

            return Task<DatasetOnlineInformation>.Factory.StartNew(
                result, 
                token,
                TaskCreationOptions.LongRunning,
                m_Scheduler);
        }
    }
}
