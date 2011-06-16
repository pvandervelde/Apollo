//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.UI.Common;

namespace Apollo.ProjectExplorer.Views.Menu
{
    /// <summary>
    /// A parameter for the <see cref="MenuPresenter"/>.
    /// </summary>
    internal class MenuParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public MenuParameter(IContextAware context)
            : base(context)
        { 
        }
    }
}
