//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// A parameter for the dataset graph view.
    /// </summary>
    public sealed class DatasetGraphParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetGraphParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public DatasetGraphParameter(IContextAware context)
            : base(context)
        { 
        }
    }
}
