//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.UI.Common;

namespace Apollo.ProjectExplorer.Views.Shell
{
    /// <summary>
    /// A <see cref="Parameter"/> used by the shell.
    /// </summary>
    internal sealed class ShellParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public ShellParameter(IContextAware context)
            : base(context)
        { 
        }
    }
}
