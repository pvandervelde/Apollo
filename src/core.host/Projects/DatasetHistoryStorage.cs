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
using Apollo.Utilities.History;
using QuickGraph;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Stores the collections of datasets, both active and total, in a timeline specific way.
    /// </summary>
    internal sealed class DatasetHistoryStorage : IAmHistoryEnabled
    {
        /// <summary>
        /// The history index of the name field.
        /// </summary>
        private const byte GraphIndex = 0;

        /// <summary>
        /// The history index of the summary field.
        /// </summary>
        private const byte DatasetsIndex = 1;

        /// <summary>
        /// Creates a new instance of the <see cref="DatasetHistoryStorage"/> class with the given 
        /// history information.
        /// </summary>
        /// <param name="id">The history ID for the dataset storage.</param>
        /// <param name="members">The collection that holds all the members for the current object.</param>
        /// <param name="constructorArguments">The optional constructor arguments.</param>
        /// <returns>A new instance of the <see cref="DatasetHistoryStorage"/> class.</returns>
        internal static DatasetHistoryStorage CreateInstance(
            HistoryId id,
            IEnumerable<Tuple<byte, IStoreTimelineValues>> members,
            params object[] constructorArguments)
        {
            {
                Debug.Assert(members.Count() == 2, "There should only be 2 members.");
            }

            BidirectionalGraphHistory<DatasetId, Edge<DatasetId>> graph = null;
            IDictionaryTimelineStorage<DatasetId, DatasetProxy> knownDatasets = null;
            foreach (var member in members)
            {
                if (member.Item1 == GraphIndex)
                {
                    graph = member.Item2 as BidirectionalGraphHistory<DatasetId, Edge<DatasetId>>;
                    continue;
                }

                if (member.Item1 == DatasetsIndex)
                {
                    knownDatasets = member.Item2 as IDictionaryTimelineStorage<DatasetId, DatasetProxy>;
                    continue;
                }

                throw new UnknownMemberException();
            }

            return new DatasetHistoryStorage(id, graph, knownDatasets);
        }

        /// <summary>
        /// The graph which describes the connections between the different datasets.
        /// </summary>
        [FieldIndexForHistoryTracking(GraphIndex)]
        private readonly IBidirectionalGraphHistory<DatasetId, Edge<DatasetId>> m_Graph;

        /// <summary>
        /// The collection of all datasets that belong to the current project.
        /// </summary>
        [FieldIndexForHistoryTracking(DatasetsIndex)]
        private readonly IDictionaryTimelineStorage<DatasetId, DatasetProxy> m_Datasets;

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
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="graph"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="knownDatasets"/> is <see langword="null" />.
        /// </exception>
        private DatasetHistoryStorage(
            HistoryId id,
            IBidirectionalGraphHistory<DatasetId, Edge<DatasetId>> graph,
            IDictionaryTimelineStorage<DatasetId, DatasetProxy> knownDatasets)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => graph);
                Lokad.Enforce.Argument(() => knownDatasets);
            }

            m_HistoryId = id;
            m_Graph = graph;
            m_Datasets = knownDatasets;
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
        /// Gets the collection that holds the datasets.
        /// </summary>
        public IDictionary<DatasetId, DatasetProxy> Datasets
        {
            get
            {
                return m_Datasets;
            }
        }
    }
}
