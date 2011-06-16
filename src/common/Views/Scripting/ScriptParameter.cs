//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// A parameter for the script view.
    /// </summary>
    public sealed class ScriptParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public ScriptParameter(IContextAware context)
            : base(context)
        { 
        }
    }
}
