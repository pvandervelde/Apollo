//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities.Properties;
using Lokad;

namespace Apollo.Utilities
{
    /// <summary>
    /// Event arguments used for startup progress events.
    /// </summary>
    [Serializable]
    public sealed class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressEventArgs"/> class.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="currentlyProcessing">The action that is currently being processed.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="progress"/> is less than 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="progress"/> is more than 100.</exception>
        public ProgressEventArgs(int progress, IProgressMark currentlyProcessing)
            : this(progress, currentlyProcessing, TimeSpan.FromTicks(-1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressEventArgs"/> class.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="currentlyProcessing">The action that is currently being processed.</param>
        /// <param name="estimatedTimeTillFinish">
        ///     The amount of time it will take to finish the entire task from start to finish. Can be negative 
        ///     if no time is known.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="progress"/> is less than 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="progress"/> is more than 100.</exception>
        public ProgressEventArgs(int progress, IProgressMark currentlyProcessing, TimeSpan estimatedTimeTillFinish)
        {
            {
                Enforce.With<ArgumentOutOfRangeException>(progress >= 0, Resources.Exceptions_Messages_ProgressToSmall, progress);
                Enforce.With<ArgumentOutOfRangeException>(progress <= 100, Resources.Exceptions_Messages_ProgressToLarge, progress);
                Enforce.Argument(() => currentlyProcessing);
            }

            Progress = progress;
            CurrentlyProcessing = currentlyProcessing;
            EstimatedFinishingTime = estimatedTimeTillFinish;
        }

        /// <summary>
        /// Gets the progress percentage, ranging from 0 to 100.
        /// </summary>
        public int Progress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets an object describing the action that is currently being processed.
        /// </summary>
        public IProgressMark CurrentlyProcessing
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the time it will take to finish the complete task from start to finish.
        /// </summary>
        public TimeSpan EstimatedFinishingTime
        {
            get;
            private set;
        }
    }
}
