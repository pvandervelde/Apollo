//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apollo.Core.Base;
using Apollo.Utilities;
using Apollo.Utilities.History;
using QuickGraph;

namespace Apollo.Core.Host.Projects
{
    /// <content>
    /// Stores all information about the history of the project.
    /// </content>
    internal sealed partial class Project
    {
        /// <summary>
        /// Stores the collections of datasets, both active and total, in a timeline specific way.
        /// </summary>
        private sealed class DatasetHistoryStorage : IAmHistoryEnabled
        {
            /// <summary>
            /// Returns the name of the <see cref="KnownDatasets"/> field.
            /// </summary>
            /// <remarks>FOR INTERNAL USE ONLY!</remarks>
            /// <returns>The name of the field.</returns>
            internal static string NameOfGraphField()
            {
                return ReflectionExtensions.MemberName<DatasetHistoryStorage, IMutableBidirectionalGraph<DatasetId, Edge<DatasetId>>>(
                    p => p.m_Graph);
            }

            /// <summary>
            /// Returns the name of the <see cref="KnownDatasets"/> field.
            /// </summary>
            /// <remarks>FOR INTERNAL USE ONLY!</remarks>
            /// <returns>The name of the field.</returns>
            internal static string NameOfKnownDatasetsField()
            {
                return ReflectionExtensions.MemberName<DatasetHistoryStorage, IDictionaryTimelineStorage<DatasetId, DatasetOfflineInformation>>(
                    p => p.m_KnownDatasets);
            }

            /// <summary>
            /// Returns the name of the <see cref="ActiveDatasets"/> field.
            /// </summary>
            /// <remarks>FOR INTERNAL USE ONLY!</remarks>
            /// <returns>The name of the field.</returns>
            internal static string NameOfActiveDatasetsField()
            {
                return ReflectionExtensions.MemberName<DatasetHistoryStorage, IDictionaryTimelineStorage<DatasetId, DatasetOnlineInformation>>(
                    p => p.m_ActiveDatasets);
            }

            /// <summary>
            /// The graph which describes the connections between the different datasets.
            /// </summary>
            private readonly IBidirectionalGraphHistory<DatasetId, Edge<DatasetId>> m_Graph;

            /// <summary>
            /// The collection of all datasets that belong to the current project.
            /// </summary>
            private readonly IDictionaryTimelineStorage<DatasetId, DatasetOfflineInformation> m_KnownDatasets;

            /// <summary>
            /// The collection of all datasets which are currently loaded onto a machine.
            /// </summary>
            private readonly IDictionaryTimelineStorage<DatasetId, DatasetOnlineInformation> m_ActiveDatasets;

            /// <summary>
            /// The ID used by the timeline to uniquely identify the current object.
            /// </summary>
            private readonly HistoryId m_HistoryId;

            /// <summary>
            /// Initializes a new instance of the <see cref="DatasetHistoryStorage"/> class.
            /// </summary>
            /// <param name="id">The ID used by the timeline to uniquely identify the current object.</param>
            /// <param name="graph">The graph that describes the connections between the datasets.</param>
            /// <param name="knownDatasets">The collection that contains the known but not necessary active datasets.</param>
            /// <param name="activeDatasets">The collection that contains the active datasets.</param>
            public DatasetHistoryStorage(
                HistoryId id,
                IBidirectionalGraphHistory<DatasetId, Edge<DatasetId>> graph,
                IDictionaryTimelineStorage<DatasetId, DatasetOfflineInformation> knownDatasets,
                IDictionaryTimelineStorage<DatasetId, DatasetOnlineInformation> activeDatasets)
            {
                {
                    Lokad.Enforce.Argument(() => id);
                    Lokad.Enforce.Argument(() => graph);
                    Lokad.Enforce.Argument(() => knownDatasets);
                    Lokad.Enforce.Argument(() => activeDatasets);
                }

                m_HistoryId = id;
                m_Graph = graph;
                m_KnownDatasets = knownDatasets;
                m_ActiveDatasets = activeDatasets;
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
            /// Gets the graph that describes the relation between the different datasets in the project.
            /// </summary>
            public IMutableBidirectionalGraph<DatasetId, Edge<DatasetId>> Graph
            {
                get
                {
                    return m_Graph;
                }
            }

            /// <summary>
            /// Gets the collection that holds the known, but not necessary active, datasets.
            /// </summary>
            public IDictionary<DatasetId, DatasetOfflineInformation> KnownDatasets
            {
                get
                {
                    return m_KnownDatasets;
                }
            }

            /// <summary>
            /// Gets the collection that holds the active datasets.
            /// </summary>
            public IDictionary<DatasetId, DatasetOnlineInformation> ActiveDatasets
            {
                get
                {
                    return m_ActiveDatasets;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or
            /// resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // Don't do anything at the moment.
            }
        }

        private static DatasetHistoryStorage BuildDatasetHistoryStorage(HistoryId id, IEnumerable<Tuple<string, IStoreTimelineValues>> members)
        {
            {
                Debug.Assert(members.Count() == 3, "There should only be 3 members.");
            }

            BidirectionalGraphHistory<DatasetId, Edge<DatasetId>> graph = null;
            IDictionaryTimelineStorage<DatasetId, DatasetOfflineInformation> knownDatasets = null;
            IDictionaryTimelineStorage<DatasetId, DatasetOnlineInformation> activeDatasets = null;
            foreach (var member in members)
            {
                if (string.Equals(DatasetHistoryStorage.NameOfGraphField(), member.Item1, StringComparison.Ordinal))
                {
                    graph = member.Item2 as BidirectionalGraphHistory<DatasetId, Edge<DatasetId>>;
                    continue;
                }

                if (string.Equals(DatasetHistoryStorage.NameOfKnownDatasetsField(), member.Item1, StringComparison.Ordinal))
                {
                    knownDatasets = member.Item2 as IDictionaryTimelineStorage<DatasetId, DatasetOfflineInformation>;
                    continue;
                }

                if (string.Equals(DatasetHistoryStorage.NameOfActiveDatasetsField(), member.Item1, StringComparison.Ordinal))
                {
                    activeDatasets = member.Item2 as IDictionaryTimelineStorage<DatasetId, DatasetOnlineInformation>;
                    continue;
                }

                throw new UnknownMemberNameException();
            }

            return new DatasetHistoryStorage(id, graph, knownDatasets, activeDatasets);
        }

        /// <summary>
        /// The timeline for the current project.
        /// </summary>
        private readonly ITimeline m_Timeline;

        private void OnTimelineRolledBack(object sender, EventArgs args)
        {
            ReloadProxiesDueToHistoryChange();
        }

        private void ReloadProxiesDueToHistoryChange()
        {
            var proxiesToRemove = new List<DatasetId>();
            foreach (var pair in m_DatasetProxies)
            {
                if (!m_Datasets.KnownDatasets.ContainsKey(pair.Key))
                {
                    proxiesToRemove.Add(pair.Key);
                }
            }

            foreach (var proxy in proxiesToRemove)
            {
                m_DatasetProxies[proxy].OwnerHasDeletedDataset();
            }

            // We should really remove all the proxies and reload them
            // but for now we'll just notify of deletions only
            RaiseOnDatasetDeleted();
        }

        private void OnTimelineRolledForward(object sender, EventArgs args)
        {
            ReloadProxiesDueToHistoryChange();
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
    }
}
