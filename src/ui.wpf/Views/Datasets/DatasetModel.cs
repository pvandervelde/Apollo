﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Wpf.Properties;
using Apollo.Utilities;
using Lokad;
using ICommand = System.Windows.Input.ICommand;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// Defines the view model for a dataset.
    /// </summary>
    public sealed class DatasetModel : Model
    {
        /// <summary>
        /// The project that owns the dataset.
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
        /// Initializes a new instance of the <see cref="DatasetModel"/> class.
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
        public DatasetModel(IContextAware context, ITrackSteppingProgress progressTracker, ILinkToProjects project, DatasetFacade dataset)
            : base(context)
        {
            {
                Enforce.Argument(() => progressTracker);
                Enforce.Argument(() => project);
                Enforce.Argument(() => dataset);
            }

            m_ProgressTracker = progressTracker;
            m_Project = project;
            m_Dataset = dataset;
            m_Dataset.OnNameChanged += (s, e) => Notify(() => Name);
            m_Dataset.OnSummaryChanged += (s, e) => Notify(() => Summary);
            m_Dataset.OnProgressOfCurrentAction += HandleDatasetProgress;
            m_Dataset.OnActivated += HandleDatasetOnActivated;
            m_Dataset.OnDeactivated += (s, e) =>
                { 
                    Notify(() => IsActivated);
                    Notify(() => RunsOn);
                    RaiseOnDeactivated();
                };
        }

        private void HandleDatasetProgress(object sender, ProgressEventArgs e)
        {
            ProgressDescription = e.Description;
            Progress = e.Progress / 100.0;
            if (e.Progress <= 0)
            {
                m_ProgressTracker.StartTracking();
            }

            m_ProgressTracker.UpdateProgress(e.Progress, e.Description, e.HasErrors);

            if (e.Progress >= 100)
            {
                m_ProgressTracker.StopTracking();
            }
        }

        private void HandleDatasetOnActivated(object sender, EventArgs e)
        {
            Notify(() => IsActivated);
            Notify(() => RunsOn);

            Progress = 0.0;
            ProgressDescription = string.Empty;
            m_ProgressTracker.StopTracking();

            RaiseOnActivated();
        }

        /// <summary>
        /// Gets or sets the new child dataset command.
        /// </summary>
        public ICommand NewChildDatasetCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the delete dataset command.
        /// </summary>
        public ICommand DeleteDatasetCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the load dataset command.
        /// </summary>
        public ICommand ActivateDatasetCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unload dataset command.
        /// </summary>
        public ICommand DeactivateDatasetCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command used to show the detail view.
        /// </summary>
        public ICommand ShowDetailViewCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the ID of the dataset.
        /// </summary>
        public DatasetId Id
        {
            get
            {
                return m_Dataset.Id;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be moved from one parent
        /// to another parent.
        /// </summary>
        /// <design>
        /// Datasets created by the user are normally movable. Datasets created by the system
        /// may not be movable because it doesn't make sense to move a dataset whose only purpose
        /// is to provide information to the parent.
        /// </design>
        public bool CanBeAdopted
        {
            get
            {
                return m_Dataset.CanBeAdopted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be copied to another
        /// dataset.
        /// </summary>
        public bool CanBeCopied
        {
            get
            {
                return m_Dataset.CanBeCopied;
            }
        }

        /// <summary>
        /// Gets a value indicating who created the dataset.
        /// </summary>
        public DatasetCreator CreatedBy
        {
            get
            {
                return m_Dataset.CreatedBy;
            }
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
        /// An event fired after the dataset has been activated.
        /// </summary>
        public event EventHandler<EventArgs> OnActivated;

        private void RaiseOnActivated()
        {
            EventHandler<EventArgs> local = OnActivated;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event fired after the dataset has been deactivated.
        /// </summary>
        public event EventHandler<EventArgs> OnDeactivated;

        private void RaiseOnDeactivated()
        {
            EventHandler<EventArgs> local = OnDeactivated;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is loaded on the local machine
        /// or a remote machine.
        /// </summary>
        public bool IsActivated
        {
            get
            {
                return m_Dataset.IsActivated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset can be loaded onto a machine.
        /// </summary>
        public bool CanActivate
        {
            get
            {
                return m_Dataset.CanActivate;
            }
        }

        /// <summary>
        /// Gets the name of the machine on which the dataset is loaded.
        /// </summary>
        public string RunsOn
        {
            get
            {
                return IsActivated ? m_Dataset.RunsOn().ToString() : Resources.DatasetModel_DatasetIsNotActivated;
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
    }
}
