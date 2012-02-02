//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for objects that store object member variable values 
    /// in the timeline.
    /// </summary>
    /// <typeparam name="T">The type of the member variable for which the values are stored.</typeparam>
    [DefineAsHistoryTrackingInterface]
    public interface IVariableTimeline<T>
    {
        /// <summary>
        /// An event raised when the the stored value is changed externally.
        /// </summary>
        event EventHandler<EventArgs> OnExternalValueUpdate;

        /// <summary>
        /// Gets or sets the current value for the variable.
        /// </summary>
        T Current
        {
            get;
            set;
        }
    }
}
