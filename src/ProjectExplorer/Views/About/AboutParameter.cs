//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.UI.Common;

namespace Apollo.ProjectExplorer.Views.About
{
    /// <summary>
    /// A parameter for the about view.
    /// </summary>
    internal sealed class AboutParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public AboutParameter(IContextAware context)
            : base(context)
        { 
        }
    }
}
