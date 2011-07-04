//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base;
using Apollo.Core.UserInterfaces.Projects;
using Apollo.UI.Common.Properties;
using Lokad;
using ICommand = System.Windows.Input.ICommand;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// Defines the viewmodel for a dataset.
    /// </summary>
    public sealed class DatasetModel : Model
    {
        /// <summary>
        /// The dataset that holds the actual data.
        /// </summary>
        private readonly DatasetFacade m_Dataset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="dataset">The dataset.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataset"/> is <see langword="null" />.
        /// </exception>
        public DatasetModel(IContextAware context, DatasetFacade dataset)
            : base(context)
        {
            {
                Enforce.Argument(() => dataset);
            }

            m_Dataset = dataset;
            m_Dataset.OnNameChanged += (s, e) => Notify(() => Name);
            m_Dataset.OnSummaryChanged += (s, e) => Notify(() => Summary);
            m_Dataset.OnLoadingProgress += (s, e) => { }; // @todo: add progress reporting ...
            m_Dataset.OnLoaded += (s, e) =>
                { 
                    Notify(() => this.IsLoaded);
                    Notify(() => this.RunsOn);
                };
            m_Dataset.OnUnloaded += (s, e) =>
                { 
                    Notify(() => this.IsLoaded);
                    Notify(() => this.RunsOn);
                };
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
        public ICommand LoadDatasetCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unload dataset command.
        /// </summary>
        public ICommand UnloadDatasetCommand
        {
            get;
            set;
        }

        // Start & Stop etc.

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
                m_Dataset.Name = value;
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
                m_Dataset.Summary = value;
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
        /// Gets a value indicating whether the dataset can be loaded onto a machine.
        /// </summary>
        public bool CanLoad
        {
            get
            {
                return m_Dataset.CanLoad;
            }
        }

        /// <summary>
        /// Gets the name of the machine on which the dataset is loaded.
        /// </summary>
        public string RunsOn
        {
            get
            {
                return IsLoaded ? m_Dataset.RunsOn().ToString() : Resources.DatasetModel_DatasetIsNotLoaded;
            }
        }

        // PROGRESS (Run / Save etc.)
    }
}
