//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.UI.Wpf;

namespace Apollo.UI.Explorer.Views.Welcome
{
    /// <summary>
    /// A <see cref="Parameter"/> used by the welcome view.
    /// </summary>
    internal sealed class WelcomeParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public WelcomeParameter(IContextAware context)
            : base(context)
        { 
        }
    }
}
