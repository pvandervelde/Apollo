//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Loaders;
using Lokad;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// Defines the view model for the selection of a machine onto which a dataset will be loaded.
    /// </summary>
    public sealed class MachineSelectorModel : Model
    {
        /// <summary>
        /// The collection that contains all the plans.
        /// </summary>
        private readonly ObservableCollection<DistributionSuggestion> m_Proposals
            = new ObservableCollection<DistributionSuggestion>();

        /// <summary>
        /// Indicates that the collection is loading.
        /// </summary>
        private bool m_IsLoading = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineSelectorModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="suggestions">The collection containing the dataset loading suggestions.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks on.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="suggestions"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
            Justification = "The use of default parameters makes things simpler here.")]
        public MachineSelectorModel(
            IContextAware context, 
            IEnumerable<DistributionSuggestion> suggestions,
            TaskScheduler scheduler = null)
            : base(context)
        {
            {
                Enforce.Argument(() => suggestions);
            }

            Action action =
                () =>
                {
                    IsLoading = true;
                    foreach (var suggestion in suggestions)
                    {
                        if (context.IsSynchronized)
                        {
                            m_Proposals.Add(suggestion);
                        }
                        else
                        {
                            context.Invoke(() => m_Proposals.Add(suggestion));
                        }
                    }

                    IsLoading = false;
                };

            Task.Factory.StartNew(
                action, 
                new CancellationToken(),
                TaskCreationOptions.LongRunning,
                scheduler ?? TaskScheduler.Default);
        }

        /// <summary>
        /// Gets the collection containing all the proposals.
        /// </summary>
        public ObservableCollection<DistributionSuggestion> AvailableProposals
        {
            get
            {
                return m_Proposals;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the loading of the 
        /// collection of proposals is finished.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return m_IsLoading;
            }
            
            private set
            {
                if (value != m_IsLoading)
                {
                    m_IsLoading = value;
                    Notify(() => IsLoading);
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected plan.
        /// </summary>
        public DistributionPlan SelectedPlan
        {
            get;
            set;
        }
    }
}
