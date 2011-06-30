//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using QuickGraph;

namespace Apollo.Core.Projects
{
    /// <content>
    /// Stores all information about the datasets.
    /// </content>
    internal sealed partial class Project
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The graph that describes the relations between the different datasets.
        /// </summary>
        private readonly BidirectionalGraph<DatasetId, Edge<DatasetId>> m_Graph;

        /// <summary>
        /// The collection of all datasets that belong to the current project.
        /// </summary>
        private readonly IDictionary<DatasetId, DatasetOfflineInformation> m_Datasets =
            new ConcurrentDictionary<DatasetId, DatasetOfflineInformation>();

        /// <summary>
        /// The collection of all datasets which are currently loaded onto a machine.
        /// </summary>
        private readonly IDictionary<DatasetId, DatasetOnlineInformation> m_ActiveDatasets =
            new ConcurrentDictionary<DatasetId, DatasetOnlineInformation>();

        /// <summary>
        /// The collection that holds the proxies for the datasets that we are mirroring 
        /// for the UI.
        /// </summary>
        private readonly IDictionary<DatasetId, IOwnedProxyDataset> m_DatasetProxies =
            new ConcurrentDictionary<DatasetId, IOwnedProxyDataset>();

        /// <summary>
        /// The collection that holds the ID numbers of all datasets that are currently
        /// being loaded.
        /// </summary>
        /// <remarks>
        /// It is expected that there will never be that many entries in this collection.
        /// </remarks>
        private readonly IDictionary<DatasetId, object> m_LoadingDatsets =
            new ConcurrentDictionary<DatasetId, object>();

        /// <summary>
        /// The ID number of the root dataset.
        /// </summary>
        private DatasetId m_RootDataset;

        /// <summary>
        /// Returns a value indicating if the dataset is still valid.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <returns>
        /// <see langword="true" /> if the dataset is valid; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private bool IsValid(DatasetId id)
        {
            Debug.Assert(id != null, "The ID should not be a null reference.");
            return !IsClosed && m_Datasets.ContainsKey(id);
        }

        /// <summary>
        /// Returns a proxy object for the given dataset.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <returns>The requested proxy.</returns>
        private IProxyDataset ObtainProxyFor(DatasetId id)
        {
            {
                Debug.Assert(IsValid(id), "Cannot create a proxy for an invalid ID.");
            }

            if (!m_DatasetProxies.ContainsKey(id))
            {
                m_DatasetProxies.Add(id, new DatasetProxy(this, id));
            }

            return m_DatasetProxies[id];
        }

        /// <summary>
        /// Returns a value indicating if the given dataset is loaded onto one or
        /// more machines.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <returns>
        /// <see langword="true" /> if the dataset is loaded; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private bool IsLoaded(DatasetId id)
        {
            Debug.Assert(id != null, "The ID should not be a null reference.");
            return m_ActiveDatasets.ContainsKey(id);
        }

        /// <summary>
        /// Returns a value indicating whether the dataset can be loaded onto a machine.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <returns>
        ///     <see langword="true" /> if the dataset can be loaded; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private bool CanLoad(DatasetId id)
        {
            Debug.Assert(id != null, "The ID should not be a null reference.");
            return !id.Equals(m_RootDataset) && !m_LoadingDatsets.ContainsKey(id);
        }

        /// <summary>
        /// Returns the offline information for the given dataset.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <returns>The requested information.</returns>
        private DatasetOfflineInformation OfflineInformation(DatasetId id)
        {
            {
                Debug.Assert(m_Datasets.ContainsKey(id), "Unknown dataset ID found.");
            }

            return m_Datasets[id];
        }

