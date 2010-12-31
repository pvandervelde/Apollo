//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        /// The graph that holds the information about the
        /// datasets for visualization purposes.
        /// </summary>
        private readonly DatasetViewGraph m_Graph = new DatasetViewGraph();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetGraphModel"/> class.
        /// </summary>
        /// <param name="facade">The project that holds the graph of datasets.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="facade"/> is <see langword="null" />.
        /// </exception>
        public DatasetGraphModel(ProjectFacade facade)
        {
            {
                Enforce.Argument(() => facade);
            }

            m_Project = facade;
            m_Project.OnDatasetCreated += (s, e) => ReloadProject();
            m_Project.OnDatasetDeleted += (s, e) => ReloadProject();
            m_Project.OnDatasetUpdated += (s, e) => ReloadProject();

            ReloadProject();
        }

        private void ReloadProject()
        {
            m_Graph.Clear();

            // There must be at least one dataset (the root dataset is created by default).
            // Thus we can build a graph.
            //
            // First the root dataset.
            var root = m_Project.Root();
            var rootVertex = new DatasetViewVertex(new DatasetModel(root));
            m_Graph.AddVertex(rootVertex);

            var nodes = new Queue<Tuple<DatasetFacade, DatasetViewVertex>>();
            nodes.Enqueue(new Tuple<DatasetFacade, DatasetViewVertex>(root, rootVertex));

            // And then the children of the root dataset (and their children, etc. etc.)
            while (nodes.Count > 0)
            {
                var pair = nodes.Dequeue();
                var dataset = pair.Item1;
                var vertex = pair.Item2;

                foreach (var child in dataset.Children())
                {
                    var childVertex = new DatasetViewVertex(new DatasetModel(child));
                    nodes.Enqueue(new Tuple<DatasetFacade, DatasetViewVertex>(child, childVertex));

                    m_Graph.AddVertex(childVertex);
                    m_Graph.AddEdge(new DatasetViewEdge(vertex, childVertex));
                }
            }

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
