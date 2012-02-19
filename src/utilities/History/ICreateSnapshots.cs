//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for objects that create snapshots.
    /// </summary>
    public interface ICreateSnapshots
    {
        /// <summary>
        /// Creates a new snapshot and stores it.
        /// </summary>
        void CreateSnapshot();
    }
}
