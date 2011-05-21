//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Practices.Prism.Events;

namespace Apollo.ProjectExplorer.Events
{
    /// <summary>
    /// Manages publication and subscription of the showing of a view event.
    /// </summary>
    internal class ShowViewEvent : CompositePresentationEvent<ShowViewRequest>
    {
    }
}
