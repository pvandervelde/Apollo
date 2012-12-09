//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Utilities.History;
using QuickGraph;

namespace Apollo.Utilities.History
{
    /// <content>
    /// Defines the internal classes that are used to track history changes in the graph.
    /// </content>
    public sealed partial class BidirectionalGraphHistory<TVertex, TEdge>
    {
        /// <summary>
        /// An <see cref="IHistoryChange{T}"/> which indicates the edge capacity was changed on a
        /// <see cref="BidirectionalGraph{TVertex, TEdge}"/>.
        /// </summary>
        private sealed class EdgeCapacityChange : IGraphHistoryChange<BidirectionalGraph<TVertex, TEdge>>
        {
            /// <summary>
            /// The new value for the capacity.
            /// </summary>
            private readonly int m_Capacity;

            /// <summary>
            /// Initializes a new instance of the <see cref="EdgeCapacityChange"/> class.
            /// </summary>
            /// <param name="capacity">The new value for the capacity.</param>
            public EdgeCapacityChange(int capacity)
            {
                m_Capacity = capacity;
            }

            /// <summary>
            /// Gets a value indicating whether the change is affected by invoking the
            /// clear method on the graph.
            /// </summary>
            public bool IsAffectedByGraphClear 
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            public void ApplyTo(BidirectionalGraph<TVertex, TEdge> historyObject)
            {
                Debug.Assert(historyObject != null, "This change can only be applied to a BidirectionalGraph.");
                historyObject.EdgeCapacity = m_Capacity;
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{T}"/> which indicates that an edge was added to a
        /// <see cref="BidirectionalGraph{TVertex, TEdge}"/>.
        /// </summary>
        private sealed class AddEdgeChange : IGraphHistoryChange<BidirectionalGraph<TVertex, TEdge>>
        { 
            /// <summary>
            /// The new edge.
            /// </summary>
            private readonly TEdge m_Edge;

            /// <summary>
            /// Initializes a new instance of the <see cref="AddEdgeChange"/> class.
            /// </summary>
            /// <param name="edge">The new edge.</param>
            public AddEdgeChange(TEdge edge)
            {
                m_Edge = edge;
            }

            /// <summary>
            /// Gets a value indicating whether the change is affected by invoking the
            /// clear method on the graph.
            /// </summary>
            public bool IsAffectedByGraphClear
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            public void ApplyTo(BidirectionalGraph<TVertex, TEdge> historyObject)
            {
                Debug.Assert(historyObject != null, "This change can only be applied to a BidirectionalGraph.");
                historyObject.AddEdge(m_Edge);
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{T}"/> which indicates that an edge was removed from a
        /// <see cref="BidirectionalGraph{TVertex, TEdge}"/>.
        /// </summary>
        private sealed class RemoveEdgeChange : IGraphHistoryChange<BidirectionalGraph<TVertex, TEdge>>
        {
            /// <summary>
            /// The new edge.
            /// </summary>
            private readonly TEdge m_Edge;

            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveEdgeChange"/> class.
            /// </summary>
            /// <param name="edge">The edge.</param>
            public RemoveEdgeChange(TEdge edge)
            {
                m_Edge = edge;
            }

            /// <summary>
            /// Gets a value indicating whether the change is affected by invoking the
            /// clear method on the graph.
            /// </summary>
            public bool IsAffectedByGraphClear
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            public void ApplyTo(BidirectionalGraph<TVertex, TEdge> historyObject)
            {
                Debug.Assert(historyObject != null, "This change can only be applied to a BidirectionalGraph.");
                historyObject.RemoveEdge(m_Edge);
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{T}"/> which indicates that an vertex was added to a
        /// <see cref="BidirectionalGraph{TVertex, TEdge}"/>.
        /// </summary>
        private sealed class AddVertexChange : IGraphHistoryChange<BidirectionalGraph<TVertex, TEdge>>
        {
            /// <summary>
            /// The new edge.
            /// </summary>
            private readonly TVertex m_Vertex;

            /// <summary>
            /// Initializes a new instance of the <see cref="AddVertexChange"/> class.
            /// </summary>
            /// <param name="vertex">The new edge.</param>
            public AddVertexChange(TVertex vertex)
            {
                m_Vertex = vertex;
            }

            /// <summary>
            /// Gets a value indicating whether the change is affected by invoking the
            /// clear method on the graph.
            /// </summary>
            public bool IsAffectedByGraphClear
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            public void ApplyTo(BidirectionalGraph<TVertex, TEdge> historyObject)
            {
                Debug.Assert(historyObject != null, "This change can only be applied to a BidirectionalGraph.");
                historyObject.AddVertex(m_Vertex);
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{T}"/> which indicates that an vertex was removed from a
        /// <see cref="BidirectionalGraph{TVertex, TEdge}"/>.
        /// </summary>
        private sealed class RemoveVertexChange : IGraphHistoryChange<BidirectionalGraph<TVertex, TEdge>>
        {
            /// <summary>
            /// The new edge.
            /// </summary>
            private readonly TVertex m_Vertex;

            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveVertexChange"/> class.
            /// </summary>
            /// <param name="vertex">The vertex.</param>
            public RemoveVertexChange(TVertex vertex)
            {
                m_Vertex = vertex;
            }

            /// <summary>
            /// Gets a value indicating whether the change is affected by invoking the
            /// clear method on the graph.
            /// </summary>
            public bool IsAffectedByGraphClear
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            public void ApplyTo(BidirectionalGraph<TVertex, TEdge> historyObject)
            {
                Debug.Assert(historyObject != null, "This change can only be applied to a BidirectionalGraph.");
                historyObject.RemoveVertex(m_Vertex);
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{T}"/> which indicates that the <see cref="BidirectionalGraph{TVertex, TEdge}"/>
        /// was cleared.
        /// </summary>
        private sealed class GraphClearChange : IGraphHistoryChange<BidirectionalGraph<TVertex, TEdge>>
        {
            /// <summary>
            /// Gets a value indicating whether the change is affected by invoking the
            /// clear method on the graph.
            /// </summary>
            public bool IsAffectedByGraphClear
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            public void ApplyTo(BidirectionalGraph<TVertex, TEdge> historyObject)
            {
                Debug.Assert(historyObject != null, "This change can only be applied to a BidirectionalGraph.");
                historyObject.Clear();
            }
        }
    }
}
