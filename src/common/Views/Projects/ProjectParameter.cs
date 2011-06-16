﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Common.Views.Projects
{
    /// <summary>
    /// A parameter for the project view.
    /// </summary>
    public sealed class ProjectParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public ProjectParameter(IContextAware context)
            : base(context)
        { 
        }
    }
}
