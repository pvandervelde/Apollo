//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

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
        /// <param name="estimatedTime">
        ///     The amount of time it will take to finish the entire task from start to finish. Can be negative 
        ///     if no time is known.
        /// </param>
        void Step(int progress, TimeSpan estimatedTime);

        /// <summary>
        /// Provides the progress for the current step.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="mark">The action that is currently being processed.</param>
        /// <param name="estimatedTime">
        ///     The amount of time it will take to finish the entire task from start to finish. Can be negative 
        ///     if no time is known.
        /// </param>
        void Step(int progress, IProgressMark mark, TimeSpan estimatedTime);
    }
}
