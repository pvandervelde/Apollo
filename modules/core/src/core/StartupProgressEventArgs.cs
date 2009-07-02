//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core
{
    /// <summary>
    /// Event arguments used for startup progress events.
    /// </summary>
    public sealed class StartupProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Stores the current progress percentage, ranging from 0 to 100.
        /// </summary>
        private readonly int m_Progress;
        
        /// <summary>
        /// Stores the element that is currently being processed.
        /// </summary>
        private readonly string m_CurrentlyProcessing;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupProgressEventArgs"/> class.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="currentlyProcessing">The action that is currently being processed.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="progress"/> is less than 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="progress"/> is more than 100.</exception>
        public StartupProgressEventArgs(int progress, string currentlyProcessing)
        {
            // Argument validation.
            {
                if (progress < 0)
                {
                    throw new ArgumentOutOfRangeException("progress", "Progress needs to be 0% or larger.");
                }

                if (progress > 100)
                {
                    throw new ArgumentOutOfRangeException("progress", "Progress needs to be 100% or smaller.");
                }
            }

            m_Progress = progress;
            m_CurrentlyProcessing = currentlyProcessing ?? string.Empty;
        }

        /// <summary>
        /// Gets the progress percentage, ranging from 0 to 100.
        /// </summary>
        /// <value>The progress percentage.</value>
        public int Progress
        {
            get { return m_Progress; }
        }

        /// <summary>
        /// Gets a string describing the action that is currently being processed.
        /// </summary>
        /// <value>The action that is currently being processed.</value>
        public string CurrentlyProcessing
        {
            get { return m_CurrentlyProcessing; }
        }
    }
}
