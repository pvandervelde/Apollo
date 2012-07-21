//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Common.Properties;
using Apollo.Utilities;
using ICommand = System.Windows.Input.ICommand;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// Defines the viewmodel for detail view of the dataset.
    /// </summary>
    public sealed class DatasetDetailModel : Model
    {
        /// <summary>
        /// The project that owns the datset.
        /// </summary>
        private readonly ILinkToProjects m_Project;

        /// <summary>
        /// The dataset that holds the actual data.
        /// </summary>
        private readonly DatasetFacade m_Dataset;

        /// <summary>
        /// The object that handles progress notifications to the application itself.
        /// </summary>
        private readonly ITrackSteppingProgress m_ProgressTracker;

        /// <summary>
        /// Describes the currently running action.
        /// </summary>
        private string m_ProgressDescription;

        /// <summary>
        /// Describes the progress for the current action.
        /// </summary>
        private double m_Progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetDetailModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="progressTracker">The object that handles the progress notifications for the applications.</param>
        /// <param name="project">The project that holds all the data.</param>
        /// <param name="dataset">The dataset.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="progressTracker"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="project"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataset"/> is <see langword="null" />.
        /// </exception>
        public DatasetDetailModel(IContextAware context, ITrackSteppingProgress progressTracker, ILinkToProjects project, DatasetFacade dataset)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => progressTracker);
                Lokad.Enforce.Argument(() => project);
                Lokad.Enforce.Argument(() => dataset);
            }

            m_ProgressTracker = progressTracker;
            m_Project = project;
            m_Dataset = dataset;
            m_Dataset.OnNameChanged += (s, e) => 
                {
                    Notify(() => Name);
                    Notify(() => DisplayName);
                };
            m_Dataset.OnSummaryChanged += (s, e) => Notify(() => Summary);
            m_Dataset.OnProgressOfCurrentAction += HandleDatasetProgress;
            m_Dataset.OnUnloaded += (s, e) =>
                { 
                    Notify(() => this.IsLoaded);
                    Notify(() => this.Endpoint);
                    RaiseOnUnloaded();
                };
        }

        private void HandleDatasetProgress(object sender, ProgressEventArgs e)
        {
            ProgressDescription = e.CurrentlyProcessing.ToString();
            Progress = e.Progress / 100.0;
            if (e.Progress <= 0)
            {
                m_ProgressTracker.StartTracking();
            }

            m_ProgressTracker.UpdateProgress(e.Progress, e.CurrentlyProcessing, e.EstimatedFinishingTime);

            if (e.Progress >= 100)
            {
                m_ProgressTracker.StopTracking();
            }
        }

        /// <summary>
        /// Gets the name of the model for uses on a display.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string DisplayName
        {
            get
            {
                var name = !string.IsNullOrEmpty(Name) ? Name : Resources.DatasetDetailView_UnnamedDatasetName;
                return string.Format(Resources.DatasetDetailView_ViewName, name);
            }
        }

        /// <summary>
        /// Gets or sets the command that closes the script sub-system
        /// and all associated views.
        /// </summary>
        public ICommand CloseCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the dataset.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Dataset.Name;
            }

            set
            {
                using (var set = m_Project.ActiveProject().History.RecordHistory())
                {
                    m_Dataset.Name = value;
                    set.StoreChanges();
                }
            }
        }

        /// <summary>
        /// Gets or sets the summary for the dataset.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Dataset.Summary;
            }

            set
            {
                using (var set = m_Project.ActiveProject().History.RecordHistory())
                {
                    m_Dataset.Summary = value;
                    set.StoreChanges();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is locked against edit changes.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return !m_Dataset.IsEditMode;
            }
        }

        /// <summary>
        /// An event fired after the dataset has been unloaded from the machines it was loaded onto.
        /// </summary>
        public event EventHandler<EventArgs> OnUnloaded;

        private void RaiseOnUnloaded()
        {
            EventHandler<EventArgs> local = OnUnloaded;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is loaded on the local machine
        /// or a remote machine.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return m_Dataset.IsLoaded;
            }
        }

        /// <summary>
        /// Gets the name of the machine on which the dataset is loaded.
        /// </summary>
        public string Endpoint
        {
            get
            {
                return IsLoaded ? m_Dataset.RunsOn().ToString() : Resources.DatasetModel_DatasetIsNotLoaded;
            }
        }

        /// <summary>
        /// Gets the description for the currently executing action.
        /// </summary>
        public string ProgressDescription
        {
            get
            {
                return m_ProgressDescription;
            }

            private set
            {
                m_ProgressDescription = value;
                Notify(() => ProgressDescription);
            }
        }

        /// <summary>
        /// Gets the progress for the currently executing action.
        /// </summary>
        public double Progress
        {
            get
            {
                return m_Progress;
            }

            private set
            {
                var result = value;
                if (result < 0.0)
                {
                    result = 0.0;
                }

                if (result > 1.0)
                {
                    result = 1.0;
                }

                m_Progress = result;
                Notify(() => Progress);
            }
        }

        /// <summary>
        /// Gets or sets the command that switches the dataset to edit mode.
        /// </summary>
        public ICommand SwitchDatasetToEditModeCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command that switches the dataset to executing mode.
        /// </summary>
        public ICommand SwitchDatasetToExecutingModeCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command that whows the advanced details view for the dataset.
        /// </summary>
        public ICommand ShowDatasetAdvancedViewCommand
        {
            get;
            set;
        }
    }
}
