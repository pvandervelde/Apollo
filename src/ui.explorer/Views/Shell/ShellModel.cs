//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.UI.Wpf;

namespace Apollo.UI.Explorer.Views.Shell
{
    /// <summary>
    /// The model for the shell.
    /// </summary>
    internal sealed class ShellModel : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public ShellModel(IContextAware context)
            : base(context)
        { 
        }
    }
}
