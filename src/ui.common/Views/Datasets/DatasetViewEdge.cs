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
    /// A graph edge which connects different <see cref="DatasetViewVertex"/> objects.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DatasetViewEdge : Edge<DatasetViewVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetViewEdge"/> class.
        /// </summary>
        /// <param name="source">The vertex from which the edge leaves.</param>
        /// <param name="target">The vertex at which the edge arrives.</param>
        public DatasetViewEdge(DatasetViewVertex source, DatasetViewVertex target)
            : base(source, target)
        { 
        }
    }
}
