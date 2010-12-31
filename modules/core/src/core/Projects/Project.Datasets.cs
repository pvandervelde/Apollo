﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using Apollo.Core.Base.Projects;
using Apollo.Core.Utils;
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
        private readonly BidirectionalGraph<DatasetId, Edge<DatasetId>> m_Graph;

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
        private readonly Dictionary<DatasetId, IProxyDatasets> m_DatasetProxies =
            new Dictionary<DatasetId, IProxyDatasets>();

        /// <summary>
        /// The collection that holds all the object that require notification when a 
        /// dataset changes.
        /// </summary>
        private readonly Dictionary<DatasetId, List<INotifyOnDatasetChange>> m_DatasetObservers =
            new Dictionary<DatasetId, List<INotifyOnDatasetChange>>();

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
        private IProxyDatasets ObtainProxyFor(DatasetId id)
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

            // First create the dataset
            var id = new DatasetId();
            var newDataset = new DatasetOfflineInformation(id, newChild);

            // Store the datset
            m_Datasets.Add(id, newDataset);

            // When adding a new dataset there is no way we can create cycles because
            // we can only add new children to parents, there is no way to link an
            // existing node to the parent.
            SecurityHelpers.Elevate(
                new PermissionSet(PermissionState.Unrestricted),
                () => m_Graph.AddVertex(id));

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
                SecurityHelpers.Elevate(
                    new PermissionSet(PermissionState.Unrestricted), 
                    () => m_Graph.AddEdge(new Edge<DatasetId>(realParent, id)));
            }

            foreach (var observer in m_ProjectObservers)
            {
                observer.DatasetCreated();
            }

            return id;
        }

        // Remove dataset?

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

            var result = SecurityHelpers.Elevate(
                new PermissionSet(PermissionState.Unrestricted),
                () => from outEdge in m_Graph.OutEdges(parent)
                      select m_Datasets[outEdge.Target]);

            return result;
        }

        /// <summary>
        /// Loads a given dataset onto one or more machines.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <param name="preferredLocation">The preferred loading location.</param>
        /// <param name="range">The minimum and maximum number of machines over which the dataset should be distributed.</param>
        private void LoadOntoMachine(DatasetId id, LoadingLocation preferredLocation, MachineDistributionRange range)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to load a dataset onto a machine.");
            }

            throw new NotImplementedException();
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

            // Should invalidate the dataset?
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers the given object for change notifications.
        /// </summary>
        /// <param name="id">The ID of the dataset for which the notifications should be registered.</param>
        /// <param name="observer">The object that wants to receive change notifications.</param>
        private void RegisterDatasetObservers(DatasetId id, INotifyOnDatasetChange observer)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to register a notification.");
                Debug.Assert(IsValid(id), "The dataset should be valid.");
                Debug.Assert(observer != null, "The notification object should not be a Null reference.");
            }

            if (!m_DatasetObservers.ContainsKey(id))
            {
                m_DatasetObservers.Add(id, new List<INotifyOnDatasetChange>());
            }

            var observers = m_DatasetObservers[id];
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        /// <summary>
        /// Unregisters the given object for change notifications.
        /// </summary>
        /// <param name="id">The ID of the dataset for which the notifications should be unregistered.</param>
        /// <param name="observer">The object that is registered for change notifications.</param>
        private void UnregisterDatasetObservers(DatasetId id, INotifyOnDatasetChange observer)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to register a notification.");
                Debug.Assert(observer != null, "The notification object should not be a Null reference.");
            }

            if (!m_DatasetObservers.ContainsKey(id))
            {
                return;
            }

            var observers = m_DatasetObservers[id];
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }

        /// <summary>
        /// Unregisters the given object for change notifications.
        /// </summary>
        /// <param name="id">The ID of the dataset for which the notifications should be unregistered.</param>
        private void UnregisterAllDatasetObservers(DatasetId id)
        {
            {
                Debug.Assert(!IsClosed, "The project should not be closed if we want to register a notification.");
            }

            if (m_DatasetObservers.ContainsKey(id))
            {
                m_DatasetObservers.Remove(id);
            }
        }

        /// <summary>
        /// Returns the collection of objects that have registered for change notifications
        /// for the dataset with the given ID.
        /// </summary>
        /// <param name="id">The ID of the dataset.</param>
        /// <returns>
        /// The collection of objects that have registered for change notifications.
        /// </returns>
        private IEnumerable<INotifyOnDatasetChange> DatasetObservers(DatasetId id)
        {
            {
                Debug.Assert(id != null, "The ID should not be a null reference.");
            }

            if (!m_DatasetObservers.ContainsKey(id))
            {
                return new INotifyOnDatasetChange[0];
            }

            return m_DatasetObservers[id].AsReadOnly();
        }
    }
}
