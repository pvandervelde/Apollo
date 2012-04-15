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
using Apollo.Utilities;
using Lokad;
using NManto;

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
        private readonly ISendCommandsToRemoteEndpoints m_CommandHub;

        /// <summary>
        /// The object that receives notifications from remote endpoints.
        /// </summary>
        private readonly INotifyOfRemoteEndpointEvents m_NotificationHub;

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
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDatasetDistributor"/> class.
        /// </summary>
        /// <param name="localDistributor">The object that handles distribution proposals for the local machine.</param>
        /// <param name="loader">The object that handles the actual starting of the dataset application.</param>
        /// <param name="commandHub">The object that sends commands to remote endpoints.</param>
        /// <param name="notificationHub">The object that receives notifications from remote endpoints.</param>
        /// <param name="uploads">The object that stores all the uploads waiting to be started.</param>
        /// <param name="datasetInformationBuilder">The function that builds <see cref="DatasetOnlineInformation"/> objects.</param>
        /// <param name="channelInformation">The function that returns information about the correct channel to use for communication.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks on.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localDistributor"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="loader"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandHub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationHub"/> is <see langword="null" />.
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
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public LocalDatasetDistributor(
            ICalculateDistributionParameters localDistributor,
            IApplicationLoader loader,
            ISendCommandsToRemoteEndpoints commandHub,
            INotifyOfRemoteEndpointEvents notificationHub,
            WaitingUploads uploads,
            Func<DatasetId, EndpointId, NetworkIdentifier, DatasetOnlineInformation> datasetInformationBuilder,
            Func<ChannelConnectionInformation> channelInformation,
            SystemDiagnostics systemDiagnostics,
            TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => localDistributor);
                Enforce.Argument(() => loader);
                Enforce.Argument(() => commandHub);
                Enforce.Argument(() => notificationHub);
                Enforce.Argument(() => datasetInformationBuilder);
                Enforce.Argument(() => channelInformation);
                Enforce.Argument(() => systemDiagnostics);
            }

            m_LocalDistributor = localDistributor;
            m_Loader = loader;
            m_CommandHub = commandHub;
            m_NotificationHub = notificationHub;
            m_Uploads = uploads;
            m_DatasetInformationBuilder = datasetInformationBuilder;
            m_ChannelInformation = channelInformation;
            m_Diagnostics = systemDiagnostics;
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
            using (var interval = m_Diagnostics.Profiler.Measure("Generating local proposal"))
            {
                var proposal = m_LocalDistributor.ProposeForLocalMachine(request.ExpectedLoadPerMachine);
                var plan = new DistributionPlan(
                    (p, t, r) => ImplementPlan(p, t, r),
                    request.DatasetToLoad,
                    new NetworkIdentifier(proposal.Endpoint.OriginatesOnMachine()),
                    proposal);
                return new DistributionPlan[] { plan };
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
                    var endpoint = m_Loader.LoadDataset(m_ChannelInformation());
                    var resetEvent = new AutoResetEvent(false);
                    var commandAvailabilityNotifier = 
                        Observable.FromEventPattern<CommandSetAvailabilityEventArgs>(
                            h => m_CommandHub.OnEndpointSignedIn += h,
                            h => m_CommandHub.OnEndpointSignedIn -= h)
                        .Where(args => args.EventArgs.Endpoint.Equals(endpoint))
                        .Take(1);

                    var notificationAvailabilityNotifier =
                        Observable.FromEventPattern<NotificationSetAvailabilityEventArgs>(
                            h => m_NotificationHub.OnEndpointSignedIn += h,
                            h => m_NotificationHub.OnEndpointSignedIn -= h)
                        .Where(args => args.EventArgs.Endpoint.Equals(endpoint))
                        .Take(1);

                    var availability = commandAvailabilityNotifier.Zip(
                            notificationAvailabilityNotifier,
                            (a, b) => { return true; })
                        .Subscribe(
                            args =>
                            {
                                resetEvent.Set();
                            });

                    using (availability)
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
                        var commands = m_CommandHub.CommandsFor<IDatasetApplicationCommands>(endpoint);
                        var task = commands.Load(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            uploadToken);
                        task.Wait();

                        // Now the dataset loading is complete
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
