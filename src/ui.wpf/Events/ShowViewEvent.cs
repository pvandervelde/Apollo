//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Wpf.Events
{
    /// <summary>
    /// Manages publication and subscription of the showing of a view event.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ShowViewEvent : CompositePresentationEvent<ShowViewRequest>
    {
    }
}
