//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Common.Views.Scheduling
{
    /// <summary>
    /// The interface for views that display information about the available schedules.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
       Justification = "We need an interface for the view because Prism needs it.")]
    public interface ISchedulingView : IView<SchedulingModel>
    {
    }
}
