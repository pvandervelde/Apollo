//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Projects;
using QuickGraph;

namespace Apollo.Core.Projects
{
    /// <content>
    /// Stores all information about the datasets.
    /// </content>
    internal sealed partial class Project
    {
        /// <summary>
        /// The graph that describes the relations between the different datasets.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This is the way the QuickGraph API works.")]
        private readonly BidirectionalGraph<DatasetId, Edge<DatasetId>> m_Graph =
            new BidirectionalGraph<DatasetId, Edge<DatasetId>>(false);

        /// <summary>
        /// The collection of all datasets that belong to the current project.
        /// </summary>
        private readonly Dictionary<DatasetId, DatasetOfflineInformation> m_Datasets =
            new Dictionary<DatasetId, DatasetOfflineInformation>();

        /// <summary>
        /// The collection of all datasets which are currently loaded onto a machine.
        /// </summary>
        private readonly Dictionary<DatasetId, DatasetOnlineInformation> m_ActiveDatasets =
            new Dictionary<DatasetId, DatasetOnlineInformation>();

        /// <summary>
        /// The collection that holds the proxies for the datasets that we are mirroring 
        /// for the UI.
        /// </summary>
        private readonly Dictionary<DatasetId, IReadOnlyDataset> m_DatasetProxies =
            new Dictionary<DatasetId, IReadOnlyDataset>();

        /// <summary>
        /// The ID number of the root dataset.
        /// </summary>
        private DatasetId m_RootDataset;

        private bool IsValid(DatasetId id)
        {
            Debug.Assert(id != null, "The ID should not be a null reference.");
            return !IsClosed && m_Datasets.ContainsKey(id);
        }

        private IReadOnlyDataset ObtainProxyFor(DatasetId id)
        {
            {
                Debug.Assert(IsValid(id), "Cannot create a proxy for an invalid ID.");
            }

            if (!m_DatasetProxies.ContainsKey(id))
            {
                m_DatasetProxies.Add(id, new ReadOnlyDataset(this, id));
            }

            return m_DatasetProxies[id];
        }

        private bool IsLoaded(DatasetId id)
        {
            Debug.Assert(id != null, "The ID should not be a null reference.");
            return m_ActiveDatasets.ContainsKey(id);
        }

        private DatasetOfflineInformation OfflineInformation(DatasetId id)
        {
            {
                Debug.Assert(m_Datasets.ContainsKey(id), "Unknown dataset ID found.");
            }

            return m_Datasets[id];
        }

        private DatasetOnlineInformation OnlineInformation(DatasetId id)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to get the online information.");
                Debug.Assert(m_ActiveDatasets.ContainsKey(id), "Unknown dataset ID found.");
            }

            return m_ActiveDatasets[id];
        }

        private DatasetId CreateDataset(DatasetId parent, DatasetCreationInformation newChild)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to create a new dataset.");
            }

            // First create the dataset
            var id = new DatasetId();
            var newDataset = new DatasetOfflineInformation(id, newChild);

            // Store the datset
            m_Datasets.Add(id, newDataset);

            // When adding a new dataset there is no way we can create cycles because
            // we can only add new children to parents, there is no way to link an
            // existing node to the parent.
            m_Graph.AddVertex(id);

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
                m_Graph.AddEdge(new Edge<DatasetId>(realParent, id));
            }

            return id;
        }

        private IEnumerable<DatasetOfflineInformation> Children(DatasetId parent)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to get the children of a dataset.");
            }

            var result = from outEdge in m_Graph.OutEdges(parent)
                         select m_Datasets[outEdge.Target];

            return result;
        }

        private void LoadOntoMachine(DatasetId id, LoadingLocation preferredLocation, MachineDistributionRange range)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to load a dataset onto a machine.");
            }

            throw new NotImplementedException();
        }

        private void UnloadFromMachine(DatasetId id)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to unload a dataset from a machine.");
            }

            // Should invalidate the dataset?
            throw new NotImplementedException();
        }
    }
}
