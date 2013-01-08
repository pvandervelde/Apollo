//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Wpf.Views.Feedback
{
    /// <summary>
    /// Defines a parameter for the <see cref="FeedbackModel"/>.
    /// </summary>
    public sealed class FeedbackParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public FeedbackParameter(IContextAware context)
            : base(context)
        {
        }
    }
}
