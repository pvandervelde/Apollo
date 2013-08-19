//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for objects which track progress of a stepped process.
    /// </summary>
    public interface ITrackSteppingProgress : ITrackProgress
    {
        /// <summary>
        /// Provides the progress for the current step.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="mark">The action that is currently being processed.</param>
        void UpdateProgress(int progress, string mark);
    }
}
