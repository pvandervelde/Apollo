//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// The interface that defines the methods for views that display information 
    /// about <c>Dataset</c> objects.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
       Justification = "We need an interface for the view because Prism needs it.")]
    public interface IMachineSelectorView : IView<MachineSelectorModel>
    {
    }
}
