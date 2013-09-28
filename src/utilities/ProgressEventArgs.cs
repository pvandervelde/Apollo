//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities.Properties;

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
        /// <param name="description">The description for the current progress point.</param>
        /// <param name="hasErrors">A value indicating whether errors have occurred during processing.</param>
        public ProgressEventArgs(int progress, string description, bool hasErrors)
        {
            {
                Lokad.Enforce.With<ArgumentOutOfRangeException>(progress >= 0, Resources.Exceptions_Messages_ProgressToSmall, progress);
                Lokad.Enforce.With<ArgumentOutOfRangeException>(progress <= 100, Resources.Exceptions_Messages_ProgressToLarge, progress);
            }

            Progress = progress;
            Description = description;
            HasErrors = hasErrors;
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
        /// Gets the description for the current progress point.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether errors have occurred during processing.
        /// </summary>
        public bool HasErrors
        {
            get;
            private set;
        }
    }
}
