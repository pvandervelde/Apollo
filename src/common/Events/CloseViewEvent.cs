﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Common.Events
{
    /// <summary>
    /// Manages publication and subscription of the closing of a view event.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class CloseViewEvent : CompositePresentationEvent<CloseViewRequest>
    {
    }
}
