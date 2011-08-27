//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.UI.Common;

namespace Apollo.ProjectExplorer.Views.StatusBar
{
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "We need an interface for the view because Prism needs it.")]
    internal interface IStatusBarView : IView<StatusBarModel>
    {
    }
}
