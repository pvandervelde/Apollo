//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;
using Apollo.Utilities.History;
using Lokad;
using QuickGraph;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Stores all the information for a project, a collection of 
    /// datasets, their relations and general metadata.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A project contains information about a specific set of physical
    /// situations. The actual physical data for these situations is 
    /// described by the hierarchical set of datasets.
    /// </para>
    /// </remarks>
    internal sealed partial class Project : IProject, ICanClose
    {
        /// <summary>
        /// The function which returns a <c>DistributionPlan</c> for a given
        /// <c>DatasetRequest</c>.
        /// </summary>
        private readonly Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> m_DatasetDistributor;

        /// <summary>
        /// A flag that indicates if the project has been closed.
        /// </summary>
        /// <design>
        /// <para>
        /// This flag may be set in a multi-threaded environment, fortunately setting a boolean 
        /// value is always an atomic operation (see the VS2010 C# language specification
        /// paragraph 5.5, as quoted below). However we still need the 'volatile' marker to 
        /// ensure that the compiler inserts the correct read/write barriers. This ensures that
        /// the reads and writes aren't re-ordered. The 'volatile' switch also ensures that the
        /// data is written straight back into the RAM memory so that other processors can see it.
        /// </para>
        /// <para>
        /// VS2010 C# language specification. Section 5.5.
        /// <quote>
        /// Reads and writes of the following data types are atomic: bool, char, byte, sbyte, 
        /// short, ushort, uint, int, float, and reference types. In addition, reads and writes 
        /// of enum types with an underlying type in the previous list are also atomic. Reads and 
        /// writes of other types, including long, ulong, double, and decimal, as well as user-defined 
        /// types, are not guaranteed to be atomic. Aside from the library functions designed for that 
        /// purpose, there is no guarantee of atomic read-modify-write, such as in the case of 
        /// increment or decrement.
        /// </quote>
        /// </para>
        /// </design>
        private volatile bool m_IsClosed;

        /// <summary>
        /// The name of the project.
        /// </summary>
        private string m_Name;

        /// <summary>
        /// The summary for the project.
        /// </summary>
        private string m_Summary;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="timeline">The timeline for the current project.</param>
        /// <param name="distributor">
        /// The function which returns a <see cref="DistributionPlan"/> for a given
        /// <see cref="DatasetRequest"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="distributor"/> is <see langword="null" />.
        /// </exception>
        public Project(
            ITimeline timeline,
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor)
            : this(timeline, distributor, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="timeline">The timeline for the current project.</param>
        /// <param name="distributor">
        /// The function which returns a <see cref="DistributionPlan"/> for a given
        /// <see cref="DatasetRequest"/>.
        /// </param>
        /// <param name="persistenceInfo">
        /// The object that describes how the project was persisted.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="timeline"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="distributor"/> is <see langword="null" />.
        /// </exception>
        public Project(
            ITimeline timeline,
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor, 
            IPersistenceInformation persistenceInfo)
        {
            {
                Enforce.Argument(() => timeline);
                Enforce.Argument(() => distributor);
            }

            m_Timeline = timeline;
            m_Timeline.OnRolledBack += new EventHandler<EventArgs>(OnTimelineRolledBack);
            m_Timeline.OnRolledForward += new EventHandler<EventArgs>(OnTimelineRolledForward);
            
            m_Datasets = m_Timeline.AddToTimeline<DatasetHistoryStorage>(BuildDatasetHistoryStorage);

            m_DatasetDistributor = distributor;
            if (persistenceInfo != null)
            {
                RestoreFromStore(persistenceInfo);
            }

            // Create a root dataset if there isn't one
            if (m_RootDataset == null)
            {
                m_RootDataset = CreateDataset(
                    null,
                    new DatasetCreationInformation 
                        { 
                            CreatedOnRequestOf = DatasetCreator.System,
                            LoadFrom = new NullPersistenceInformation(),
                            CanBeDeleted = false,
                            CanBeCopied = false,
                            CanBecomeParent = true,
                            CanBeAdopted = false,
                        });

                var dataset = m_Datasets.KnownDatasets[m_RootDataset];
                dataset.Name = Resources.Projects_Dataset_RootDatasetName;
                dataset.Summary = Resources.Projects_Dataset_RootDatasetSummary;
            }
        }

        private DatasetId RestoreFromStore(IPersistenceInformation persistenceInfo)
        {
            // Restore the project here ...
            // Probably needs to be version safe etc.
            // Note that we also need to store the stream somewhere 
            // so that we always have access to it (which will probably be on disk)
            // this may cause all kinds of untold chaos, e.g.
            // - disk full / not big enough
            // - The stream is a remote stream and cuts out half way (i.e we don't consume it on time)
            //
            //
            // When creating a dataset check:
            // - The root must not have parents
            // - Adding the dataset should not create any cycles
            // - No parent may become a child of a child node
            //   OR better yet, no node may become a child after it is inserted
            return new DatasetId();
        }

        /// <summary>
        /// Gets a value indicating whether the project has been closed.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return m_IsClosed;
            }
        }

        /// <summary>
        /// The event raised when the project is closed.
        /// </summary>
        public event EventHandler<EventArgs> OnClosed;

        private void RaiseOnClosed()
        {
            var local = OnClosed;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the name of the project.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                if (!string.Equals(m_Name, value))
                {
                    m_Name = value;
                    RaiseOnNameChanged(value);
                }
            }
        }

        /// <summary>
        /// An event raised when the name of a project is changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<string>> OnNameChanged;

        private void RaiseOnNameChanged(string newName)
        {
            var local = OnNameChanged;
            if (local != null)
            {
                local(this, new ValueChangedEventArgs<string>(newName));
            }
        }

        /// <summary>
        /// Gets or sets a value describing the project.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Summary;
            }

            set
            {
                if (!string.Equals(m_Summary, value))
                {
                    m_Summary = value;
                    RaiseOnSummaryChanged(value);
                }
            }
        }

        /// <summary>
        /// An event raised when the summary of a project is changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<string>> OnSummaryChanged;

        private void RaiseOnSummaryChanged(string newSummary)
        {
            var local = OnSummaryChanged;
            if (local != null)
            {
                local(this, new ValueChangedEventArgs<string>(newSummary));
            }
        }

        /// <summary>
        /// Gets a value indicating the number of dataset for the project.
        /// </summary>
        public int NumberOfDatasets
        {
            get
            {
                return m_Datasets.KnownDatasets.Count;
            }
        }

        /// <summary>
        /// The event raised when a new dataset is created and added to the project.
        /// </summary>
        public event EventHandler<EventArgs> OnDatasetCreated;

        private void RaiseOnDatasetCreated()
        {
            var local = OnDatasetCreated;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The event raised when a dataset is deleted from the project.
        /// </summary>
        public event EventHandler<EventArgs> OnDatasetDeleted;

        private void RaiseOnDatasetDeleted()
        {
            var local = OnDatasetDeleted;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns a read-only view of the dataset on which all the other datasets are based.
        /// </summary>
        /// <returns>
        /// The read-only view of the base dataset.
        /// </returns>
        public IProxyDataset BaseDataset()
        {
            {
                Enforce.With<CannotUseProjectAfterClosingItException>(
                    !IsClosed, 
                    Resources_NonTranslatable.Exception_Messages_CannotUseProjectAfterClosingIt);
            }

            return ObtainProxyFor(m_RootDataset);
        }

        /// <summary>
        /// Saves the project and all the datasets to the given stream.
        /// </summary>
        /// <param name="persistenceInfo">
        /// The object that describes how the project should be persisted.
        /// </param>
        /// <remarks>
        /// Note that saving project and dataset information to a stream on the local machine may take
        /// some time because the datasets may be large, reside on a remote machine or both.
        /// </remarks>
        public void Save(IPersistenceInformation persistenceInfo)
        {
            {
                Enforce.With<CannotUseProjectAfterClosingItException>(
                    !IsClosed, 
                    Resources_NonTranslatable.Exception_Messages_CannotUseProjectAfterClosingIt);
                Enforce.Argument(() => persistenceInfo);
            }

            // Do we need to have a save flag that we can set to prevent closing from happening
            // while saving?
            throw new NotImplementedException();
        }

        /// <summary>
        /// Exports the given dataset as the base of a new project.
        /// </summary>
        /// <param name="datasetToExport">
        /// The ID number of the dataset that should be exported.
        /// </param>
        /// <param name="shouldIncludeChildren">
        /// Indicates if all the child datasets of <paramref name="datasetToExport"/> should be included in the
        /// export or not.
        /// </param>
        /// <param name="persistenceInfo">
        /// The object that describes how the dataset should be exported.
        /// </param>
        /// <remarks>
        /// Note that saving project and dataset information to a stream on the local machine may take
        /// some time because the datasets may be large, reside on a remote machine or both.
        /// </remarks>
        public void Export(DatasetId datasetToExport, bool shouldIncludeChildren, IPersistenceInformation persistenceInfo)
        {
            {
                Enforce.With<CannotUseProjectAfterClosingItException>(
                    !IsClosed, 
                    Resources_NonTranslatable.Exception_Messages_CannotUseProjectAfterClosingIt);
                Enforce.Argument(() => datasetToExport);
                Enforce.With<UnknownDatasetException>(
                    m_Datasets.KnownDatasets.ContainsKey(datasetToExport), 
                    Resources_NonTranslatable.Exception_Messages_UnknownDataset_WithId, 
                    datasetToExport);
                Enforce.Argument(() => persistenceInfo);
            }

            // Do we need to have a save flag that we can set to prevent closing from happening
            // while saving?
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops all external datasets from running, unloads them from their machines and then prepares
        /// the project for shut-down.
        /// </summary>
        /// <design>
        /// We do not want anybody to call the close, except for the owner of the project (i.e. the 
        /// project service). This is because when we close a project we need the project service to
        /// perform certain actions (like removing the project etc.). So closing should always be
        /// done through the project owner.
        /// </design>
        public void Close()
        {
            // Indicate that we're closing the project. Do this first so that any actions that come
            // in parallel to this one will be notified.
            m_IsClosed = true;

            // @todo: We should only close if we're not saving data. 
            //        If we are saving data then wait till we're done, then close.
            // @todo: We should only close if we're not loading
            //        Technically we should abort the load
            lock (m_Lock)
            {
                // Invalidate all datasets first.
                m_DatasetProxies.Clear();

                // Terminate all dataset applications
                foreach (var online in m_Datasets.ActiveDatasets.Values)
                {
                    online.Close();
                }
            }

            RaiseOnClosed();
        }
    }
}
