//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.Core.Base.Plugins;
using QuickGraph;

namespace Apollo.Core.Dataset.Plugins
{
    /// <summary>
    /// Defines an <see cref="Edge{T}"/> that links two part groups in the <see cref="GroupCompositionLayer"/>.
    /// </summary>
    internal sealed class GroupCompositionGraphEdge : Edge<GroupCompositionId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCompositionGraphEdge"/> class.
        /// </summary>
        /// <param name="importingGroup">The ID of the importing group.</param>
        /// <param name="exportingGroup">The ID of the exporting group.</param>
        public GroupCompositionGraphEdge(GroupCompositionId importingGroup, GroupCompositionId exportingGroup)
            : base(exportingGroup, importingGroup)
        { 
        }
    }
}
