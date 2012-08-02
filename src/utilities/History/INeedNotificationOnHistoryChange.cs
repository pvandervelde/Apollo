//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for objects that need clean-up before removal from the 
    /// timeline.
    /// </summary>
    public interface INeedNotificationOnHistoryChange
    {
        /// <summary>
        /// Provides implementing classes with the ability to clean-up before
        /// the object is removed from history.
        /// </summary>
        void BeforeRemoval();
    }
}
