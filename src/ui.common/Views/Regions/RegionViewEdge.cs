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
    /// A graph edge which connects different <see cref="IRegionElementModel"/> objects.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class RegionViewEdge : Edge<IRegionElementModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionViewEdge"/> class.
        /// </summary>
        /// <param name="source">The vertex from which the edge leaves.</param>
        /// <param name="target">The vertex at which the edge arrives.</param>
        public RegionViewEdge(IRegionElementModel source, IRegionElementModel target)
            : base(source, target)
        {
        }
    }
}
