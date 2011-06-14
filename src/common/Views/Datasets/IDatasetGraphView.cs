//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// The interface for views that display information about 
    /// the <c>Dataset</c> connection graph.
    /// </summary>
     [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "We need an interface for the view because Prism needs it.")]
    [CLSCompliant(false)]
    public interface IDatasetGraphView : IView<DatasetGraphModel>
    {
    }
}
