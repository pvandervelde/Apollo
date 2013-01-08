//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.UI.Wpf;

namespace Apollo.UI.Explorer.Views.StatusBar
{
    internal sealed class StatusBarParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBarParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public StatusBarParameter(IContextAware context)
            : base(context)
        { 
        }
    }
}
