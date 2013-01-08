//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using GraphSharp.Controls;
using Lokad;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// Defines the viewmodel for the graph of datasets.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DatasetGraphModel : Model
    {
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
        /// The project that holds the dataset graph.
        /// </summary>
        private readonly ProjectFacade m_Project;

        /// <summary>
        /// The function that builds <see cref="DatasetModel"/> objects.
        /// </summary>
        private readonly Func<DatasetFacade, DatasetModel> m_DatasetModelBuilder;

        /// <summary>
        /// The type of layout that should be used.
        /// </summary>
        private string m_LayoutType;

        /// <summary>
        /// The layout parameters.
        /// </summary>
        private ILayoutParameters m_LayoutParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetGraphModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="facade">The project that holds the graph of datasets.</param>
        /// <param name="datasetModelBuilder">The function that builds <see cref="DatasetModel"/> objects.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="facade"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="datasetModelBuilder"/> is <see langword="null" />.
        /// </exception>
        public DatasetGraphModel(
            IContextAware context, 
            ProjectFacade facade,
            Func<DatasetFacade, DatasetModel> datasetModelBuilder)
            : base(context)
        {
            {
                Enforce.Argument(() => facade);
            }

            m_Project = facade;
            m_Project.OnDatasetCreated += (s, e) => AddDatasetToGraph();
            m_Project.OnDatasetDeleted += (s, e) => RemoveDatasetFromGraph();

            m_DatasetModelBuilder = datasetModelBuilder;

            LayoutType = "Tree";
            LayoutParameters = new SimpleTreeLayoutParameters 
                { 
                    LayerGap = 250,
                    VertexGap = 250,
                    Direction = LayoutDirection.TopToBottom,
                };

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

                        var vertex = new DatasetViewVertex(InternalContext, m_DatasetModelBuilder(child));
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
                    if (InternalContext.IsSynchronized)
                    {
                        action(m_Graph, dataset, child);
                    }
                    else 
                    {
                        Action a = () => action(m_Graph, dataset, child);
                        InternalContext.Invoke(a);
                    }
                    
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

            Action deleteAction = () =>
                {
                    foreach (var dataset in datasets)
                    {
                        m_Graph.RemoveVertex(m_VertexMap[dataset]);
                        m_VertexMap.Remove(dataset);
                    }
                };
            if (InternalContext.IsSynchronized)
            {
                deleteAction();
            }
            else
            {
                InternalContext.Invoke(deleteAction);
            }

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

                        var vertex = new DatasetViewVertex(InternalContext, m_DatasetModelBuilder(child));
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

        /// <summary>
        /// Gets the layout type.
        /// </summary>
        public string LayoutType
        {
            get 
            {
                return m_LayoutType;
            }

            private set 
            {
                m_LayoutType = value;
            }
        }

        /// <summary>
        /// Gets or sets the layout parameters for the current layout method.
        /// </summary>
        public ILayoutParameters LayoutParameters
        {
            get 
            {
                return m_LayoutParameters;
            }

            set
            {
                m_LayoutParameters = value;
            }
        }

        /// <summary>
        /// Gets the creation transition.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "This property is used in the XAML databinding. It's easier if it is an instance property.")]
        public ITransition CreationTransition
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the destruction transition.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "This property is used in the XAML databinding. It's easier if it is an instance property.")]
        public ITransition DestructionTransition
        {
            get
            {
                return new FadeOutTransition();
            }
        }
    }
}
