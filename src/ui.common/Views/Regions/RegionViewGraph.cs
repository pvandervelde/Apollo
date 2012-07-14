//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using QuickGraph;

namespace Apollo.UI.Common.Views.Regions
{
    /// <summary>
    /// Defines a graph that displays the relations between datasets for the regions.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class RegionViewGraph : BidirectionalGraph<IRegionElementModel, RegionViewEdge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionViewGraph"/> class.
        /// </summary>
        public RegionViewGraph()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionViewGraph"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        public RegionViewGraph(bool allowParallelEdges)
            : base(allowParallelEdges)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionViewGraph"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <param name="vertexCapacity">Indicates the initial number of vertices.</param>
        public RegionViewGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity)
        {
        }
    }
}
