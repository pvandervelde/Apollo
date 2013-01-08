//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Wpf.Views.Notification
{
    /// <summary>
    /// Defines a parameter for the <see cref="NotificationModel"/>.
    /// </summary>
    public sealed class NotificationParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public NotificationParameter(IContextAware context)
            : base(context)
        { 
        }
    }
}
