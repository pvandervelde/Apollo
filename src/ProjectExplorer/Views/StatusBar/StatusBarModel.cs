//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.UI.Common;

namespace Apollo.ProjectExplorer.Views.StatusBar
{
    /// <summary>
    /// The model for the status bar.
    /// </summary>
    internal sealed class StatusBarModel : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBarModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public StatusBarModel(IContextAware context)
            : base(context)
        { 
        }
    }
}
