//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Common
{
    /// <summary>
    /// Implemented by views which are shown in a region.
    /// </summary>
    public interface IRegionView : IStandardView
    {
        /// <summary>
        /// Occurs when the view is activated.
        /// </summary>
        event EventHandler OnActivated;

        /// <summary>
        /// Occurs when the view is deactivated.
        /// </summary>
        event EventHandler OnDeactivated;
    }
}