        /// <summary>
        /// Returns the online information for the given dataset. 
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <returns>The requested information.</returns>
        private DatasetOnlineInformation OnlineInformation(DatasetId id)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to get the online information.");
                Debug.Assert(m_ActiveDatasets.ContainsKey(id), "Unknown dataset ID found.");
            }

            return m_ActiveDatasets[id];
        }

        /// <summary>
        /// Creates a new datset as child of the given parent dataset.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="newChild">The information required to create the new child.</param>
        /// <returns>The ID number of the new child.</returns>
        private DatasetId CreateDataset(DatasetId parent, DatasetCreationInformation newChild)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to create a new dataset.");
            }

            var id = new DatasetId();
            var newDataset = new DatasetOfflineInformation(id, newChild);
            m_Datasets.Add(id, newDataset);

            // When adding a new dataset there is no way we can create cycles because
            // we can only add new children to parents, there is no way to link an
            // existing node to the parent.
            lock (m_Lock)
            {
                m_Graph.AddVertex(id);
            }

            if (parent != null)
            {
                // Check if the parent can have a child.
                {
                    Debug.Assert(m_Datasets.ContainsKey(parent), "The provided parent node does not exist.");
                    Debug.Assert(m_Datasets[parent].CanBecomeParent, "The given parent is not allowed to have children.");
                }

                // Find the actual ID object that we have stored, the caller may have a copy
                // of ID. Using a copy of the real ID might cause issues when connecting the
                // graph so we only use the ID numbers that we have stored.
                var realParent = m_Datasets[parent].Id;
                lock (m_Lock)
                {
                    m_Graph.AddEdge(new Edge<DatasetId>(realParent, id));
                }
            }

            RaiseOnDatasetCreated();

            return id;
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
        private void DeleteDatasetAndChildren(DatasetId dataset)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to create a new dataset.");
            }

            if (!IsValid(dataset))
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

                Debug.Assert(m_Datasets.ContainsKey(node), "The dataset was in the graph but not in the collection.");
                if (!m_Datasets[node].CanBeDeleted)
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

                if (IsLoaded(datasetToDelete))
                {
                    UnloadFromMachine(datasetToDelete);
                }

                IOwnedProxyDataset proxy = null;
                if (m_DatasetProxies.ContainsKey(datasetToDelete))
                {
                    proxy = m_DatasetProxies[datasetToDelete];
                }

                lock (m_Lock)
                {
                    m_Graph.RemoveVertex(datasetToDelete);
                }

                m_DatasetProxies.Remove(datasetToDelete);
                m_Datasets.Remove(datasetToDelete);

                if (proxy != null)
                {
                    proxy.OwnerHasDeletedDataset();
                }
            }

            RaiseOnDatasetDeleted();
        }

        /// <summary>
        /// Returns the collection of children for a given dataset.
        /// </summary>
        /// <param name="parent">The ID number of the parent dataset.</param>
        /// <returns>The collection of child datsets.</returns>
        private IEnumerable<DatasetOfflineInformation> Children(DatasetId parent)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to get the children of a dataset.");
            }

            List<Edge<DatasetId>> outEdges;
            lock (m_Lock)
            {
                outEdges = m_Graph.OutEdges(parent).ToList();
            }

            var result = from outEdge in outEdges
                         select m_Datasets[outEdge.Target];

            return result;
        }

        /// <summary>
        /// Loads a given dataset onto one or more machines.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <param name="preferredLocation">The preferred loading location.</param>
        /// <param name="machineSelector">
        ///     The function that selects the most suitable machine for the dataset to run on.
        /// </param>
        /// <param name="token">The token that is used to cancel the loading.</param>
        private void LoadOntoMachine(
            DatasetId id, 
            LoadingLocations preferredLocation,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
            CancellationToken token)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to load a dataset onto a machine.");
                Debug.Assert(machineSelector != null, "There should be a way to select the most suitable machine.");
            }

            // Indicate that the loading process has started.
            m_LoadingDatsets.Add(id, null);
            try
            {
                var offline = m_Datasets[id];
                var request = new DatasetRequest
                    {
                        DatasetToLoad = offline,
                        PreferedLocations = preferredLocation,
                        ExpectedLoadPerMachine = new ExpectedDatasetLoad
                            {
                                OnDiskSizeInBytes = offline.StoredAt.StoredSizeInBytes(),
                                InMemorySizeInBytes = offline.StoredAt.StoredSizeInBytes(),
                                RelativeMemoryExpansionWhileRunning = 2.0,
                                RelativeOnDiskExpansionAfterRunning = 2.0,
                            },
                    };
                var suggestedPlans = m_DatasetDistributor(request, token);
                var selection = from plan in suggestedPlans
                                select new DistributionSuggestion(plan);

                var selectedPlan = machineSelector(selection);
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (selectedPlan.WasSelectionCancelled)
                {
                    if (m_LoadingDatsets.ContainsKey(id))
                    {
                        m_LoadingDatsets.Remove(id);
                    }

                    return;
                }

                if (m_DatasetProxies.ContainsKey(id))
                {
                    var proxy = m_DatasetProxies[id];
                    proxy.OwnerReportsDatasetLoadingProgress(0, new DatasetLoadingProgressMark());
                }

                var task = selectedPlan.Plan.Accept(token);
                task.ContinueWith(
                    t =>
                    {
                        var dataset = t.Result;
                        m_ActiveDatasets.Add(id, dataset);

                        if (m_LoadingDatsets.ContainsKey(id))
                        {
                            m_LoadingDatsets.Remove(id);
                        }

                        if (m_DatasetProxies.ContainsKey(id))
                        {
                            var proxy = m_DatasetProxies[id];
                            proxy.OwnerReportsDatasetLoadingProgress(100, new DatasetLoadingProgressMark());
                            proxy.OwnerHasLoadedDataset();
                        }
                    },
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (OperationCanceledException)
            {
                if (m_LoadingDatsets.ContainsKey(id))
                {
                    m_LoadingDatsets.Remove(id);
                }

                throw;
            }
        }

        /// <summary>
        /// Unloads the dataset from all the machines it is loaded onto.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        private void UnloadFromMachine(DatasetId id)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to unload a dataset from a machine.");
            }

            if (!m_ActiveDatasets.ContainsKey(id))
            {
                return;
            }

            if (m_DatasetProxies.ContainsKey(id))
            {
                var proxy = m_DatasetProxies[id];
                proxy.OwnerHasUnloadedDataset();
            }

            var onlineInfo = m_ActiveDatasets[id];
            onlineInfo.Close();

            m_ActiveDatasets.Remove(id);
        }
    }
}
