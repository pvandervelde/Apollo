﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// The interface for objects that display information about
    /// a script.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "We need an interface for the view because Prism needs it.")]
    public interface IScriptView : IView<ScriptModel>
    {
    }
}
