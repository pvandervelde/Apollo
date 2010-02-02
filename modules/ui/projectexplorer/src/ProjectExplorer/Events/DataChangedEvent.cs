//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Practices.Composite.Presentation.Events;

namespace Apollo.ProjectExplorer.Events
{
    /// <summary>
    /// Manages publication and subscription of data changed events.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class DataChangedEvent<TData> : CompositePresentationEvent<TData>
    {
    }
}