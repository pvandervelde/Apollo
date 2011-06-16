//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// A <see cref="Parameter"/> for the <see cref="SelectScriptLanguagePresenter"/>.
    /// </summary>
    public sealed class SelectScriptLanguageParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectScriptLanguageParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public SelectScriptLanguageParameter(IContextAware context)
            : base(context)
        { 
        }
    }
}
