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
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The function which returns a <c>DistributionPlan</c> for a given
        /// <c>DatasetRequest</c>.
        /// </summary>
        private readonly Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> m_DatasetDistributor;

        /// <summary>
        /// Stores the history of the project information.
        /// </summary>
        private readonly ProjectHistoryStorage m_ProjectInformation;

        /// <summary>
        /// The collection of all datasets that belong to the current project.
        /// </summary>
        private readonly DatasetHistoryStorage m_Datasets;

        /// <summary>
        /// The timeline for the current project.
        /// </summary>
        private readonly ITimeline m_Timeline;

        /// <summary>
        /// The ID number of the root dataset.
        /// </summary>
        private DatasetId m_RootDataset;

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
        /// </design>
        private volatile bool m_IsClosed;

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
            m_Timeline.ForgetAllHistory();

            m_Timeline.OnRolledBack += new EventHandler<EventArgs>(OnTimelineRolledBack);
            m_Timeline.OnRolledForward += new EventHandler<EventArgs>(OnTimelineRolledForward);

            m_ProjectInformation = m_Timeline.AddToTimeline<ProjectHistoryStorage>(ProjectHistoryStorage.Build);
            m_Datasets = m_Timeline.AddToTimeline<DatasetHistoryStorage>(DatasetHistoryStorage.Build);

            m_DatasetDistributor = distributor;
            if (persistenceInfo != null)
            {
                RestoreFromStore(persistenceInfo);
            }

            // Create a root dataset if there isn't one
            if (m_RootDataset == null)
            {
                var dataset = CreateDataset(
                    null,
                    new DatasetCreationInformation 
                        { 
                            CreatedOnRequestOf = DatasetCreator.System,
                            LoadFrom = new NullPersistenceInformation(),
                            CanBeDeleted = false,
                            CanBeCopied = false,
                            CanBecomeParent = true,
                            CanBeAdopted = false,
                            IsRoot = true,
                        });

                m_RootDataset = dataset.Id;
                dataset.Name = Resources.Projects_Dataset_RootDatasetName;
                dataset.Summary = Resources.Projects_Dataset_RootDatasetSummary;
            }

            m_Timeline.SetCurrentAsDefault();
        }

        private void OnTimelineRolledBack(object sender, EventArgs args)
        {
            ReloadProjectInformation();
            ReloadProxiesDueToHistoryChange();
        }

        private void ReloadProjectInformation()
        {
            RaiseOnNameChanged(m_ProjectInformation.Name);
            RaiseOnSummaryChanged(m_ProjectInformation.Summary);
        }

        private void ReloadProxiesDueToHistoryChange()
        {
            // this is not very nice but for now there is no way to determine if
            // a dataset was created or deleted.
            RaiseOnDatasetCreated();
            RaiseOnDatasetDeleted();
        }

        private void OnTimelineRolledForward(object sender, EventArgs args)
        {
            ReloadProjectInformation();
            ReloadProxiesDueToHistoryChange();
        }

        private DatasetId RestoreFromStore(IPersistenceInformation persistenceInfo)
        {
            // Restore the dataset here ...
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
        /// Gets the timeline for the project.
        /// </summary>
        public ITimeline History
        {
            get
            {
                return m_Timeline;
            }
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
                return m_ProjectInformation.Name;
            }

            set
            {
                if (!string.Equals(m_ProjectInformation.Name, value))
                {
                    m_ProjectInformation.Name = value;
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
                return m_ProjectInformation.Summary;
            }

            set
            {
                if (!string.Equals(m_ProjectInformation.Summary, value))
                {
                    m_ProjectInformation.Summary = value;
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
                return m_Datasets.Datasets.Count;
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
                    Resources.Exceptions_Messages_CannotUseProjectAfterClosingIt);
            }

            return m_Datasets.Datasets[m_RootDataset];
        }

        /// <summary>
        /// Returns the dataset with the given ID.
        /// </summary>
        /// <param name="id">The ID of the dataset.</param>
        /// <returns>The dataset with the given ID if it exists; otherwise, <see langword="null" />.</returns>
        public IProxyDataset Dataset(DatasetId id)
        { 
            IProxyDataset result = null;
            if (m_Datasets.Datasets.ContainsKey(id))
            {
                result = m_Datasets.Datasets[id];
            }

            return result;
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
                    Resources.Exceptions_Messages_CannotUseProjectAfterClosingIt);
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
                    Resources.Exceptions_Messages_CannotUseProjectAfterClosingIt);
                Enforce.Argument(() => datasetToExport);
                Enforce.With<UnknownDatasetException>(
                    m_Datasets.Datasets.ContainsKey(datasetToExport), 
                    Resources.Exceptions_Messages_UnknownDataset_WithId, 
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
                // Terminate all dataset applications
                foreach (var pair in m_Datasets.Datasets)
                {
                    var dataset = pair.Value;
                    if (dataset.IsLoaded)
                    {
                        CloseOnlineDataset(dataset);
                    }
                }
            }

            RaiseOnClosed();
        }

        private void CloseOnlineDataset(DatasetProxy info)
        {
            info.UnloadFromMachine();
        }

        /// <summary>
        /// Creates a new datset as child of the given parent dataset.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="newChild">The information required to create the new child.</param>
        /// <returns>The new child.</returns>
        internal IProxyDataset CreateDataset(DatasetId parent, DatasetCreationInformation newChild)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to create a new dataset.");
                Debug.Assert(
                    (parent == null) || ((parent != null) && m_Datasets.Datasets.ContainsKey(parent)),
                    "The provided parent node does not exist.");
                Debug.Assert(
                    (parent == null) || ((parent != null) && m_Datasets.Datasets[parent].CanBecomeParent),
                    "The given parent is not allowed to have children.");
            }

            var dataset = CreateNewDatasetProxy(newChild);

            // When adding a new dataset there is no way we can create cycles because
            // we can only add new children to parents, there is no way to link an
            // existing node to the parent.
            lock (m_Lock)
            {
                m_Datasets.Datasets.Add(dataset.Id, dataset);
                m_Datasets.Graph.AddVertex(dataset.Id);
            }

            if (parent != null)
            {
                // Find the actual ID object that we have stored, the caller may have a copy
                // of ID. Using a copy of the real ID might cause issues when connecting the
                // graph so we only use the ID numbers that we have stored.
                var realParent = m_Datasets.Datasets[parent].Id;
                lock (m_Lock)
                {
                    m_Datasets.Graph.AddEdge(new Edge<DatasetId>(realParent, dataset.Id));
                }
            }

            RaiseOnDatasetCreated();
            return dataset;
        }

        private DatasetProxy CreateNewDatasetProxy(DatasetCreationInformation newChild)
        {
            var id = new DatasetId();
            Action<DatasetId> cleanupAction = localId => DeleteDatasetAndChildren(localId, d => { });
            var newDataset = m_Timeline.AddToTimeline<DatasetProxy>(
                DatasetProxy.Build,
                this,
                id,
                newChild,
                m_DatasetDistributor,
                cleanupAction);

            return newDataset;
        }

        /// <summary>
        /// Deletes the given dataset and all its children.
        /// </summary>
        /// <remarks>
        /// The dataset and all its children will be deleted. If any of the datasets are
        /// loaded onto a (remote) machine they will be unloaded just before being deleted.
        /// No data will be saved.
        /// </remarks>
        /// <param name="dataset">The dataset that should be deleted.</param>
        /// <exception cref="CannotDeleteDatasetException">
        /// Thrown when the dataset or one of its children cannot be deleted. The exception
        /// is thrown before any of the datasets are deleted.
        /// </exception>
        internal void DeleteDatasetAndChildren(DatasetId dataset)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to create a new dataset.");
            }

            DeleteDatasetAndChildren(dataset, d => m_Timeline.RemoveFromTimeline(d.HistoryId));
            RaiseOnDatasetDeleted();
        }

        private void DeleteDatasetAndChildren(DatasetId dataset, Action<DatasetProxy> onRemoval)
        {
            if (!m_Datasets.Datasets.ContainsKey(dataset))
            {
                return;
            }

            // Get all the datasets that need to be deleted
            // make sure we do this in an ordered way. We need to 
            // remove the children before we can remove a parent.
            var datasetsToDelete = new Stack<DatasetId>();
            datasetsToDelete.Push(dataset);

            var nodesToProcess = new Queue<DatasetId>();
            nodesToProcess.Enqueue(dataset);

            while (nodesToProcess.Count > 0)
            {
                var node = nodesToProcess.Dequeue();

                Debug.Assert(m_Datasets.Datasets.ContainsKey(node), "The dataset was in the graph but not in the collection.");
                if (!m_Datasets.Datasets[node].CanBeDeleted)
                {
                    throw new CannotDeleteDatasetException();
                }

                var children = Children(node);
                foreach (var child in children)
                {
                    nodesToProcess.Enqueue(child.Id);
                    datasetsToDelete.Push(child.Id);
                }
            }

            while (datasetsToDelete.Count > 0)
            {
                var datasetToDelete = datasetsToDelete.Pop();

                var datasetObject = m_Datasets.Datasets[datasetToDelete];
                if (datasetObject.IsLoaded)
                {
                    CloseOnlineDataset(datasetObject);
                }

                if (onRemoval != null)
                {
                    onRemoval(datasetObject);
                }

                lock (m_Lock)
                {
                    m_Datasets.Graph.RemoveVertex(datasetToDelete);
                    m_Datasets.Datasets.Remove(datasetToDelete);
                }
            }
        }

        /// <summary>
        /// Returns the collection of children for a given dataset.
        /// </summary>
        /// <param name="parent">The ID number of the parent dataset.</param>
        /// <returns>The collection of child datsets.</returns>
        internal IEnumerable<DatasetProxy> Children(DatasetId parent)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to get the children of a dataset.");
            }

            List<Edge<DatasetId>> outEdges;
            lock (m_Lock)
            {
                outEdges = m_Datasets.Graph.OutEdges(parent).ToList();
            }

            var result = from outEdge in outEdges
                         select m_Datasets.Datasets[outEdge.Target];

            return result;
        }
    }
}
