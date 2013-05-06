//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.History;
using QuickGraph;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an <see cref="BidirectionalGraph{TVertex, TEdge}"/> collection.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex that is stored in the graph.</typeparam>
    /// <typeparam name="TEdge">The type of edge that is stored in the graph.</typeparam>
    [CLSCompliant(false)]
    public sealed partial class BidirectionalGraphHistory<TVertex, TEdge> :
        HistorySnapshotStorage<BidirectionalGraph<TVertex, TEdge>>,
        IBidirectionalGraphHistory<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Gets the type of edge in the graph.
        /// </summary>
        public static Type EdgeType
        {
            get
            {
                return typeof(TEdge);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BidirectionalGraphHistory{TVertex, TEdge}"/> class.
        /// </summary>
        public BidirectionalGraphHistory()
            : base(old => (old != null) ? old.Clone() : new BidirectionalGraph<TVertex, TEdge>())
        {
        }

        /// <summary>
        /// Gets a value indicating whether the graph is directional or not.
        /// </summary>
        public bool IsDirected
        {
            get
            {
                return Current.IsDirected;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the graph allows more than one edge between two
        /// vertices.
        /// </summary>
        public bool AllowParallelEdges
        {
            get
            {
                return Current.AllowParallelEdges;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are any edges in the graph.
        /// </summary>
        public bool IsEdgesEmpty
        {
            get
            {
                return Current.IsEdgesEmpty;
            }
        }

        /// <summary>
        /// Gets the number of edges currently in the graph.
        /// </summary>
        public int EdgeCount
        {
            get
            {
                return Current.EdgeCount;
            }
        }

        /// <summary>
        /// Gets or sets the number of edges that can be stored without having to resize the internal storage structures.
        /// </summary>
        public int EdgeCapacity
        {
            get
            {
                return Current.EdgeCapacity;
            }

            set
            {
                if (value != Current.EdgeCapacity)
                {
                    Current.EdgeCapacity = value;
                    Changes.Add(new EdgeCapacityChange(value));
                }
            }
        }

        /// <summary>
        /// Gets the collection of edges for the current graph.
        /// </summary>
        public IEnumerable<TEdge> Edges
        {
            get
            {
                return Current.Edges;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the given edge is part of the current graph.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <returns>
        /// <see langword="true"/> if the edge is part of the current graph; otherwise, <see langword="false" />.
        /// </returns>
        public bool ContainsEdge(TEdge edge)
        {
            return Current.ContainsEdge(edge);
        }

        /// <summary>
        /// Returns a value indicating whether an edge exists between the two vertices in the
        /// current graph.
        /// </summary>
        /// <param name="source">The starting vertex for the edge.</param>
        /// <param name="target">The ending vertex for the edge.</param>
        /// <returns>
        /// <see langword="true"/> if an edge exist between the two vertices in the current graph; 
        /// otherwise, <see langword="false" />.
        /// </returns>
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return Current.ContainsEdge(source, target);
        }

        /// <summary>
        /// Gets a value indicating whether the current graph has any vertices.
        /// </summary>
        public bool IsVerticesEmpty
        {
            get
            {
                return Current.IsVerticesEmpty;
            }
        }

        /// <summary>
        /// Gets the number of vertices currently in the graph.
        /// </summary>
        public int VertexCount
        {
            get
            {
                return Current.VertexCount;
            }
        }

        /// <summary>
        /// Gets the collection of vertices for the current graph.
        /// </summary>
        public IEnumerable<TVertex> Vertices
        {
            get
            {
                return Current.Vertices;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the given vertex is part of the current graph.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>
        /// <see langword="true"/> if the vertex is part of the current graph; otherwise, <see langword="false" />.
        /// </returns>
        public bool ContainsVertex(TVertex vertex)
        {
            return Current.ContainsVertex(vertex);
        }

        /// <summary>
        /// Adds a new edge to the graph.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        /// <returns>
        /// <see langword="true" /> if the edge was added successfully; otherwise, <see langword="false" />.
        /// </returns>
        public bool AddEdge(TEdge edge)
        {
            var result = Current.AddEdge(edge);
            if (result)
            {
                Changes.Add(new AddEdgeChange(edge));
                RaiseEdgeAdded(edge);
            }

            return result;
        }

        /// <summary>
        /// Adds the collection of edges to the graph.
        /// </summary>
        /// <param name="edges">The collection of edges.</param>
        /// <returns>The number of edges that were added to the graph.</returns>
        public int AddEdgeRange(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
            {
                if (AddEdge(edge))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// An event that is raised when a new edge is added to the graph.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
            Justification = "Event is inherited from the IMutableEdgeListGraph<TVertex, TEdge> interface.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
            Justification = "Cannot rename EdgeAction because it is defined in QuickGraph.")]
        public event EdgeAction<TVertex, TEdge> EdgeAdded;

        private void RaiseEdgeAdded(TEdge args)
        {
            var local = EdgeAdded;
            if (local != null)
            {
                local(args);
            }
        }

        /// <summary>
        /// Removes the given edge from the graph.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <returns>
        /// <see langword="true" /> if the edge was removed; otherwise, <see langword="false" />.
        /// </returns>
        public bool RemoveEdge(TEdge edge)
        {
            var result = Current.RemoveEdge(edge);
            if (result)
            {
                Changes.Add(new RemoveEdgeChange(edge));
                RaiseEdgeRemoved(edge);
            }

            return result;
        }

        /// <summary>
        /// Removes all edges that match the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of edges that were removed.</returns>
        public int RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
        {
            return RemoveEdgesByCollectionIf(Current.Edges, predicate);
        }

        private int RemoveEdgesByCollectionIf(IEnumerable<TEdge> edges, EdgePredicate<TVertex, TEdge> predicate)
        {
            var edgesToRemove = new List<TEdge>();
            foreach (var edge in edges)
            {
                if (predicate(edge))
                {
                    edgesToRemove.Add(edge);
                }
            }

            foreach (var edge in edgesToRemove)
            {
                RemoveEdge(edge);
            }

            return edgesToRemove.Count;
        }

        /// <summary>
        /// Removes all the inbound edges that match the given predicate.
        /// </summary>
        /// <param name="v">The vertex for which the inbound edges should be checked.</param>
        /// <param name="edgePredicate">The predicate.</param>
        /// <returns>The number of inbound edges that were removed.</returns>
        public int RemoveInEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> edgePredicate)
        {
            return RemoveEdgesByCollectionIf(Current.InEdges(v), edgePredicate);
        }

        /// <summary>
        /// Removes all the outbound edges that match the given predicate.
        /// </summary>
        /// <param name="v">The vertex for which the outbound edges should be checked.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of outbound edges that were removed.</returns>
        public int RemoveOutEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
        {
            return RemoveEdgesByCollectionIf(Current.OutEdges(v), predicate);
        }

        /// <summary>
        /// An event raised when an edge is removed from the current graph.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
            Justification = "Event is inherited from the IMutableEdgeListGraph<TVertex, TEdge> interface.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
            Justification = "Cannot rename EdgeAction because it is defined in QuickGraph.")]
        public event EdgeAction<TVertex, TEdge> EdgeRemoved;

        private void RaiseEdgeRemoved(TEdge args)
        {
            var local = EdgeRemoved;
            if (local != null)
            {
                local(args);
            }
        }

        /// <summary>
        /// Adds the given vertex to the graph.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>
        /// <see langword="true" /> if the vertex was added successfully; otherwise, <see langword="false" />.
        /// </returns>
        public bool AddVertex(TVertex v)
        {
            var result = Current.AddVertex(v);
            if (result)
            {
                Changes.Add(new AddVertexChange(v));
                RaiseVertexAdded(v);
            }

            return result;
        }

        /// <summary>
        /// Adds the collection of vertices to the graph.
        /// </summary>
        /// <param name="vertices">The collection of vertices.</param>
        /// <returns>The number of vertices that were added to the graph.</returns>
        public int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            int count = 0;
            foreach (var vertex in vertices)
            {
                if (AddVertex(vertex))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Adds the edge and the vertices that belong to the edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <returns>
        /// <see langword="true" /> if the edge was added to the graph; otherwise, <see langword="false" />.
        /// </returns>
        public bool AddVerticesAndEdge(TEdge edge)
        {
            AddVertex(edge.Source);
            AddVertex(edge.Target);
            return AddEdge(edge);
        }

        /// <summary>
        /// Adds the collection of edges and their associated vertices to the graph.
        /// </summary>
        /// <param name="edges">The collection of edges.</param>
        /// <returns>The number of edges that were added to the graph.</returns>
        public int AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
            {
                if (AddVerticesAndEdge(edge))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// An event that is raised when a new vertex is added to the graph.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
            Justification = "Event is inherited from the IMutableVertexSet<TVertex> interface.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
            Justification = "Cannot rename EdgeAction because it is defined in QuickGraph.")]
        public event VertexAction<TVertex> VertexAdded;

        private void RaiseVertexAdded(TVertex args)
        {
            var local = VertexAdded;
            if (local != null)
            {
                local(args);
            }
        }

        /// <summary>
        /// Removes the vertex from the graph.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>
        /// <see langword="true" /> if the vertex was removed; otherwise, <see langword="false" />.
        /// </returns>
        public bool RemoveVertex(TVertex v)
        {
            var result = Current.RemoveVertex(v);
            if (result)
            {
                Changes.Add(new RemoveVertexChange(v));
                RaiseVertexRemoved(v);
            }

            return result;
        }

        /// <summary>
        /// Removes all vertices that match the given predicate.
        /// </summary>
        /// <param name="pred">The predicate.</param>
        /// <returns>The number of vertices that were removed.</returns>
        public int RemoveVertexIf(VertexPredicate<TVertex> pred)
        {
            var verticesToRemove = new List<TVertex>();
            foreach (var vertex in Current.Vertices)
            {
                if (pred(vertex))
                {
                    verticesToRemove.Add(vertex);
                }
            }

            foreach (var vertex in verticesToRemove)
            {
                RemoveVertex(vertex);
            }

            return verticesToRemove.Count;
        }

        /// <summary>
        /// An event raised when a vertex is removed.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
            Justification = "Event is inherited from the IMutableVertexSet<TVertex> interface.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
            Justification = "Cannot rename EdgeAction because it is defined in QuickGraph.")]
        public event VertexAction<TVertex> VertexRemoved;

        private void RaiseVertexRemoved(TVertex args)
        {
            var local = VertexRemoved;
            if (local != null)
            {
                local(args);
            }
        }

        /// <summary>
        /// Clears all the edges and vertices from the graph.
        /// </summary>
        public void Clear()
        {
            // Can't just remove all the changes. Only remove
            // the adds, deletes and merges
            for (int i = Changes.Count - 1; i >= 0; i--)
            {
                if ((Changes[i] as IGraphHistoryChange<BidirectionalGraph<TVertex, TEdge>>).IsAffectedByGraphClear)
                {
                    Changes.RemoveAt(i);
                }
            }

            Changes.Add(new GraphClearChange());
            Current.Clear();
            RaiseCleared();
        }

        /// <summary>
        /// Called when the graph vertices and edges have been cleared.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
            Justification = "Event is inherited from the IMutableGraph<TVertex, TEdge> interface.")]
        public event EventHandler Cleared;

        private void RaiseCleared()
        {
            var local = Cleared;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Removes all the edges that are connected to the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        public void ClearEdges(TVertex v)
        {
            RemoveEdgesByCollectionIf(Current.InEdges(v), e => { return true; });
            RemoveEdgesByCollectionIf(Current.OutEdges(v), e => { return true; });
        }

        /// <summary>
        /// Removes all the inbound edges that are connected to the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        public void ClearInEdges(TVertex v)
        {
            RemoveEdgesByCollectionIf(Current.InEdges(v), e => { return true; });
        }

        /// <summary>
        /// Removes all the outbound edges that are connected to the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        public void ClearOutEdges(TVertex v)
        {
            RemoveEdgesByCollectionIf(Current.OutEdges(v), e => { return true; });
        }

        /// <summary>
        /// Returns the degree of the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>The number of edges that connects to the given vertex.</returns>
        public int Degree(TVertex v)
        {
            return Current.Degree(v);
        }

        /// <summary>
        /// Returns the number of inbound edges that connects to the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>The number of inbound edges that connects to the given vertex.</returns>
        public int InDegree(TVertex v)
        {
            return Current.InDegree(v);
        }

        /// <summary>
        /// Returns the number of outbound edges that connects to the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>The number of outbound edges that connects to the given vertex.</returns>
        public int OutDegree(TVertex v)
        {
            return Current.OutDegree(v);
        }

        /// <summary>
        /// Returns the inbound edge with the given index.
        /// </summary>
        /// <param name="v">The vertex for which the inbound edge should be returned.</param>
        /// <param name="index">The index of the inbound edge.</param>
        /// <returns>The inbound edge at the given index.</returns>
        public TEdge InEdge(TVertex v, int index)
        {
            return Current.InEdge(v, index);
        }

        /// <summary>
        /// Returns the collection of inbound edges for the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>The collection of inbound edges.</returns>
        public IEnumerable<TEdge> InEdges(TVertex v)
        {
            return Current.InEdges(v);
        }

        /// <summary>
        /// Returns a value indicating if there are any inbound edges for the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>
        /// <see langword="true" /> if there are any inbound edges for the given vertex; otherwise, <see langword="false" />.
        /// </returns>
        public bool IsInEdgesEmpty(TVertex v)
        {
            return Current.IsInEdgesEmpty(v);
        }

        /// <summary>
        /// Returns the outbound edge with the given index.
        /// </summary>
        /// <param name="v">The vertex for which the outbound edge should be returned.</param>
        /// <param name="index">The index of the outbound edge.</param>
        /// <returns>The outbound edge at the given index.</returns>
        public TEdge OutEdge(TVertex v, int index)
        {
            return Current.OutEdge(v, index);
        }

        /// <summary>
        /// Returns the collection of outbound edges for the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>The collection of outbound edges.</returns>
        public IEnumerable<TEdge> OutEdges(TVertex v)
        {
            return Current.OutEdges(v);
        }

        /// <summary>
        /// Returns a value indicating if there are any outbound edges for the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>
        /// <see langword="true" /> if there are any outbound edges for the given vertex; otherwise, <see langword="false" />.
        /// </returns>
        public bool IsOutEdgesEmpty(TVertex v)
        {
            return Current.IsOutEdgesEmpty(v);
        }

        /// <summary>
        /// Removes the given vertex and creates new edges between all the vertices
        /// that were connected to the deleted vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="edgeFactory">The delegate that is used to create new edges.</param>
        public void MergeVertex(TVertex vertex, EdgeFactory<TVertex, TEdge> edgeFactory)
        {
            var inEdges = Current.InEdges(vertex);
            var outEdges = Current.OutEdges(vertex);

            RemoveVertex(vertex);

            foreach (var inEdge in inEdges)
            {
                if (inEdge.Source.Equals(vertex))
                {
                    continue;
                }

                foreach (var outEdge in outEdges)
                {
                    if (vertex.Equals(outEdge.Target))
                    {
                        continue;
                    }

                    AddEdge(edgeFactory(inEdge.Source, outEdge.Target));
                }
            }
        }

        /// <summary>
        /// Removes each vertex that matches the predicate vertex and creates new edges between all the vertices
        /// that were connected to the deleted vertex.
        /// </summary>
        /// <param name="vertexPredicate">The predicate that indicates which vertices need to be removed.</param>
        /// <param name="edgeFactory">The delegate that is used to create new edges.</param>
        public void MergeVertexIf(VertexPredicate<TVertex> vertexPredicate, EdgeFactory<TVertex, TEdge> edgeFactory)
        {
            var list = new List<TVertex>(VertexCount / 4);
            foreach (var vertex in this.Vertices)
            {
                if (vertexPredicate(vertex))
                {
                    list.Add(vertex);
                }
            }

            foreach (var vertex in list)
            {
                this.MergeVertex(vertex, edgeFactory);
            }
        }

        /// <summary>
        /// Shrinks the internal data structures to match their current count.
        /// </summary>
        public void TrimEdgeExcess()
        {
            // @todo: Do we need to store the fact that we did a TrimEdgeExcess?
            Current.TrimEdgeExcess();
        }

        /// <summary>
        /// Tries to get the edge that points from the source vertex to the target vertex.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <param name="edge">The desired edge or <see langword="null" />.</param>
        /// <returns>
        /// <see langword="true" /> if the method returns successfully; otherwise, <see langword="false" />.
        /// </returns>
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            return Current.TryGetEdge(source, target, out edge);
        }

        /// <summary>
        /// Tries to get the edges that point from the source vertex to the target vertex.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <param name="edges">The collection of edges if they exist, otherwise <see langword="null" />.</param>
        /// <returns>
        /// <see langword="true" /> if the method returns successfully; otherwise, <see langword="false" />.
        /// </returns>
        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            return Current.TryGetEdges(source, target, out edges);
        }

        /// <summary>
        /// Tries to get the collection of inbound edges for the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <param name="edges">The collection of inbound edges if they exist, otherwise <see langword="null" />.</param>
        /// <returns>
        /// <see langword="true" /> if the method returns successfully; otherwise, <see langword="false" />.
        /// </returns>
        public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            return Current.TryGetInEdges(v, out edges);
        }

        /// <summary>
        /// Tries to get the collection of outbound edges for the given vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <param name="edges">The collection of outbound edges if they exist, otherwise <see langword="null" />.</param>
        /// <returns>
        /// <see langword="true" /> if the method returns successfully; otherwise, <see langword="false" />.
        /// </returns>
        public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            return Current.TryGetOutEdges(v, out edges);
        }

        /// <summary>
        /// Clones the internal graph.
        /// </summary>
        /// <returns>A new copy of the graph.</returns>
        public BidirectionalGraph<TVertex, TEdge> Clone()
        {
            return Current.Clone();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
