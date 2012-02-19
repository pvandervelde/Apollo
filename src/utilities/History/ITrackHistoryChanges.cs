//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Tracks changes in history through <see cref="IChangeSet"/> objects.
    /// </summary>
    public interface ITrackHistoryChanges
    {
        /// <summary>
        /// Starts recording history and returns a change set for those changes.
        /// </summary>
        /// <returns>
        /// The change set that records the changes when it is disposed.
        /// </returns>
        IChangeSet RecordHistory();

        /// <summary>
        /// Starts recording history and returns a named change set for those changes.
        /// </summary>
        /// <param name="name">The name of the change set.</param>
        /// <returns>
        /// The change set that records the changes when it is disposed.
        /// </returns>
        IChangeSet RecordHistory(string name);
    }
}
