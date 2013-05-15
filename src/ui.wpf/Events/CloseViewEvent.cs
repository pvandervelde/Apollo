//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Wpf.Events
{
    /// <summary>
    /// Manages publication and subscription of the closing of a view event.
    /// </summary>
    public sealed class CloseViewEvent : CompositePresentationEvent<CloseViewRequest>
    {
    }
}
