//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines the plan for distributing a dataset across one or more machines.
    /// </summary>
    public sealed class DistributionPlan
    {
        /// <summary>
        /// The function that is used to load the dataset on to the machine that proposed the current plan.
        /// </summary>
        private readonly Func<DistributionPlan, CancellationToken, Action<int, IProgressMark>, Task<DatasetOnlineInformation>> m_Loader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionPlan"/> class.
        /// </summary>
        /// <param name="loader">
        ///     The function that provides the ability to load the dataset onto the selected machine
        ///     if the current distribution plan should be accepted.
        /// </param>
        /// <param name="dataset">The ID of the datset that should be loaded.</param>
        /// <param name="machine">
        ///     The machine onto which the dataset will be loaded if the current plan is accepted.
        /// </param>
        /// <param name="proposal">
        ///     The proposal that provides the estimated performance of loading the given
        ///     dataset onto the given machine.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="loader"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataset"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="machine"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="proposal"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Loading a dataset is a time consuming task hence the return value of the function is a Task<T>.")]
        public DistributionPlan(
            Func<DistributionPlan, CancellationToken, Action<int, IProgressMark>, Task<DatasetOnlineInformation>> loader,
            DatasetOfflineInformation dataset,
            NetworkIdentifier machine,
            DatasetLoadingProposal proposal)
        {
            {
                Enforce.Argument(() => loader);
                Enforce.Argument(() => dataset);
                Enforce.Argument(() => machine);
                Enforce.Argument(() => proposal);
            }

            m_Loader = loader;
            DistributionFor = dataset;
            MachineToDistributeTo = machine;
            Proposal = proposal;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the dataset for which
        /// this distribution plan is valid.
        /// </summary>
        public DatasetOfflineInformation DistributionFor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the identifier of the machine onto which the dataset would be loaded
        /// if this plan is accepted.
        /// </summary>
        public NetworkIdentifier MachineToDistributeTo
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the proposal that provides the estimated performance of loading the
        /// dataset on the given machine.
        /// </summary>
        public DatasetLoadingProposal Proposal
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a collection of objects describing the datasets that have 
        /// been loaded through the execution of the current distribution plan.
        /// </summary>
        /// <param name="token">The cancellation token that is used to cancel the loading action.</param>
        /// <param name="progressReporter">The action that handles the reporting of progress.</param>
        /// <returns>
        /// The collection of objects describing the activated datasets.
        /// </returns>
        public Task<DatasetOnlineInformation> Accept(CancellationToken token, Action<int, IProgressMark> progressReporter)
        {
            return m_Loader(this, token, progressReporter);
        }
    }
}
