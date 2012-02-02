//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utilities.History;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for objects that store a single change to a graph that tracks history.
    /// </summary>
    /// <typeparam name="T">The type of history object.</typeparam>
    internal interface IGraphHistoryChange<T> : IHistoryChange<T>
    {
        /// <summary>
        /// Gets a value indicating whether the change is affected by invoking the
        /// clear method on the graph.
        /// </summary>
        bool IsAffectedByGraphClear
        {
            get;
        }
    }
}
