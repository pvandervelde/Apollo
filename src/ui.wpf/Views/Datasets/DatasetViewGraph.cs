//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using QuickGraph;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// Defines a graph that displays the relations between datasets for the <see cref="DatasetGraphView"/>.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DatasetViewGraph : BidirectionalGraph<DatasetViewVertex, DatasetViewEdge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetViewGraph"/> class.
        /// </summary>
        public DatasetViewGraph()
            : this(false)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetViewGraph"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        public DatasetViewGraph(bool allowParallelEdges)
            : base(allowParallelEdges) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetViewGraph"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <param name="vertexCapacity">Indicates the initial number of vertices.</param>
        public DatasetViewGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) 
        { 
        }
    }
}
