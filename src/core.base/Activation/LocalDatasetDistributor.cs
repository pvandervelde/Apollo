﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Utilities;
using Lokad;
using Nuclei.Communication;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Provide dataset loading proposals for the local machine.
    /// </summary>
    internal sealed class LocalDatasetDistributor : IGenerateDistributionProposals, IActivateDatasets
    {
        /// <summary>
        /// The object that handles the distribution proposals for the local machine.
        /// </summary>
        private readonly ICalculateDistributionParameters m_LocalDistributor;

        /// <summary>
        /// The object that handles the actual starting of the dataset application.
        /// </summary>
        private readonly IDatasetActivator m_Loader;

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
        /// Initializes a new instance of the <see cref="LocalDatasetDistributor"/> class.
        /// </summary>
        /// <param name="localDistributor">The object that handles distribution proposals for the local machine.</param>
        /// <param name="loader">The object that handles the actual starting of the dataset application.</param>
        /// <param name="commandHub">The object that sends commands to remote endpoints.</param>
        /// <param name="notificationHub">The object that receives notifications from remote endpoints.</param>
        /// <param name="uploads">The object that stores all the uploads waiting to be started.</param>
        /// <param name="datasetInformationBuilder">The function that builds <see cref="DatasetOnlineInformation"/> objects.</param>
        /// <param name="communicationLayer">The object that handles the communication for the application.</param>
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
        ///     Thrown if <paramref name="communicationLayer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public LocalDatasetDistributor(
            ICalculateDistributionParameters localDistributor,
            IDatasetActivator loader,
            ISendCommandsToRemoteEndpoints commandHub,
            INotifyOfRemoteEndpointEvents notificationHub,
            IStoreUploads uploads,
            Func<DatasetId, EndpointId, NetworkIdentifier, DatasetOnlineInformation> datasetInformationBuilder,
            ICommunicationLayer communicationLayer,
            SystemDiagnostics systemDiagnostics,
            TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => localDistributor);
                Enforce.Argument(() => loader);
                Enforce.Argument(() => commandHub);
                Enforce.Argument(() => notificationHub);
                Enforce.Argument(() => datasetInformationBuilder);
                Enforce.Argument(() => communicationLayer);
                Enforce.Argument(() => systemDiagnostics);
            }

            m_LocalDistributor = localDistributor;
            m_Loader = loader;
            m_CommandHub = commandHub;
            m_NotificationHub = notificationHub;
            m_Uploads = uploads;
            m_DatasetInformationBuilder = datasetInformationBuilder;
            m_CommunicationLayer = communicationLayer;
            m_Diagnostics = systemDiagnostics;
            m_Scheduler = scheduler ?? TaskScheduler.Default;
        }

        /// <summary>
        /// Processes the dataset request and returns a collection of distribution plans.
        /// </summary>
        /// <param name="activationRequest">
        /// The request that describes the characteristics of the dataset that 
        /// should be loaded.
        /// </param>
        /// <param name="token">The cancellation token that can be used to terminate the proposal.</param>
        /// <returns>
        /// The collection containing all the distribution plans.
        /// </returns>
        public IEnumerable<DistributionPlan> ProposeDistributionFor(DatasetActivationRequest activationRequest, CancellationToken token)
        {
            using (m_Diagnostics.Profiler.Measure(BaseConstants.TimingGroup, "Generating local proposal"))
            {
                var proposal = m_LocalDistributor.ProposeForLocalMachine(activationRequest.ExpectedLoadPerMachine);
                var plan = new DistributionPlan(
                    ImplementPlan,
                    activationRequest.DatasetToActivate,
                    new NetworkIdentifier(proposal.Endpoint.OriginatesOnMachine()),
                    proposal);
                return new[] { plan };
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
            Action<int, string, bool> progressReporter)
        {
            Func<DatasetOnlineInformation> result =
                () =>
                {
                    m_Diagnostics.Log(LevelToLog.Info, BaseConstants.LogPrefix, "Activating dataset");

                    var info = m_CommunicationLayer.LocalConnectionFor(ChannelType.NamedPipe);
                    EndpointId endpoint;
                    try
                    {
                        endpoint = m_Loader.ActivateDataset(info.Item1, ChannelType.NamedPipe, info.Item2);
                    }
                    catch (Exception e)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Error, 
                            BaseConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Failed to activate the dataset. Error was: {0}",
                                e));
                        throw;
                    }

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
                            m_Diagnostics.Log(LevelToLog.Trace, BaseConstants.LogPrefix, "Waiting for dataset to connect.");

                            resetEvent.WaitOne();
                        }
                    }

                    m_Diagnostics.Log(LevelToLog.Trace, "Received commands and notifications from dataset.");

                    IDatasetApplicationNotifications notifications;
                    try
                    {
                        notifications = m_NotificationHub.NotificationsFor<IDatasetApplicationNotifications>(endpoint);
                    }
                    catch (Exception e)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Error,
                            BaseConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Failed to get the notifications. Error was: {0}",
                                e));
                        throw;
                    }

                    EventHandler<ProgressEventArgs> progressHandler =
                        (s, e) => progressReporter(e.Progress, e.Description, e.HasErrors);
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
