//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using QuickGraph;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for graph objects that track the history of changes to the graph.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex that is stored in the graph.</typeparam>
    /// <typeparam name="TEdge">The type of edge that is stored in the graph.</typeparam>
    [DefineAsHistoryTrackingInterface]
    [CLSCompliant(false)]
    public interface IBidirectionalGraphHistory<TVertex, TEdge> :
        IEdgeListAndIncidenceGraph<TVertex, TEdge>,
        IMutableBidirectionalGraph<TVertex, TEdge>,
        ICloneable,
        IStoreTimelineValues where TEdge : IEdge<TVertex>
    {
    }
}
