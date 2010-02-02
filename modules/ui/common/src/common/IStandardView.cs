//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace Apollo.UI.Common
{
    /// <summary>
    /// Implemented by views.
    /// </summary>
    public interface IStandardView
    {
        /// <summary>
        /// Occurs when the view is shown.
        /// </summary>
        event EventHandler Shown;

        /// <summary>
        /// Occurs when the view is closing.
        /// </summary>
        event CancelEventHandler Closing;

        /// <summary>
        /// Occurs when the view is closed.
        /// </summary>
        event EventHandler Closed;
    }
}