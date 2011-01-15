//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Apollo.Core.UserInterfaces.Project;
using Lokad;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// Defines the viewmodel for the graph of datasets.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DatasetGraphModel : Model
    {
        /// <summary>
        /// The project that holds the dataset graph.
        /// </summary>
        private readonly ProjectFacade m_Project;

        /// <summary>
        /// The context for executing actions that have
        /// thread affinity.
        /// </summary>
        private readonly IContextAware m_Context;

        /// <summary>
        /// The graph that holds the information about the
        /// datasets for visualization purposes.
        /// </summary>
        private readonly DatasetViewGraph m_Graph = new DatasetViewGraph();

        /// <summary>
        /// The collection that maps the current set of datasets to the current set of vertices.
        /// </summary>
        private readonly Dictionary<DatasetFacade, DatasetViewVertex> m_VertexMap =
            new Dictionary<DatasetFacade, DatasetViewVertex>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetGraphModel"/> class.
        /// </summary>
        /// <param name="facade">The project that holds the graph of datasets.</param>
        /// <param name="context">The context for executing actions that have thread affinity.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="facade"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public DatasetGraphModel(ProjectFacade facade, IContextAware context)
        {
            {
                Enforce.Argument(() => facade);
                Enforce.Argument(() => context);
            }

            m_Context = context;

            m_Project = facade;
            m_Project.OnDatasetCreated += (s, e) => AddDatasetToGraph();
            m_Project.OnDatasetDeleted += (s, e) => RemoveDatasetFromGraph();
            m_Project.OnDatasetUpdated += (s, e) => UpdateGraph();

            ReloadProject();
        }

        private void AddDatasetToGraph()
        {
            Action<DatasetViewGraph, DatasetFacade, DatasetFacade> action =
                (graph, parent, child) =>
                    {
                        if (m_VertexMap.ContainsKey(child))
                        {
                            return;
                        }

                        var vertex = new DatasetViewVertex(new DatasetModel(child));
                        m_VertexMap.Add(child, vertex);

                        graph.AddVertex(vertex);
                        if (parent != null)
                        {
                            Debug.Assert(m_VertexMap.ContainsKey(parent), "Lost the parent!");
                            var parentVertex = m_VertexMap[parent];
                            var edge = new DatasetViewEdge(parentVertex, vertex);
                            graph.AddEdge(edge);
                        }
                    };

            IterateOverGraph(action);
             Notify(() => Graph);
        }

        private void IterateOverGraph(Action<DatasetViewGraph, DatasetFacade, DatasetFacade> action)
        {
            // @Todo: We should really just be able to iterate over the original graph
            // that would be much cleaner ....
            var root = m_Project.Root();
            action(m_Graph, null, root);

            var nodes = new Queue<DatasetFacade>();
            nodes.Enqueue(root);
            while (nodes.Count > 0)
            {
                var dataset = nodes.Dequeue();
                foreach (var child in dataset.Children())
                {
                    var childVertex = new DatasetViewVertex(new DatasetModel(child));
                    action(m_Graph, dataset, child);
                    nodes.Enqueue(child);
                }
            }
        }

        private void RemoveDatasetFromGraph()
        {
            var datasets = new List<DatasetFacade>();
            foreach (var pair in m_VertexMap)
            {
                datasets.Add(pair.Key);
            }

            Action<DatasetViewGraph, DatasetFacade, DatasetFacade> action =
                (graph, parent, child) =>
                {
                    if (m_VertexMap.ContainsKey(child))
                    {
                        datasets.Remove(child);
                    }
                };
            IterateOverGraph(action);

            foreach (var dataset in datasets)
            {
                m_Graph.RemoveVertex(m_VertexMap[dataset]);
                m_VertexMap.Remove(dataset);
            }

            Notify(() => Graph);
        }

        private void UpdateGraph()
        {
            Notify(() => Graph);
        }

        private void ReloadProject()
        {
            m_Graph.Clear();

            Action<DatasetViewGraph, DatasetFacade, DatasetFacade> action =
                (graph, parent, child) =>
                    {
                        if (m_VertexMap.ContainsKey(child))
                        {
                            return;
                        }

                        var vertex = new DatasetViewVertex(new DatasetModel(child));
                        m_VertexMap.Add(child, vertex);
                        graph.AddVertex(vertex);

                        if (parent != null)
                        {
                            Debug.Assert(m_VertexMap.ContainsKey(parent), "Lost the parent!");
                            var parentVertex = m_VertexMap[parent];
                            var edge = new DatasetViewEdge(parentVertex, vertex);
                            graph.AddEdge(edge);
                        }
                    };

            IterateOverGraph(action);

            Notify(() => Graph);
        }

        /// <summary>
        /// Gets the graph.
        /// </summary>
        public DatasetViewGraph Graph
        {
            get 
            { 
                return m_Graph; 
            }
        }

        // - Has events / methods to pass data onto the graph / project
        //   to make changes
        // - Has methods that get updated when the graph changes
    }
}
