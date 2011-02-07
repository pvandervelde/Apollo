//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils.Properties;
using Lokad;

namespace Apollo.Utils
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
        private readonly IProgressMark m_CurrentlyProcessing;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupProgressEventArgs"/> class.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="currentlyProcessing">The action that is currently being processed.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="progress"/> is less than 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="progress"/> is more than 100.</exception>
        public StartupProgressEventArgs(int progress, IProgressMark currentlyProcessing)
        {
            {
                Enforce.With<ArgumentOutOfRangeException>(progress >= 0, Resources.Exceptions_Messages_ProgressToSmall, progress);
                Enforce.With<ArgumentOutOfRangeException>(progress <= 100, Resources.Exceptions_Messages_ProgressToLarge, progress);
                Enforce.Argument(() => currentlyProcessing);
            }

            m_Progress = progress;
            m_CurrentlyProcessing = currentlyProcessing;
        }

        /// <summary>
        /// Gets the progress percentage, ranging from 0 to 100.
        /// </summary>
        /// <value>The progress percentage.</value>
        public int Progress
        {
            get 
            { 
                return m_Progress; 
            }
        }

        /// <summary>
        /// Gets a string describing the action that is currently being processed.
        /// </summary>
        /// <value>The action that is currently being processed.</value>
        public IProgressMark CurrentlyProcessing
        {
            get 
            { 
                return m_CurrentlyProcessing; 
            }
        }
    }
}
