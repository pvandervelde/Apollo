//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the method that need to be implemented for an object to 
    /// have a history timeline.
    /// </summary>
    public interface IAmHistoryEnabled : IDisposable
    {
        /// <summary>
        /// Gets the ID which relates the object to the timeline.
        /// </summary>
        HistoryId HistoryId
        {
            get;
        }
    }
}
