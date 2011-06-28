//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        /// The object that handles the communication between applications.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

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
        /// Initializes a new instance of the <see cref="LocalDatasetDistributor"/> class.
        /// </summary>
        /// <param name="localDistributor">The object that handles distribution proposals for the local machine.</param>
        /// <param name="loader">The object that handles the actual starting of the dataset application.</param>
        /// <param name="layer">The object that handles the communication between applications.</param>
        /// <param name="hub">The object that sends commands to remote endpoints.</param>
        /// <param name="uploads">The object that stores all the uploads waiting to be started.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localDistributor"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="loader"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="hub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        public LocalDatasetDistributor(
            ICalculateDistributionParameters localDistributor,
            IApplicationLoader loader,
            ICommunicationLayer layer,
            ISendCommandsToRemoteEndpoints hub,
            WaitingUploads uploads)
        {
            {
                Enforce.Argument(() => localDistributor);
                Enforce.Argument(() => loader);
                Enforce.Argument(() => layer);
                Enforce.Argument(() => hub);
            }

            m_LocalDistributor = localDistributor;
            m_Loader = loader;
            m_Layer = layer;
            m_Hub = hub;
            m_Uploads = uploads;
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
        public Task<DatasetOnlineInformation> ImplementPlan(DistributionPlan planToImplement, CancellationToken token)
        {
            Func<DatasetOnlineInformation> result =
                () =>
                {
                    var connection = (from i in m_Layer.LocalConnectionPoints()
                                      where i.ChannelType.Equals(typeof(NamedPipeChannelType))
                                      select i).First();
                    var endpoint = m_Loader.LoadDataset(connection);
                    
                    // Wait for the application to start up and register itself with our command hub.
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

                    // Store the file 
                    var file = planToImplement.DistributionFor.StoredAt.AsFile();
                    var uploadToken = m_Uploads.Register(file.FullName);

                    // The commands have been registered, so now load the dataset
                    Debug.Assert(m_Hub.HasCommandFor(endpoint, typeof(IDatasetApplicationCommands)), "No application commands registered.");
                    var commands = m_Hub.CommandsFor<IDatasetApplicationCommands>(endpoint);
                    var task = commands.Load(m_Layer.Id, uploadToken);
                    task.Wait();

                    // Now the dataset loading is complete
                    return new DatasetOnlineInformation(
                        planToImplement.DistributionFor.Id, 
                        planToImplement.Proposal.Endpoint,
                        m_Hub);
                };

            return Task<DatasetOnlineInformation>.Factory.StartNew(result, token);
        }
    }
}
