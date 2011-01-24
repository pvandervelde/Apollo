//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Utils
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that stores information about a change in
    /// the current <see cref="IProgressMark"/> on an <see cref="ITrackProgress"/> object.
    /// </summary>
    public sealed class ProgressMarkEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressMarkEventArgs"/> class.
        /// </summary>
        /// <param name="mark">The progress mark.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="mark"/> is <see langword="null"/>
        /// </exception>
        public ProgressMarkEventArgs(IProgressMark mark)
        {
            {
                Enforce.Argument(() => mark);
            }

            Mark = mark;
        }

        /// <summary>
        /// Gets the progress mark.
        /// </summary>
        /// <value>The progress mark.</value>
        public IProgressMark Mark 
        { 
            get; 
            private set; 
        }
    }
}
