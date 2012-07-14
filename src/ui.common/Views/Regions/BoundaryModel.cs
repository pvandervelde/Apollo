//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Common.Views.Regions
{
    /// <summary>
    /// A graph vertex that stores a link to a boundary.
    /// </summary>
    public sealed class BoundaryModel : Model, IRegionElementModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoundaryModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public BoundaryModel(IContextAware context)
            : base(context)
        {
        }
    }
}
