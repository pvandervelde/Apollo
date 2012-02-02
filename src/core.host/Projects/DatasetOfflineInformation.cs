//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;
using Apollo.Utilities.History;
using Lokad;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Stores persistent information about a dataset. This information will be loaded
    /// even if the dataset isn't loaded.
    /// </summary>
    internal sealed class DatasetOfflineInformation : IDatasetOfflineInformation, IAmHistoryEnabled, INeedCleanupBeforeRemovalFromHistory
    {
        /// <summary>
        /// Returns the name of the <see cref="Name"/> field.
        /// </summary>
        /// <remarks>FOR INTERNAL USE ONLY!</remarks>
        /// <returns>The name of the field.</returns>
        internal static string NameOfNameField()
        {
            return ReflectionExtensions.MemberName<DatasetOfflineInformation, IVariableTimeline<string>>(
                p => p.m_Name);
        }

        /// <summary>
        /// Returns the name of the <see cref="Summary"/> field.
        /// </summary>
        /// <remarks>FOR INTERNAL USE ONLY!</remarks>
        /// <returns>The name of the field.</returns>
        internal static string NameOfSummaryField()
        {
            return ReflectionExtensions.MemberName<DatasetOfflineInformation, IVariableTimeline<string>>(
                p => p.m_Summary);
        }

        /// <summary>
        /// The ID number of the dataset.
        /// </summary>
        private readonly DatasetId m_Id;

        /// <summary>
        /// The ID used by the timeline to uniquely identify the current object.
        /// </summary>
        private readonly HistoryId m_HistoryId;

        /// <summary>
        /// The function that handles the clean-up if the dataset is removed.
        /// </summary>
        private readonly Action<DatasetId> m_Cleanup;

        /// <summary>
        /// The name of the project.
        /// </summary>
        private readonly IVariableTimeline<string> m_Name;

        /// <summary>
        /// The summary for the project.
        /// </summary>
        private readonly IVariableTimeline<string> m_Summary;

        /// <summary>
        /// The object that describes how the dataset was persisted.
        /// </summary>
        private readonly IPersistenceInformation m_LoadFrom;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetOfflineInformation"/> class.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <param name="historyId">The ID for use in the history system.</param>
        /// <param name="constructionReason">The object describing why the dataset was created.</param>
        /// <param name="cleanup">The function that handles the clean-up if the dataset is removed.</param>
        /// <param name="name">The datastructure that stores the history information for the name of the dataset.</param>
        /// <param name="summary">The datastructure that stores the history information for the summary of the dataset.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="historyId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="constructionReason"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="CannotCreateDatasetWithoutCreatorException">
        ///     Thrown if <paramref name="constructionReason"/> defines a creator as <see cref="DatasetCreator.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="cleanup"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="summary"/> is <see langword="null" />.
        /// </exception>
        public DatasetOfflineInformation(
            DatasetId id, 
            HistoryId historyId, 
            DatasetCreationInformation constructionReason,
            Action<DatasetId> cleanup,
            IVariableTimeline<string> name,
            IVariableTimeline<string> summary)
        {
            {
                Enforce.Argument(() => id);
                Enforce.Argument(() => constructionReason);
                Enforce.With<CannotCreateDatasetWithoutCreatorException>(
                    constructionReason.CreatedOnRequestOf != DatasetCreator.None,
                    Resources_NonTranslatable.Exceptions_Messages_CannotCreateDatasetWithoutCreator);
                Enforce.Argument(() => cleanup);
                Enforce.Argument(() => name);
                Enforce.Argument(() => summary);
            }

            m_Id = id;
            CreatedBy = constructionReason.CreatedOnRequestOf;
            CanBecomeParent = constructionReason.CanBecomeParent;
            CanBeAdopted = constructionReason.CanBeAdopted;
            CanBeCopied = constructionReason.CanBeCopied;
            CanBeDeleted = constructionReason.CanBeDeleted;
            m_LoadFrom = constructionReason.LoadFrom;

            m_Cleanup = cleanup;
            m_HistoryId = historyId;
            m_Name = name;
            m_Summary = summary;

            m_Name.OnExternalValueUpdate += new EventHandler<EventArgs>(HandleOnNameUpdate);
            m_Summary.OnExternalValueUpdate += new EventHandler<EventArgs>(HandleOnSummaryUpdate);
        }

        private void HandleOnSummaryUpdate(object sender, EventArgs e)
        {
            RaiseOnHistoryHasUpdatedData();
        }

        private void HandleOnNameUpdate(object sender, EventArgs e)
        {
            RaiseOnHistoryHasUpdatedData();
        }

        /// <summary>
        /// Gets a value indicating the ID number of the dataset for 
        /// which the persistence information is stored.
        /// </summary>
        public DatasetId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the ID which relates the object to the timeline.
        /// </summary>
        public HistoryId HistoryId
        {
            get
            {
                return m_HistoryId;
            }
        }

        /// <summary>
        /// Gets a value indicating who created the dataset.
        /// </summary>
        public DatasetCreator CreatedBy
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the current dataset is allowed to request the 
        /// creation of its own children.
        /// </summary>
        /// <design>
        /// Normally all datasets created by the user are allowed to create their own 
        /// children. In some cases datasets created by the system are blocked from 
        /// creating their own children.
        /// </design>
        public bool CanBecomeParent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be deleted from the
        /// project.
        /// </summary>
        public bool CanBeDeleted
        {
            get;
            private set;
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
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be copied to another
        /// dataset.
        /// </summary>
        public bool CanBeCopied
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating where and how the dataset was persisted.
        /// </summary>
        public IPersistenceInformation StoredAt
        {
            get
            {
                return m_LoadFrom;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the name of the dataset.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name.Current;
            }

            set
            {
                m_Name.Current = value;
            }
        }

        /// <summary>
        /// Gets or sets a value describing the dataset.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Summary.Current;
            }

            set
            {
                m_Summary.Current = value;
            }
        }

        /// <summary>
        /// Raised when the information about the dataset is changed due to changes in the timeline.
        /// </summary>
        public event EventHandler<EventArgs> OnHistoryHasUpdatedData;

        private void RaiseOnHistoryHasUpdatedData()
        {
            var local = OnHistoryHasUpdatedData;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Provides implementing classes with the ability to clean-up before
        /// the object is removed from history.
        /// </summary>
        public void CleanupBeforeRemovalFromHistory()
        {
            m_Name.OnExternalValueUpdate -= new EventHandler<EventArgs>(HandleOnNameUpdate);
            m_Summary.OnExternalValueUpdate -= new EventHandler<EventArgs>(HandleOnSummaryUpdate);
            m_Cleanup(m_Id);
        }
    }
}
